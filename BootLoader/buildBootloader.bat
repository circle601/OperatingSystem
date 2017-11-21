SETLOCAL 
echo off
call "D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\BootLoader\Stage1\Build.cmd"
call "D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\BootLoader\Stage2\Build.cmd"

SET bootloader_output=D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\BIN
SET build_output=D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\BootLoader\BIN


copy "%build_output%\MBRboot.img" "D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\MBRboot.img" 
copy /B "%build_output%\KRNLDR.BIN" "%bootloader_output%\KRNLDR.BIN"


pause