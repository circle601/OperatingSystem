/******************************************************************************
   main.cpp
		-Kernel main program

   modified\ Jan 23 2016
   arthor\ Mike
******************************************************************************/

#include <bootinfo.h>
#include <hal.h>
#include <kybrd.h>
#include <string.h>
#include <flpydsk.h>
#include <fat12.h>
#include <stdio.h>
#include "Display.h"


#include "DebugDisplay.h"
#include "exception.h"
#include "mmngr_phys.h"
#include "mmngr_virtual.h"
#include "task.h"
struct memory_region {
	uint32_t	startLo;
	uint32_t	startHi;
	uint32_t	sizeLo;
	uint32_t	sizeHi;
	uint32_t	type;
	uint32_t	acpi_3_0;
};

uint32_t kernelSize=0;

extern void install_tss (uint32_t idx, uint16_t kernelSS, uint16_t kernelESP);
extern void syscall_init ();

/**
*	Initialization
*/
void init (multiboot_info* bootinfo) {
	DebugClrScr (0x13);
	DebugGotoXY (0,0);
	DebugSetColor (0x17);
	hal_initialize ();
	setvect (0,(void (__cdecl &)(void))divide_by_zero_fault);
	setvect (1,(void (__cdecl &)(void))single_step_trap);
	setvect (2,(void (__cdecl &)(void))nmi_trap);
	setvect (3,(void (__cdecl &)(void))breakpoint_trap);
	setvect (4,(void (__cdecl &)(void))overflow_trap);
	setvect (5,(void (__cdecl &)(void))bounds_check_fault);
	setvect (6,(void (__cdecl &)(void))invalid_opcode_fault);
	setvect (7,(void (__cdecl &)(void))no_device_fault);
	setvect (8,(void (__cdecl &)(void))double_fault_abort);
	setvect (10,(void (__cdecl &)(void))invalid_tss_fault);
	setvect (11,(void (__cdecl &)(void))no_segment_fault);
	setvect (12,(void (__cdecl &)(void))stack_fault);
	setvect (13,(void (__cdecl &)(void))general_protection_fault);
	setvect (14,(void (__cdecl &)(void))page_fault);
	setvect (16,(void (__cdecl &)(void))fpu_fault);
	setvect (17,(void (__cdecl &)(void))alignment_check_fault);
	setvect (18,(void (__cdecl &)(void))machine_check_abort);
	setvect (19,(void (__cdecl &)(void))simd_fpu_fault);
	pmmngr_init ((size_t) bootinfo->m_memorySize, 0xC0000000 + kernelSize*512);
	memory_region*	region = (memory_region*)0x1000;
	for (int i=0; i<10; ++i) {
		if (region[i].type>4)
			break;
		if (i>0 && region[i].startLo==0)
			break;
		pmmngr_init_region (region[i].startLo, region[i].sizeLo);
	}
	pmmngr_deinit_region (0x100000, kernelSize*512);


	


	DebugPrintf("End Of kernal: %x ", 0x100000 + kernelSize * 512);
	pmmngr_deinit_region (0x0, 0x10000);

	vmmngr_initialize ();
	kkybrd_install (33);
	flpydsk_set_working_drive (0);
	flpydsk_install (38);
	fsysFatInitialize ();
	syscall_init ();
	install_tss (5,0x10,0);



	#define PMMNGR_BLOCK_SIZE	4096
	int sise;
	int blocks = 3;
	sise = blocks * PMMNGR_BLOCK_SIZE;
	void* space = pmmngr_alloc_blocks(blocks);
	DebugPrintf("Address: %x Sise: %x", space, sise);

}

/*
===========================

	Chapter 25 New Code 

===========================
*/

/* kernel threads. */
void kthread_1 ();
void kthread_2 ();
void kthread_3 ();
void CompilerThread();
void RunThread();



/* sleep for some time. */
void sleep (int ms) {

	if (ms > 1)
		thread_sleep (ms);
}



/*** Main program *****************************************/

/* we are changing the stack, so we store a local copy. */
static multiboot_info _bootinfo;

/* for chapter 25, we moved the stack into kernel space here. */
__declspec(align(16)) char _kernel_stack[8096];

/* main program. */
void _cdecl kmain (multiboot_info* bootinfo) {

	/* store kernel size and copy bootinfo. */
	_asm mov	word ptr [kernelSize], dx
	memcpy(&_bootinfo, bootinfo, sizeof(multiboot_info));

	/* adjust stack. */
	_asm lea esp, dword ptr [_kernel_stack+8096]
	init (&_bootinfo);

	/* set video mode and map framebuffer. */
//	VbeBochsSetMode(WIDTH,HEIGHT,BPP);
//	VbeBochsMapLFB();
//	fillScreen32 ();

	/* init scheduler. */
	scheduler_initialize ();












	/* create kernel threads. */
//	queue_insert (thread_create(kthread_1, (uint32_t) create_kernel_stack(),true));
//	queue_insert (thread_create(kthread_2, (uint32_t) create_kernel_stack(),true));
//	queue_insert (thread_create(kthread_3, (uint32_t) create_kernel_stack(),true));
	//queue_insert(thread_create(CompilerThread, (uint32_t)create_kernel_stack(), true));
	queue_insert(thread_create(RunThread, (uint32_t)create_kernel_stack(), true));
	/* execute idle thread. */
	execute_idle();

	/* this should never get executed. */
	for (;;) _asm {cli
		hlt};
}

/*** Threads ********************************************/



/* thread cycles through colors of red. */
void kthread_1 () {
	int col = 0;
	bool dir = true;
	while(1) {
		rect32(200,250,100,100,col << 16);
		if (dir) {
			if (col++ == 0xfe)
				dir=false;
		}
		else
			if (col-- == 1)
				dir=true;
		sleep(3);
	}
}

/* thread cycles through colors of green. */
void kthread_2 () {
	int col = 0;
	bool dir = true;
	while(1) {
		rect32(350,250,100,100,col << 8);
		if (dir) {
			if (col++ == 0xfe)
				dir=false;
		}
		else
			if (col-- == 1)
				dir=true;
		sleep(5);
	}
}

/* thread cycles through colors of blue. */
void kthread_3 () {
	int col = 0;
	bool dir = true;
	while(1) {
		rect32(500,250,100,100,col);
		if (dir) {
			if (col++ == 0xfe)
				dir=false;
		}
		else
			if (col-- == 1)
				dir=true;
		sleep(2);
	}
}


void CompilerThread() {
}

void RunThread() {
	sleep(10);
	sleep(1000);
}

