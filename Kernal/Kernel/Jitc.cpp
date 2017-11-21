
#define _KERNAL

#include "Jitc.h";
#include "JitThread.h";
#include "loader.h";
#include "ClassFinder.h"
 void PrintError(const char* errorcode) {
	 DebugPrintf(errorcode);
}

static void ReadFiletoBuffer(FILE* file, char* buffer) {
	int length = file->fileLength;
	unsigned char buf[512];
	int Tocopy = 0;
	int position = 0;
	while (!file->eof && length > 0) {
		volReadFile(file, buf, 512);

		if (length > 512) {
			Tocopy = 512;
		}
		else {
			Tocopy = length;
		}
		memcpy(buffer + position, &buf, Tocopy);
		length -= 512;
		position += 512;
	}

}






PackageProgram* LoadProgram(const char* path) {

	int length;
	char* buffer;
	DebugPrintf("Loading Program \n");

	FILE file = volOpenFile(path);
	//! test for invalid file
	if (file.flags == FS_INVALID) {
		DebugPrintf("Not a file");
		return NULL;
	}

	if ((file.flags & FS_DIRECTORY) == FS_DIRECTORY) {
		DebugPrintf("Not a file");
		return NULL;
	}
	 

		// get length of file:
		length = file.fileLength;

		buffer = (char*)Allocate(length);
		if (buffer == NULL) {
			DebugPrintf("Unable to allocate memory");
			return NULL;
		}
		DebugPrintf("Reading file\n");

		ReadFiletoBuffer(&file, buffer);

		DebugPrintf("Loading program \n");

		PackageProgram* result = LoadProgramFile(buffer, length);
		DebugPrintf("Freeing Buffer \n");
		Free(buffer);
		return result;
	
}






void StartMemoryAllocator(void* start,size_t length) {
	StartAllocator(start, length);
	Obj* Startup = AllocateObject(3000);
	if (Startup == NULL) {
		DebugPrintf("Unable to Start Memory Allocator");
	}

	DebugPrintf("Creating space ");

	Startup->Baseclass = 0;
	Startup->CallMin = 0;
	Startup->CallMax = 3;
	CallTableElement* Space = GetCallElementPtr(Startup, 0);
	CallTableElement* Start = GetCallElementPtr(Startup, 1);
	CallTableElement* Result = GetCallElementPtr(Startup, 2);
	Space->type = CallTableSystemReserved;
	Space->ofset = 0;

	Start->ofset = 1000;
	Start->type = 1;

	DebugPrintf(" Staring Store");


	SetCompilerWorkingMemory((char*)GetItemPtr(Startup, Space), 1000);

	DebugPrintf("readying Compiler thread");
	//InitJitD();
	StartClassStore();
	


	DebugPrintf("Jit ready");
}











void StartJitThread() {
	CompilerThreadRun();
}



void StartProgram(const char* path) {
	PackageProgram* program = LoadProgram(path);
//	PackageProgram* program;
	if (program == NULL) {
		DebugPrintf("Unable to load Program! \n");
		return;
	}
	DebugPrintf("Starting Compiler \n");
	SetPackage(program);
	//todo use startpoint

	DebugPrintf("Compiling \n");
	void* ptr = GetExecuteAddr(program->classes[0], 0);
	DebugPrintf("Running \n");
	if (ptr != NULL) {
	int result = ((MyFunc)ptr)();
	DebugPrintf("result: %i", result);
	}
	else {
		DebugPrintf("Program not run \n");
	}
}