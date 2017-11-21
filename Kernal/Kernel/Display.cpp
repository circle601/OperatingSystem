#include "Display.h"
#include <string.h>
#include <stdio.h>
#include <hal.h>
#include "mmngr_virtual.h"





/*** VESA Support ****************************************/







/*** BGA Support ****************************************/


/* We'll be needing this. */
void outportw(unsigned short portid, unsigned short value) {
	_asm {
		mov		ax, word ptr[value]
		mov		dx, word ptr[portid]
		out		dx, ax
	}
}

/* Definitions for BGA. Reference Graphics 1. */
#define VBE_DISPI_IOPORT_INDEX          0x01CE
#define VBE_DISPI_IOPORT_DATA           0x01CF
#define VBE_DISPI_INDEX_XRES            0x1
#define VBE_DISPI_INDEX_YRES            0x2
#define VBE_DISPI_INDEX_BPP             0x3
#define VBE_DISPI_INDEX_ENABLE          0x4
#define VBE_DISPI_DISABLED              0x00
#define VBE_DISPI_ENABLED               0x01
#define VBE_DISPI_LFB_ENABLED           0x40

/* writes to BGA port. */
void VbeBochsWrite(uint16_t index, uint16_t value) {
	outportw(VBE_DISPI_IOPORT_INDEX, index);
	outportw(VBE_DISPI_IOPORT_DATA, value);
}

/* sets video mode. */
void VbeBochsSetMode(uint16_t xres, uint16_t yres, uint16_t bpp) {
	VbeBochsWrite(VBE_DISPI_INDEX_ENABLE, VBE_DISPI_DISABLED);
	VbeBochsWrite(VBE_DISPI_INDEX_XRES, xres);
	VbeBochsWrite(VBE_DISPI_INDEX_YRES, yres);
	VbeBochsWrite(VBE_DISPI_INDEX_BPP, bpp);
	VbeBochsWrite(VBE_DISPI_INDEX_ENABLE, VBE_DISPI_ENABLED | VBE_DISPI_LFB_ENABLED);
}

/* video mode info. */
#define WIDTH           800
#define HEIGHT          600
#define BPP             32
#define BYTES_PER_PIXEL 4

/* BGA LFB is at LFB_PHYSICAL. */
#define LFB_PHYSICAL 0xE0000000
#define LFB_VIRTUAL  0x300000
#define PAGE_SIZE    0x1000

/* map LFB into current process address space. */
void* VbeBochsMapLFB() {
	int pfcount = WIDTH*HEIGHT*BYTES_PER_PIXEL / PAGE_SIZE;
	for (int c = 0; c <= pfcount; c++)
		vmmngr_mapPhysicalAddress(vmmngr_get_directory(), LFB_VIRTUAL + c * PAGE_SIZE, LFB_PHYSICAL + c * PAGE_SIZE, 7);
	return (void*)LFB_VIRTUAL;
}

/* clear screen to white. */
void fillScreenWhite() {
	uint32_t* lfb = (uint32_t*)LFB_VIRTUAL;
	for (uint32_t c = 0; c<WIDTH*HEIGHT; c++)
		lfb[c] = 0xffffffff;
}


/* render rectangle in 32 bpp modes. */
void rect32(int x, int y, int w, int h, int col) {
	uint32_t* lfb = (uint32_t*)LFB_VIRTUAL;
	for (uint32_t k = 0; k < h; k++)
		for (uint32_t j = 0; j < w; j++)
			lfb[(j + x) + (k + y) * WIDTH] = col;
}