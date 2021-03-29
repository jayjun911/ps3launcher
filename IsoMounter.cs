using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace ps3Launcher
{
    class IsoMounter
    {
        private string m_iso = null;
        private string driveLetter = null;
        public IsoMounter(string iso, string drv) 
        {
            if (!File.Exists(iso))
            {
                Debug.WriteLine("ISO File not exists: {0}", iso);
                return;
            }

            char drvChar = drv.Trim().ToCharArray()[0];

            if (!Char.IsLetter(drvChar))
            {
                Debug.WriteLine("Not Valid Drive Letter: {0}", drvChar);
            }
            
            m_iso = iso;
            driveLetter = drvChar.ToString();
        }
        
        public void Unmount()
        {
            using (var ps = PowerShell.Create())
            {
                // Unmount Via Image File Path
                var command = ps.AddCommand("Dismount-DiskImage");
                command.AddParameter("ImagePath", m_iso);
                ps.Invoke();
                ps.Commands.Clear();
                System.Threading.Thread.Sleep(2000);

            }
        }
        
        // Unmount Via Drive Letter
        public void Unmount(string driveLetter)
        {
            using (var ps = PowerShell.Create())
            {                
                ps.AddScript("$ShellApplication = New-Object -ComObject Shell.Application;" +
                    "$ShellApplication.Namespace(17).ParseName(\"" + driveLetter + ":\").InvokeVerb(\"Eject\")");
                ps.Invoke();
                ps.Commands.Clear();
                System.Threading.Thread.Sleep(2000);
            }
        }

        public string FindMountedISO(string iso)
        {
            string drive_letter = string.Empty;
            using (var ps = PowerShell.Create())
            {
                //Get Drive Letter ISO Image Was Mounted To
                var runSpace = ps.Runspace;
                var pipeLine = runSpace.CreatePipeline();
                var getImageCommand = new Command("Get-DiskImage");
                getImageCommand.Parameters.Add("ImagePath", iso);
                pipeLine.Commands.Add(getImageCommand);
                pipeLine.Commands.Add("Get-Volume");

                foreach (PSObject psObject in pipeLine.Invoke())
                {
                    drive_letter = psObject.Members["DriveLetter"].Value.ToString();
                    Debug.Write("Mounted On Drive: " + drive_letter);
                }
                pipeLine.Commands.Clear();
                System.Threading.Thread.Sleep(1000);
            }
            return drive_letter;

        }

        // Mount ISO file
        public string MountISO()
        {
            string drive_letter = FindMountedISO(m_iso);            
            if(drive_letter!=String.Empty)
            {
                this.driveLetter = drive_letter;
                return drive_letter;
            }    

            // First Unmount! 
            this.Unmount(this.driveLetter);
            this.driveLetter = string.Empty;

            // Then Mount ISO
            using (var ps = PowerShell.Create())
            {

                //Mount ISO Image
                var command = ps.AddCommand("Mount-DiskImage");
                command.AddParameter("ImagePath", m_iso);
                command.Invoke();
                ps.Commands.Clear();
                System.Threading.Thread.Sleep(2000);
            }
            this.driveLetter = FindMountedISO(m_iso);
            // return mounted drive letter
            return this.driveLetter;
        }

        public string GetDriveLetter()
        {
            return driveLetter;
        }
    }
}
