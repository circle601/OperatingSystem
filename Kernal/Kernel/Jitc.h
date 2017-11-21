#pragma once

#include <JitInterface.h>
#include <Compiler.h>
#include <string.h>
#include <flpydsk.h>
#include <fat12.h>
#include <stdio.h>
#include <Memory.h>
#include <Loader.h>
#include <DebugDisplay.h>



void StartMemoryAllocator(void* start, size_t length);
void StartProgram(const char* path);
void StartJitThread();