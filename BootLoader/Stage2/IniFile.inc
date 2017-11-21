
;*******************************************************
;
;	Stage2.asm
;		Stage2 Bootloader
;
;	OS Development Series
;*******************************************************

%ifndef __INIFILE_INCLUDED__
%define __INIFILE_INCLUDED__

[BITS 16]

INIWorkingMemory times 16 db 0

INISegmentStart     db "["
INISegmentEnd     db "]"
iniError     db "Invalid ini file", 0x0D, 0x0A, 0x0A, 0x00
iniLineError     db "Invalid ini Line", 0x0D, 0x0A, 0x0A, 0x00
iniDone     db "Done ini file", 0x0D, 0x0A, 0x0A, 0x00
initStar     db "*", 0x00
DotStar     db ".", 0x00
ListSectionID db 0

INIpath     db "path", 0x00
INIHal      db "hal", 0x00
INIStart	db "Start", 0x00


;___________________________________________________________________________________
ReadIniLine:

push ebx
push eax
push edx

ReadIniLineStart:
xor ebx,ebx
.Loop2:
		lodsb				; load next byte from string from SI to AL
		or	al, al			; Does AL=0?
		jz	ReadIniLineexit		; Yep, null terminator found-bail out
		cmp al,0Ah
		je ReadIniLineexit ; Yep, \Newline found ,bail out
		cmp al,"="
		je IniLineDone ; Yep, "=" found ,exit loop
		mov [INIWorkingMemory + bx],al
		inc bx
		jmp	.Loop2			; Repeat until null terminator or = is found
IniLineDone:
;  "=" has been found

xor ebx,ebx
;only checks first letter acually
mov al, [INIWorkingMemory]
mov bl,16
cmp al,[INIpath]
je FoundiniTarget
mov bl,32
cmp bl,[INIHal]
je FoundiniTarget
mov bl,48
cmp al,[INIStart]
je FoundiniTarget

mov bl,64
FoundiniTarget:


mov edx,[ListSectionID]
shl edx,8
add ebx,edx

.Loop3:
		lodsb				; load next byte from string from SI to AL
		or	al, al			; Does AL=0?
		jz	ReadIniLineexit		; Yep, null terminator found-bail out
		cmp al,0Ah
		je IniLineDone2 ; Yep, \Newline found ,bail out
		mov [Extra_data + bx],al
		inc bx
		jmp	.Loop3			; Repeat until null terminator or = is found
IniLineDone2:








mov dx,si
mov		si, initStar
call   		Puts16
mov si,dx



lodsb ;check if new line
dec si
cmp al,[INISegmentStart]
jne ReadIniLineStart

ReadIniLineexit:
pop edx
pop eax
pop ebx


ret

ReadIniLineerror:
mov		si, iniLineError
call   		Puts16
jmp ReadIniLineexit


;___________________________________________________________________________________

ReadIniSection:
pusha
cld
ReadIniSectionStart:

mov dx,si
mov		si, DotStar
call   		Puts16
mov si,dx

lodsb				; load next byte from string from SI to AL
cmp al,[INISegmentStart]
jne ReadIniSectionerror
xor ebx,ebx
mov ebx,[ListSectionID]
shl ebx,8
.Loop1:
		lodsb				; load next byte from string from SI to AL
		or	al, al			; Does AL=0?
		jz	ReadIniSectionerror		; Yep, null terminator found-bail out
		cmp al,[INISegmentEnd]  
		je IniNameDone ; Yep, [ found ,bail out
		mov [Extra_data + bx],al 
		inc bx
		jmp	.Loop1			; Repeat until null terminator found
IniNameDone:

lodsb				;Ignore Newline


call ReadIniLine

mov al,[ListSectionID]
inc al
mov [ListSectionID],al




jmp  ReadIniSectionStart

ReadIniSectionexit:
popa
ret

ReadIniSectionerror:
mov		si, iniError
call   		Puts16
jmp ReadIniSectionexit


%endif