/* The bootloader will look at this image and start execution at the symbol
   designated as the entry point. */
ENTRY(_kernel_entry)

OUTPUT(Stage3.exe)
/* Tell where the various sections of the object files will be put in the final
   kernel image. */

SECTIONS
{
  . = 0x10000;
  .text :{ *(.entry .text .data .rdata) }
  .bss 0x3000 :{ *( .bss .sbss )}
}