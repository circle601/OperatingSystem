// Jit.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "Compiler.h"
#include "ByteCode.h"
#include "Memory.h"
#include "loader.h"

#include "JitThread.h"
#include "targetver.h"
#include <stdio.h>
#include <tchar.h>
#include <fstream>      
#include <iostream>
#include <string>

#include <iostream>
#include <string>
#include  <iomanip>
#include  <thread>
#include  "BuiltIns.h"

#include <malloc.h>
#include <assert.h>
#include "Display.h"



using namespace std;
#ifndef _UNIT

static void run();



//char Sourceprogram[] = { /* createvarabe */ 0x50,3,CallTableInt,0,0,0, /* createvarabe */ 0x50,1,CallTableInt,0,0,0,  /* Constant */ 0x24,1,1,0,0,0,/* Constant */ 0x24,3,7,0,0,0, /* add*/0x72,1,3,3,   /* return*/ 0x21 , 3,0};
//char Sourceprogram[] = {/*scope*/ 0x30 ,/*var*/ 0x50 , 0x0 , 0xC , 0x0 , 0x0 , 0x0 ,/*constant*/ 0x24 , 0x0 , 0x5 , 0x0 , 0x0 , 0x0 ,/*var*/ 0x50 , 0x1 , 0xC , 0x0 , 0x0 , 0x0 ,/*constant*/ 0x24 , 0x1 , 0x3 , 0x0 , 0x0 , 0x0 ,/*var*/ 0x50 , 0x2 , 0xC , 0x0 , 0x0 , 0x0 ,/*add*/ 0x72 , 0x1 , 0x0 , 0x2 ,/*return*/ 0x21 , 0x2 ,/*scope*/ 0x32 };
//char Sourceprogram[] = { /*scope*/0x30,/*var*/ 0x50, 0x0, 0xC, 0x0, 0x0, 0x0,/*constant*/ 0x24, 0x0, 0x5, 0x0, 0x0, 0x0,/*scope*/ 0x30,/*var*/  0x50, 0x1, 0xC, 0x0, 0x0, 0x0, /*ConditionalToEnd*/0x41, 0x1,/*var*/ 0x50, 0x2, 0xC, 0x0, 0x0, 0x0,/*constant*/ 0x24, 0x2, 0x1, 0x0, 0x0, 0x0,/*return*/ 0x21, 0x2,/*scope*/ 0x32,/*var*/ 0x50, 0x1, 0xC, 0x0, 0x0, 0x0,/*constant*/ 0x24, 0x1, 0x2, 0x0, 0x0, 0x0, 0x21, 0x1, 0x32 };
//char Sourceprogram[] = { 0x0, /*scope*/0x30 ,/*var*/ 0x50 , 0x0 , 0xC , 0x0 , 0x0 , 0x0 ,/*constant*/ 0x24 , 0x0 , 0x2 , 0x0 , 0x0 , 0x0 ,/*scope*/ 0x30 ,/*var*/ 0x50 , 0x1 , 0xC , 0x0 , 0x0 , 0x0 ,/*var*/ 0x50 , 0x2 , 0xC , 0x0 , 0x0 , 0x0 ,/*constant*/ 0x24 , 0x2 , 0x5 , 0x0 , 0x0 , 0x0 ,/*equals*/ 0x92 , 0x0 , 0x2 , 0x1 , 0x41 , 0x1 , 0x50 , 0x3 , 0xC , 0x0 , 0x0 , 0x0 , 0x24 , 0x3 , 0x1 , 0x0 , 0x0 , 0x0 , 0x21 , 0x3 , 0x32 , 0x50 , 0x1 , 0xC , 0x0 , 0x0 , 0x0 , 0x24 , 0x1 , 0x2 , 0x0 , 0x0 , 0x0 , 0x21 , 0x1 , 0x32 };
//char Sourceprogram[] = { 0x20 };
char Sourceprogram[] = { /* createvarabe */ 0x50,1,CallTableInt,0,0,0,/* Constant */ 0x24,1,5,0,0,0, /*print*/ 0x12,1,   /* return*/ 0x21 , 1,0};

void  copyProgram(char* dest,char* source,int lenght) {
	BytecodeProgram* program = (BytecodeProgram*)dest;
	program->ReturnType = CallTableInt;
	program->Compiled = 0;
	program->length = lenght;
	char* data = &program->data;
	for (size_t i = 0; i < lenght; i++)
	{
		data[i] = source[i];
	}
}


#ifdef NOTKERNAL
#else

PackageProgram* LoadProgramFile(const char* path) {
	int length;
	char* buffer;
	std::ifstream is(path, std::ifstream::binary);
	if (is) {
		// get length of file:
		is.seekg(0, is.end);
		length = is.tellg();
		is.seekg(0, is.beg);

		buffer = (char*)malloc(length);
		if (buffer == NULL) {
		
			return NULL;
		}
		// read data as a block:


		is.read(buffer, length);
		Fullcheck();
		if (is) {
			
		}
		else {
			return false;
		}
		is.close();
		PackageProgram* result = LoadProgramFile(buffer, length);
		free(buffer);
		return result;
	}
	else {
		return NULL;
	}
}
#endif

int main()
{
	//Init
	 cout << "Starting\n";
	 size_t memoryLength = 255 * 255;
	 void* memory = malloc(memoryLength);
	 StartAllocator(memory, memoryLength);
	 Obj* Startup = AllocateObject(3000);

	 Startup->Baseclass = 0;
	 Startup->CallMin = 0;
	 Startup->CallMax = 3;
	 CallTableElement* Space = GetCallElementPtr(Startup, 0);
	 CallTableElement* Start = GetCallElementPtr(Startup, 1);
	 CallTableElement* Result = GetCallElementPtr(Startup, 2);
	 Space->type = CallTableSystemReserved;
	 Space->ofset = 0;

	 Result->ofset = 2000;
	 Result->type = CallTableProgram;

	 //char* ProgramAddr = (char*)GetItemPtr(Startup, Start);
	// copyProgram(ProgramAddr, Sourceprogram, sizeof(Sourceprogram) / sizeof(char));
	 SetCompilerWorkingMemory((char*)GetItemPtr(Startup, Space), 1000);



	 Start->ofset = 1000;
	 Start->type = 1;

	 InitJitD();
	 LoadBuiltins();


	std::thread first(CompilerThreadRun);
	_sleep(100);
	cout << "loading program\n";
	 //InitDone



	

	//std::thread runthread(run);
	run();

	
	
	

	cout << "\nPress Any key To exit";
	cin.get();

	//runthread.join();



	//Shutdown
	ShutDownDisplay();

	StopCompilerThread();
	first.join();
	CloseJitD();

	FreeObject(Startup);
	StopAllocator();
	free(memory);





    return 0;
}


static void run() {

	PackageProgram* program = LoadProgramFile("D:\\ouput.erf");
	if (program == NULL) {
		cout << "Unable to load Program! \n";
		cout << "\nPress Any key To exit";
		cin.get();
		return ;
	}


	SetPackage(program);

	for (size_t i = 0; i < 1; i++)
	{
		void* ptr = GetExecuteAddr(program->classes[0], 0);
		if (ptr != NULL) {
			int result = ((MyFunc)ptr)();
			cout.unsetf(ios::hex);
			cout << "\nResult:" << (int)result << " (0x" << hex << result;
			cout << ")\n";
		}
		else {
			cout << "Code Not Run.\n";
			if (ptr != NULL) {
				for (int i = 0; i < ResultLength(); i++) {
					cout << setfill('0') << setw(2) << hex << (unsigned int)(unsigned char)((char*)ptr)[i];
					cout << " ";
				}

				cout.unsetf(ios::hex);
			}
			cout << "\n";
		}
	}



}



#endif // !_UNIT