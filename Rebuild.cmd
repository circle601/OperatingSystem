SETLOCAL 
echo off
SET Image_output=D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem
SET build_output=D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\BIN

echo Clearing Old files  ________________________________

DEL /S /Q "D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\BIN"
echo Copying resources  ________________________________

robocopy "D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\Resources"  "%build_output%\\"  /s /e

echo Building boot loader  ________________________________
call "D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\BootLoader\buildBootloader"

echo Building Kernal  ________________________________

call "D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\Kernal\BuildKernal"

echo Building DISK  ________________________________



echo on

%CYGWIN%\mmd -i "%Image_output%\MBRboot.img" ::/system

%CYGWIN%\mcopy -s -i "%Image_output%/MBRboot.img" "%build_output%/*" ::/
%CYGWIN%\mcopy -i "%Image_output%/MBRboot.img" "%build_output%/boot.ini" ::/
%CYGWIN%\mcopy -i "%Image_output%/MBRboot.img" "%build_output%/KRNLDR.BIN" ::/



rem -v -Q -b -s -Q -n
pause