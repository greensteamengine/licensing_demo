using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace licensing_demo
{
    class Program
    {
        private static Dictionary<string, IPlugin> plugins = new Dictionary<string, IPlugin>();



        private static string PluginPath = "";



        static void Main(string[] args)
        {
            Console.WriteLine("Start!");
            //current directory?
            Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
