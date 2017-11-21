asm(".code16gcc\n");
void putchar(char);
int start()
{
	putchar('Q');
	while (1) {
		asm("cli");
		asm("hlt");
	}
	return 0;
}
void inline putchar(char val)
{
	asm("mov al ,val \n"
		"mov ah ,14 \n"
		"int $0x10\n"
		);


}