/******************************************************************************
   main.cpp
		-Process

   modified\ Oct 10 2010
   arthor\ Mike
******************************************************************************/

#include <Compiler.h>
#include <ByteCode.h>
#include <Memory.h>
#include <loader.h>
/**
* Process entry point
*/
void processEntry () {

	char* str="\n\rHello world!";

	__asm {

		/* display message through kernel terminal */
		mov ebx, str
		mov eax, 0
		int 0x80

		/* terminate */
		mov eax, 1
		int 0x80
	}
	for (;;);
}
