using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Deployment.Application;
using Microsoft.Win32;

// <summary> 
// This namespaces if for generic application classes
// </summary>
namespace ADSearch.Scripts
{
    /// <summary> 
    /// Used to get the assembly information 
    /// </summary>
    /// <remarks>
    /// http://danielsaidi.wordpress.com/2009/05/25/c-get-assembly-information/
    /// </remarks>
    public static class AssemblyInfo
    {
        /// <summary> 
        /// Title
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static string Title
        {
            get
            {
                string result = string.Empty;
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((AssemblyTitleAttribute)customAttributes[0]).Title;
                }

                return result;
            }
        }

        /// <summary> 
        /// Description
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static string Description
        {
            get
            {
                string result = string.Empty;
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((AssemblyDescriptionAttribute)customAttributes[0]).Description;
                }
                return result;
            }
        }

        /// <summary> 
        /// Company
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static string Company
        {
            get
            {
                string result = string.Empty;
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((AssemblyCompanyAttribute)customAttributes[0]).Company;
                }

                return result;
            }
        }

        /// <summary> 
        /// Product
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static string Product
        {
            get
            {
                string result = string.Empty;
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((AssemblyProductAttribute)customAttributes[0]).Product;
                }
                return result;
            }
        }

        /// <summary> 
        /// Copyright
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static string Copyright
        {
            get
            {
                string result = string.Empty;
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
                }
                return result;
            }
        }

        /// <summary> 
        /// Trademark
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static string Trademark
        {
            get
            {
                string result = string.Empty;
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((AssemblyTrademarkAttribute)customAttributes[0]).Trademark;
                }
                return result;
            }
        }

        /// <summary> 
        /// AssemblyVersion
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static string AssemblyVersion
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                return assembly.GetName().Version.ToString();
            }
        }

        /// <summary> 
        /// FileVersion
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static string FileVersion
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        /// <summary> 
        /// Guid
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static string Guid
        {
            get
            {
                string result = string.Empty;
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                        result = ((System.Runtime.InteropServices.GuidAttribute)customAttributes[0]).Value;
                }
                return result;
            }
        }

        /// <summary> 
        /// FileName
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static string FileName
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.OriginalFilename;
            }
        }

        /// <summary> 
        /// FilePath
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static string FilePath
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileName;
            }
        }
        
        /// <summary> 
        /// Open a file
        /// </summary>
        /// <param name="filePath">Represents the file path string </param>
        public static void OpenFile(string filePath)
        {
            try
            {
                System.Diagnostics.Process pStart = new System.Diagnostics.Process();
                if (filePath == string.Empty)
                    return;
                pStart.StartInfo.FileName = filePath;
                pStart.Start();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                //MessageBox.Show("No application is assicated to this file type." & vbCrLf & vbCrLf & FilePath, "No action taken.", MessageBoxButtons.OK, MessageBoxIcon.Information)
                return;
            }
            catch (Exception ex)
            {
                ErrorHandler.DisplayMessage(ex);
            }
        }

        /// <summary> 
        /// Returns the assembly location string based on the type of location
        /// </summary>
        /// <param name="locationType">Represents assembly location type </param>
        /// <returns>A method that returns a string of the current location </returns> 
        public static string GetCurrentLocation(string locationType)
        {
            try
            {
                //Get the assembly information
                System.Reflection.Assembly assemblyInfo = System.Reflection.Assembly.GetExecutingAssembly();

                //CodeBase is the location of the ClickOnce deployment files
                Uri uriCodeBase = new Uri(assemblyInfo.CodeBase);

                switch (locationType)
                {
                    case "AssemblyLocation":
                        return assemblyInfo.Location; //Location is where the assembly is run from 
                    case "ClickOnceLocation":
                        return Path.GetDirectoryName(uriCodeBase.LocalPath.ToString());
                    default:
                        return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// set the icon in add/remove programs
        /// </summary>
        /// <param name="iconName">The referenced icon name for the application.</param>
        /// <remarks>
        /// only run if deployed 
        /// </remarks>
        public static void SetAddRemoveProgramsIcon(string iconName)
        {
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed
                 && ApplicationDeployment.CurrentDeployment.IsFirstRun)
            {
                try
                {
                    Assembly code = Assembly.GetExecutingAssembly();
                    AssemblyDescriptionAttribute asdescription =
                        (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(code, typeof(AssemblyDescriptionAttribute));
                    string assemblyDescription = asdescription.Description;

                    //Get the assembly information
                    System.Reflection.Assembly assemblyInfo = System.Reflection.Assembly.GetExecutingAssembly();

                    //CodeBase is the location of the ClickOnce deployment files
                    Uri uriCodeBase = new Uri(assemblyInfo.CodeBase);
                    string clickOnceLocation = Path.GetDirectoryName(uriCodeBase.LocalPath.ToString());

                    //the icon is included in this program
                    string iconSourcePath = Path.Combine(clickOnceLocation, @"Resources\" + iconName);
                    if (!File.Exists(iconSourcePath))
                        return;

                    RegistryKey myUninstallKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
                    string[] mySubKeyNames = myUninstallKey.GetSubKeyNames();
                    for (int i = 0; i < mySubKeyNames.Length; i++)
                    {
                        RegistryKey myKey = myUninstallKey.OpenSubKey(mySubKeyNames[i], true);
                        object myValue = myKey.GetValue("DisplayName");
                        if (myValue != null && myValue.ToString() == assemblyDescription)
                        {
                            myKey.SetValue("DisplayIcon", iconSourcePath);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.DisplayMessage(ex);
                }
            }
        }
    }
}
