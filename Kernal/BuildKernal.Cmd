echo off
SET build_gcc=D:\MinGW\bin\
SET CYGWIN=D:\cygwin\bin
SET NASMENV="-i"%Stage%""


SET kernal_output=D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\Kernal\Kernel
SET build_output=D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\BIN
SET bootloader_output=D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\BIN


copy /B "D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\Kernal\Debug\Hal.dll" "%bootloader_output%\Hal.dll"
copy /B "%kernal_output%\KRNL32.EXE" "%bootloader_output%\KRNL32.EXE"
copy /B "D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\Kernal\proc\proc.exe" "%bootloader_output%\proc.exe"
copy /B "D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\Kernal\JitD\JitD.exe" "%bootloader_output%\JitD.exe"

