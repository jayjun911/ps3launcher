using IniParser;
using IniParser.Model;
using System;
using System.Diagnostics;
using System.IO;


namespace ps3Launcher
{
    class Program
    {       
        static void Main(string[] args)
        {
            // launch rpcs3, wait until exit, clean up
            RPCS3Launcher rpcs3 = new RPCS3Launcher(args[0]);
            rpcs3.Launch();
            rpcs3.WaitCleanup();
        }
    }
}
