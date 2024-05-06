using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace DotNetOSSIndex
{
    [Command(Name = "dotnet ossindex", FullName = "A .NET Core global tool to check the packages for known vulnerabilities.")]
    class Program
    {
        [Argument(0, Name = "Path", Description = "The path to a .sln, .csproj or .vbproj file")]
        string SolutionOrProjectFile { get; set; }

        [Option(Description = "OSS Index Username", ShortName = "u")]
        string Username { get; }

        [Option(Description = "OSS Index API Token", ShortName = "a")]
        string ApiToken { get; }

        static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        async Task<int> OnExecute()
        {
            WriteLine();

            if (string.IsNullOrEmpty(SolutionOrProjectFile))
            {
                WriteLine("Path is required", ConsoleColor.Red);

                return 1;
            }

            var extension = Path.GetExtension(SolutionOrProjectFile);

            if (extension.Equals(".sln", StringComparison.OrdinalIgnoreCase))
            {
                var solutionFile = Path.GetFullPath(SolutionOrProjectFile);

                return await AnalyzeSolutionAsync(solutionFile);
            }

            if (extension.Equals(".csproj", StringComparison.OrdinalIgnoreCase) || extension.Equals(".vbproj", StringComparison.OrdinalIgnoreCase))
            {
                var projectFile = Path.GetFullPath(SolutionOrProjectFile);

                return await AnalyzeProjectAsync(projectFile);
            }

            WriteLine("Only .sln, .csproj and .vbproj files are supported", ConsoleColor.Red);

            return 1;
        }

        async Task<int> AnalyzeSolutionAsync(string solutionFile)
        {
            if (!File.Exists(solutionFile))
            {
                WriteLine($"Solution file \"{solutionFile}\" does not exist", ConsoleColor.Red);

                return 1;
            }

            WriteLine($"> Solution: {solutionFile}", ConsoleColor.Green);
            WriteLine();

            var solutionFolder = Path.GetDirectoryName(solutionFile);
            var projects = new List<string>();

            try
            {
                using var reader = File.OpenText(solutionFile);
                string line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (!line.StartsWith("Project", StringComparison.InvariantCulture))
                    {
                        continue;
                    }

                    var regex = new Regex("(.*) = \"(.*?)\", \"(.*?.(cs|vb)proj)\"");
                    var match = regex.Match(line);

                    if (match.Success)
                    {
                        var projectFileName = Path.DirectorySeparatorChar == '/'
                            ? GetPathWithForwardSlashes(match.Groups[3].Value)
                            : GetPathWithBackSlashes(match.Groups[3].Value);

                        var projectFullPath = Path.GetFullPath(Path.Combine(solutionFolder, projectFileName));

                        projects.Add(projectFullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLine($"  An unhandled exception occurred while getting the projects: {ex.Message}");

                return 1;
            }

            if (!projects.Any())
            {
                WriteLine("  No projects found");

                return 0;
            }

            WriteLine($"  {projects.Count} project(s) found");

            foreach (var project in projects)
            {
                WriteLine();

                var ret = await AnalyzeProjectAsync(project);

                if (ret != 0)
                {
                    return ret;
                }
            }

            return 0;
        }

        async Task<int> AnalyzeProjectAsync(string projectFile)
        {
            if (!File.Exists(projectFile))
            {
                WriteLine($"Project file \"{projectFile}\" does not exist", ConsoleColor.Red);

                return 1;
            }

            WriteLine($"> Project: {projectFile}", ConsoleColor.Blue);
            WriteLine();

            var coordinates = new List<string>();
            var skippedPackages = new List<(string packageName, string reason)>();

            try
            {
                using var reader = XmlReader.Create(projectFile);

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "PackageReference":
                                var packageName = reader["Include"];
                                var packageVersion = reader["Version"];

                                if (string.IsNullOrEmpty(packageVersion))
                                {
                                    skippedPackages.Add(($"pkg:nuget/{packageName}", "Package is referenced without version"));
                                }
                                else
                                {
                                    coordinates.Add($"pkg:nuget/{packageName}@{packageVersion}");
                                }

                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLine($"  An unhandled exception occurred while getting the packages: {ex.Message}", ConsoleColor.Red);

                return 1;
            }

            if (skippedPackages.Any())
            {
                WriteLine($"  {skippedPackages.Count()} package(s) skipped");
                WriteLine();

                foreach (var (packageName, reason) in skippedPackages)
                {
                    WriteLine($"          Package: {packageName}");
                    WriteLine($"           Reason: {reason}");
                    WriteLine();
                }
            }

            if (!coordinates.Any())
            {
                WriteLine("  No packages found");

                return 0;
            }

            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("dotnet-ossindex", typeof(Program).Assembly.GetName().Version.ToString()));

            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(ApiToken))
            {
                var bytes = Encoding.UTF8.GetBytes($"{Username}:{ApiToken}");
                var value = Convert.ToBase64String(bytes);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", value);
            }

            var request = new
            {
                coordinates
            };

            var requestAsString = JsonConvert.SerializeObject(request);
            var requestAsStringContent = new StringContent(requestAsString, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            try
            {
                response = await client.PostAsync("https://ossindex.sonatype.org/api/v3/component-report", requestAsStringContent);
            }
            catch (Exception ex)
            {
                WriteLine($"  An unhandled exception occurred while checking for vulnerabilities: {ex.Message}", ConsoleColor.Red);

                return 1;
            }

            var contentAsString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                WriteLine($"  An unhandled exception occurred while checking for vulnerabilities: {(int)response.StatusCode} {response.StatusCode} {contentAsString}", ConsoleColor.Red);

                return 1;
            }

            Component[] components;

            try
            {
                components = JsonConvert.DeserializeObject<Component[]>(contentAsString);
            }
            catch (Exception ex)
            {
                WriteLine($"  An unhandled exception occurred while checking for vulnerabilities: {ex.Message}", ConsoleColor.Red);

                return 1;
            }

            var affectedComponents = components.Where(c => c.Vulnerabilities.Length > 0);

            if (!affectedComponents.Any())
            {
                WriteLine("  No packages affected");

                return 0;
            }

            WriteLine($"  {affectedComponents.Count()} package(s) affected");

            foreach (var component in affectedComponents)
            {
                WriteLine();
                WriteLine($"          Package: {component.Coordinates}");
                WriteLine($"        Reference: {component.Reference}");
                Write($"  Vulnerabilities:");

                foreach (var vulnerability in component.Vulnerabilities.OrderByDescending(v => v.CVSSScore))
                {
                    // Severity scale
                    // https://www.first.org/cvss/specification-document#5-Qualitative-Severity-Rating-Scale
                    var severity = "NONE";
                    var severityForegroundColor = Console.ForegroundColor;

                    if (vulnerability.CVSSScore >= 0.1 && vulnerability.CVSSScore <= 3.9)
                    {
                        severity = "LOW";
                    }

                    if (vulnerability.CVSSScore >= 4.0 && vulnerability.CVSSScore <= 6.9)
                    {
                        severity = "MEDIUM";
                        severityForegroundColor = ConsoleColor.Yellow;
                    }

                    if (vulnerability.CVSSScore >= 7.0 && vulnerability.CVSSScore <= 8.9)
                    {
                        severity = "HIGH";
                        severityForegroundColor = ConsoleColor.Red;
                    }

                    if (vulnerability.CVSSScore >= 9.0)
                    {
                        severity = "CRITICAL";
                        severityForegroundColor = ConsoleColor.Red;
                    }

                    WriteLine($" - {severity,-8} {vulnerability.Title}", severityForegroundColor);
                    Write($"                  ");
                }
            }

            return 0;
        }

        static void Write(string value)
        {
            Console.Write(value);
        }

        static void WriteLine()
        {
            Console.WriteLine(string.Empty);
        }

        static void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        static void WriteLine(string value, ConsoleColor foregroundColor)
        {
            var currentForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = foregroundColor;

            Console.WriteLine(value);

            Console.ForegroundColor = currentForegroundColor;
        }

        static string GetPathWithForwardSlashes(string path)
        {
            return path.Replace('\\', '/');
        }

        static string GetPathWithBackSlashes(string path)
        {
            return path.Replace('/', '\\');
        }
    }

    struct Component
    {
        public string Coordinates { get; set; }

        public string Reference { get; set; }

        public Vulnerability[] Vulnerabilities { get; set; }
    }

    struct Vulnerability
    {
        public string Title { get; set; }

        public float CVSSScore { get; set; }
    }
}