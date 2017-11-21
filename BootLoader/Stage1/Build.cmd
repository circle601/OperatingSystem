SETLOCAL 

SET Stage=%~dp0
SET build_output=D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\BootLoader\BIN
SET build_gcc=D:\MinGW\bin\
SET CYGWIN=D:\cygwin\bin

echo "init done"
echo "Assembling"
D:\Users\alex\AppData\Local\nasm\nasm.exe -f bin "%Stage%\Boot1.asm" -o "%build_output%\Boot1.bin"

echo "Creating image"
%CYGWIN%\dd if=/dev/zero of="%build_output%\MBRboot.img" bs=512 count=2880
%CYGWIN%\dd if="%build_output%\Boot1.bin" of="%build_output%\MBRboot.img" bs=512 count=1 conv=notrunc
echo "Created"






