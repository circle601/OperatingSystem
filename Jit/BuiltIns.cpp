

#include "ByteCode.h"
#include "Memory.h"
#include "Display.h"
#include "ClassFinder.h"

#include <stdio.h>

void PrintDataTextLN(char* String) {
	std::cout << String << "\n";

}

void PrintDataText(char* String) {
	std::cout << String;

}
char* ReadTextLN() {
	char line[1024];

	fgets(line, line[1024], stdin);
	int length = strlen(line) + 1;
	char* stringmemory = (char*)Allocate(length);
	memcpy(stringmemory, line, length);
	return stringmemory;

}


void Printint(int number) {
	std::cout << number;

}

void ThreadSleep(int ms) {
	_sleep(ms);

}


void LoadBuiltins() {
	StartClassStore();
	Obj* Display = AllocatePointerObject(6);
	*(void**)GetItem(Display, 0) = OpenDisplay;  //open display
	*(void**)GetItem(Display, 1) = ShutDownDisplay;  //open display
	*(void**)GetItem(Display, 2) = DrawRectangle;
	*(void**)GetItem(Display, 3) = FillRectangle;
	*(void**)GetItem(Display, 4) = DrawLine;
	*(void**)GetItem(Display, 5) = DrawImage;


	AddJitClass(".display", Display);

	Obj* Console = AllocatePointerObject(5);
	*(void**)GetItem(Console, 0) = PrintDataTextLN;
	*(void**)GetItem(Console, 1) = PrintDataText;
	*(void**)GetItem(Console, 2) = ReadTextLN;
	*(void**)GetItem(Console, 3) = Printint;

	AddJitClass(".console", Console);


	Obj* thread = AllocatePointerObject(5);
	*(void**)GetItem(thread, 0) = ThreadSleep;

	AddJitClass(".thread", thread);



	Obj* GraphicsDevice = AllocatePointerObject(5);
	*(void**)GetItem(GraphicsDevice, 0) = CreateImageDC;
		*(void**)GetItem(GraphicsDevice, 1) = FreeDC;
		*(void**)GetItem(GraphicsDevice,2) = LoadFileImage;
		*(void**)GetItem(GraphicsDevice, 3) = FreeImage;

	AddJitClass(".graphicsdevice", GraphicsDevice);
	

}