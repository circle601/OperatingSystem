#pragma once
#include <windows.h>
int OpenWindow();
void DrawRectangle(int X, int Y, int width, int height, int Color);
void FillRectangle(int X, int Y, int width, int height, int Color);
void DrawLine(int X, int Y, int X2, int Y2, int Color);
void OpenDisplay();
void ShutDownDisplay();



void DrawImage(HBITMAP DC, int X, int Y, int width, int height);
HDC CreateImageDC();
void FreeDC(HDC DC);
HANDLE LoadFileImage(const char* path);
void FreeImage(HANDLE handle);