# Summary
PS3launcher mounts ps3 iso file and launch rpcs3!

RPCS3 emulator lacks support for ISO file, thus in order to run PS3 iso with RPCS3, iso file must be extracted to the JB folder format. 

PS3Launcher helps to mount ISO file automatically, using PowerShell interface and launch ps3 emulator (rpcs3) with EBOOT.bin from mounted location. 
It is useful when using rpcs3 with LaunchBox etc. 

# Basic Usage

ps3launcher <path to an iso> 
or 
ps3launcher <path to an eboot.bin>

PS3Launcher detects either iso or eboot.bin, if it's an ISO, then it tries to mount it first. 
and it unmounts the drive when you close RPCS3 session (if the same ISO file is mounted at time of closing)

To use with LaunchBox, register ps3launcher as a default PS3 emulator, then pass ISO file or EBOOT.BIN as a game rom!!

# Configuration

It has 'ps3launcher.ini' file that goes with it. 

[launcher]

emulator = [path_to]\rpcs3.exe

driveLetter = E

emulator is a path to the RPCS3 executable, modify ini file to point to the right RPCS3 installation location. 

driveLetter indicates the virtual bluray/dvd location. It tries to unmount whatever drive mounted at the location before tries to mount the new ISO. 
this prevents ever-growing virtual drives. (eg. if an ISO is already mounted at E, new ISO will be mounted at F, G, ... etc)
For whatever reasons, if it fails to unmount at the given location, it'll find next available mount point and mount it. 
driveLetter doesn't need to be manually maintained as it'll automatically updated once it finds the first available mount location. 
However, if you know and want to use the available mount location, you may specify the location here. 

note: it doesn't guarantee the mounting location. PowerShell/Windows interface doesn't have a way to mount ISO to a specific drive letter. 

