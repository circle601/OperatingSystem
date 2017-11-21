
SETLOCAL 
SET Stage=%~dp0
SET build_output=D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\BootLoader\BIN
SET build_gcc=D:\MinGW\bin\
SET CYGWIN=D:\cygwin\bin
SET NASMENV="-i"%Stage%""
echo "init done"
echo "Assembling"
 D:\Users\alex\AppData\Local\nasm\nasm.exe  -f bin "%Stage%Stage3.asm" -o "%build_output%\KRNL.SYS" 
echo "Adding To image
%CYGWIN%\mcopy -i "%build_output%\MBRboot.img" "%build_output%\KRNL.SYS" ::/



