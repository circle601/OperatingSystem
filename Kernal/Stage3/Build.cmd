SETLOCAL
SET CYGWIN=D:\cygwin\bin
SET build_gcc=D:\MinGW\bin\
SET Kernal_output=D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\Kernal\Stage3
SET Kernal_input=D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\Kernal\Stage3

SET build_output=D:\Users\alex\Documents\Visual Studio 2015\Projects\OperatingSystem\BIN


SET gccextra= -Wall -ffreestanding -nodefaultlibs -nostartfiles -nostdlib -std=gnu99 -fno-rtti
rem --oformat=binary -fno-toplevel-reorder
rem  D:\Users\alex\AppData\Local\nasm\nasm.exe   "%Kernal_input%\kernal_Entry.asm" -o "%build_output%\kernel_entry.obj" 

%build_gcc%\GCC.exe  -T linker.ld -ffreestanding -O2 -nostdlib -lgcc -fno-toplevel-reorder  "%Kernal_output%\Debug\start.obj" 
%CYGWIN%\objcopy -O binary --only-section=.text   Stage3.exe KERNAL.SYS
%CYGWIN%\objcopy --only-section=.bss   Stage3.exe KRNbSS.dat


%CYGWIN%\mcopy -o -n -i  "%build_output%\MBRboot.img" "%Kernal_output%\KERNAL.SYS" ::/


ENDLOCAL

pause