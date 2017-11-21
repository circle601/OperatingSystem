
;*******************************************************
;
;	Stage2.asm
;		Stage2 Bootloader
;
;	OS Development Series
;*******************************************************

%ifndef __ConfigChooser_INCLUDED__
%define __ConfigChooser_INCLUDED__

Title     db "JITOS Bootloader", 0x00
ChooserDescription     db "Please choose an operating system to boot from.", 0x00
Chooserselection  db "Press enter To make a Selection.", 0x00
LastSelection     db 0x00
CurrentSelection     db 0x00
CurrentSelectionIndicator     db ">", 0x00
clearSelection     db " ", 0x00
ConfigChooser:
	pusha
	call ClearScreen
	call Blockline16
	mov ah,02h
	mov BH,0
	mov DH ,24
	mov DL ,0
	int 10H
	call Blockline16

	mov ah,02h
	mov BH,0
	mov DH ,3
	mov DL ,0
	int 10H

	
	mov		si, Title
	call   		Puts16

	mov ah,02h
	mov BH,0
	mov DH ,5
	mov DL ,0
	int 10H

	mov		si, ChooserDescription
	call   		Puts16

		mov ah,02h
	mov BH,0
	mov DH ,18
	mov DL ,0
	int 10H

	mov		si, Chooserselection
	call   		Puts16



	xor ecx,ecx
	ChooserDisplayName:

	mov ah,02h
	mov BH,0
	mov DH ,10
	mov DL ,5
	add DH,cl
	int 10H  ; set position

	xor ebx,ebx
	mov bl,cl
	shl bx,8
	add bx,Extra_data

	mov		si, bx
	call   		Puts16 ;display name
	inc cl
	cmp cl,[ListSectionID]
	JB  ChooserDisplayName ;  more names to display
	ChooserDisplayNameEnd:


	call displayCurrentChooser
	cmenu:
	
	mov		ah, 0
	int     	0x16                    ; await keypress
	; AH = BIOS scan code
	cmp ah,0x48
	je Cup
	cmp ah,0x50
	je Cdown
	cmp ah,01ch
	jne cmenu; 
	jmp cloopexit

	Cup:
	mov al,[CurrentSelection]
	or al,al
	jz cmenu
	dec al
	mov [CurrentSelection],al
	call displayCurrentChooser
	jmp cmenu

	Cdown:
	mov al,[CurrentSelection]
	inc al
	cmp al,[ListSectionID]
	je cmenu
	mov [CurrentSelection],al
	call displayCurrentChooser

	jmp cmenu

	cloopexit:
	call ClearScreen


	


	popa
	ret

	


	displayCurrentChooser:
	pusha
	xor edx,edx
	xor eax,eax
	xor ebx,eax

	mov ah,02h
	mov BH,0
	mov DH ,10
	mov DL ,4
	add DH,[LastSelection]
	int 10H  ; set position
	mov		si, clearSelection
	call   	Puts16 ;display name


	mov ah,02h
	mov BH,0
	mov DH ,10
	mov DL ,4
	add DH,[CurrentSelection]
	int 10H  ; set position
	mov		si, CurrentSelectionIndicator
	call   	Puts16 ;display name

	mov al,[CurrentSelection]
	mov [LastSelection],al



	popa
	ret

%endif