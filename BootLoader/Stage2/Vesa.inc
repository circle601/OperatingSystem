
;*******************************************************
;
;	Stage2.asm
;		Stage2 Bootloader
;
;	OS Development Series
;*******************************************************

%ifndef __VESA_INC_284569_INCLUDED__
%define __VESA_INC_284569_INCLUDED__
[BITS 32]

VesaStartingMsg db 0x0D, 0x0A, "Starting Vesa", 0x00
VesaDoneMsg db 0x0D, 0x0A, "VESA BIOS found", 0x00

vesa_info_block:
    signure:			db "VBE2"		;			
    version:		dw 0			;	
    oem	:			dd 0  			;	
    capabilities:		dd 0			;	
    video_modes:		dd 0			;	
    video_memory:		dw 0			;
    software_rev:		dw 0			;
    vendor:				dd 0			;
    product_name:		dd 0			;
    product_rev:		dd 0			;	
    reserved:			times 222 db 0	;	
    oem_da:			times 256 db 0	;


[BITS 16]

check_vbe:
	push es
	mov ax, 0x4F00					; get VBE controller info
	mov di, vesa_info_block
	int 0x10
	pop es

	cmp ax, 0x4F
	jne .error

	cmp word[signure], 0x200	; we need VESA 2.0 or better for the drivers
	jl .error

	mov		si, VesaDoneMsg
	call   		Puts16
	ret

.error:
	mov		si, msgContinue
	call   		Puts16

	ret

.error_msg			db "VESA BIOS version >2.0 not found",0






%endif