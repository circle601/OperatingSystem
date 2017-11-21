
#include <hal.h>
#include <stdio.h>
#include "DebugDisplay.h"
#include "Display.h"

void _cdecl text_kernel_panic(const char* fmt, unsigned int CS, unsigned int EIP, unsigned int Other);
void _cdecl Graphical_kernel_panic(const char* fmt, unsigned int CS, unsigned int EIP, unsigned int Other);

static bool GraphicalEnabled = true;
static char* sickpc = " \
                                    \n\
                  .-\" - .      . - :-.      . - \"-. \n\
                 / RIP \\      / RIP \\      / RIP \\    \n\
                 |     |      |     |      |     |    \n\
                \\\\___|//  \\\\___|//  \\\\___|//  \n\n";

//! something is wrong--bail out
void _cdecl kernel_panic(const char* fmt, unsigned int CS, unsigned int EIP, unsigned int Other) {
	if (GraphicalEnabled) {
		Graphical_kernel_panic(fmt, CS, EIP, Other);
	}
	else {
		text_kernel_panic(fmt, CS, EIP, Other);
	}
	while (true) _asm HLT;

}


void _cdecl text_kernel_panic (const char* fmt,unsigned int CS, unsigned int EIP, unsigned int Other) {

	disable();


	char* disclamer="\n JITOS has encountered a problem and has been shut down\n\
to prevent damage to your computer. Any unsaved work might be lost.\n\
We are sorry for the inconvenience this might have caused.\n\n\
Please report the following information and restart your computer.\n\
The system has been halted.\n\n";

//	DebugClrScr (0x1f);
//	DebugGotoXY (0,0);
	DebugSetColor (0x1f);
//	DebugPuts (sickpc);
	DebugPuts (disclamer);
	DebugPrintf(fmt);
	DebugPrintf(" \n*** STOP \n");
	DebugPrintf("Address [0x%x:0x%x] \n", CS, EIP);
	DebugPrintf("Error Data [0x%x] \n", Other);
	DebugPrintf("Halting");

}


void _cdecl Graphical_kernel_panic(const char* fmt, unsigned int CS, unsigned int EIP, unsigned int Other) {
	disable();
	GraphicalEnabled = false;
	VbeBochsMapLFB();
///	fillScreen32();

}