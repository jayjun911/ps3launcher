using IniParser;
using IniParser.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace ps3Launcher
{
    class RPCS3Launcher
    {
        private IsoMounter game_drive;
        private Process game_process;
        private string rpcs3_exe;
        private string default_drive_letter;
        private IniData config;
        private string iso_file;
        private FileIniDataParser parser;
        private bool eboot_mode;



        public RPCS3Launcher(string iso)
        {

            parser = new FileIniDataParser();            
            config = parser.ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ps3launcher.ini"));
            rpcs3_exe = config["launcher"]["emulator"];
            default_drive_letter = config["launcher"]["driveLetter"];
            iso_file = iso;
            eboot_mode = false;
            PrepareGameDrive();
        }

        private void PrepareGameDrive()
        {
            // Before mounting new game, clean up previous session
            Kill_Emulator();

            // check iso or bin
            if (this.iso_file.ToUpper().Contains("EBOOT.BIN"))
            {
                eboot_mode = true;
                return;
            }

            // IsoMounter object            
            game_drive = new IsoMounter(this.iso_file, this.default_drive_letter); //default letter to 'unmount' first            
            
            // Mount a new iso 
            game_drive.MountISO();
            // Update default drive letter if necessary            
            if (this.default_drive_letter != game_drive.GetDriveLetter())
            {
                // update default drive letter
                this.config["launcher"]["driveLetter"] = game_drive.GetDriveLetter();
                parser.WriteFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ps3launcher.ini"), this.config);
            }

        }

        private void Kill_Emulator()
        {
            try
            {
                foreach (Process proc in Process.GetProcessesByName("rpcs3"))
                {
                    proc.Kill();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                System.Environment.Exit(-1);
            }
        }

        internal void Launch()
        {            
            
            if(eboot_mode)
            {
                game_process = Process.Start(rpcs3_exe, "\"" + this.iso_file + "\"");
            }else
            {
                string eboot = game_drive.GetDriveLetter() + @":\PS3_GAME\USRDIR\EBOOT.BIN";
                game_process = Process.Start(rpcs3_exe, eboot);
            }            
        }

        internal void WaitCleanup()
        {
            // wait until game process ends, then unmount            
            this.game_process.WaitForExit();
            if(eboot_mode == false)
                game_drive.Unmount();                
        }
    }
}
