#include "stdafx.h"
#include "Display.h"

#include <thread>



const char g_szClassName[] = "myWindowClass";




std::thread* windowThread;
HDC winddowDc;
HDC DisplayDC;
HBITMAP Display;
HWND hwnd;
int DisplayWidth = 320;
int DisplayHeight = 200;

LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	static int Width, Height;
	switch (msg)
	{
	case WM_CREATE: {
		winddowDc = GetWindowDC(hwnd);
		DisplayDC = CreateCompatibleDC(winddowDc);
		Display = CreateCompatibleBitmap(winddowDc, DisplayWidth, DisplayHeight);
		SelectObject(DisplayDC, Display);
	}
	case WM_SIZE: {
		Width = LOWORD(lParam);
		Height = HIWORD(lParam);
		InvalidateRect(hwnd, NULL, TRUE);
		break;
	}
	case WM_APP: {
		int Command;
		switch (wParam)
		{
		case 1:
			{}
			break;
		default:
			break;
		}
		break;
	}
	case WM_PAINT:
	{
		PAINTSTRUCT ps;
		HDC hdc = BeginPaint(hwnd, &ps);

		SetStretchBltMode(hdc, STRETCH_DELETESCANS);

		HDC winddowDc = GetWindowDC(hwnd);
		StretchBlt(hdc, 0, 0, Width, Height, DisplayDC, 0, 0, DisplayWidth, DisplayHeight, SRCCOPY);
	//	BitBlt(hdc, 0, 0, Width, Height, DisplayDC, 0, 0, SRCCOPY);
		// Paint the game
		

		EndPaint(hwnd, &ps);

		break;
	}
	case WM_CLOSE:
		DeleteDC(DisplayDC);
		DisplayDC = NULL;
		DestroyWindow(hwnd);
		break;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	default:
		return DefWindowProc(hwnd, msg, wParam, lParam);
	}
	return 0;
}

int OpenWindow()
{
	WNDCLASSEX wc;
	
	MSG Msg;
	HMODULE hInstance  = GetModuleHandle(NULL);
	//Step 1: Registering the Window Class
	wc.cbSize = sizeof(WNDCLASSEX);
	wc.style = CS_HREDRAW | CS_VREDRAW;
	wc.lpfnWndProc = WndProc;
	wc.cbClsExtra = 0;
	wc.cbWndExtra = 0;
	wc.hInstance = hInstance;
	wc.hIcon = LoadIcon(NULL, IDI_APPLICATION);
	wc.hCursor = LoadCursor(NULL, IDC_ARROW);
	wc.hbrBackground = (HBRUSH)GetStockObject(GRAY_BRUSH);
	wc.lpszMenuName = NULL;
	wc.lpszClassName = g_szClassName;
	wc.hIconSm = LoadIcon(NULL, IDI_APPLICATION);

	if (!RegisterClassEx(&wc))
	{
		MessageBox(NULL, "Window Registration Failed!", "Error!",
			MB_ICONEXCLAMATION | MB_OK);
		return 0;
	}



	

	// Step 2: Creating the Window
	hwnd = CreateWindowEx(
		WS_EX_CLIENTEDGE,
		g_szClassName,
		"Display",
		WS_OVERLAPPEDWINDOW,
		CW_USEDEFAULT, CW_USEDEFAULT, 800, 600,
		NULL, NULL, hInstance, NULL);

	if (hwnd == NULL)
	{
		MessageBox(NULL, "Window Creation Failed!", "Error!",
			MB_ICONEXCLAMATION | MB_OK);
		return 0;
	}

	ShowWindow(hwnd, SW_SHOWNORMAL);
	UpdateWindow(hwnd);

	

	// Step 3: The Message Loop
	while (GetMessage(&Msg, NULL, 0, 0) > 0)
	{
		TranslateMessage(&Msg);
		DispatchMessage(&Msg);
	}


	return Msg.wParam;
}

void Invalidate() {
	InvalidateRect(hwnd, NULL, TRUE);

}

void DrawRectangle(int X, int Y,int width,int height,int Color) {
Rectangle(DisplayDC, X, Y, width, height);
	Invalidate();
	
}

void FillRectangle(int X, int Y, int width, int height, int Color) {
	
	//SetDCBrushColor(DisplayDC, Color);
	Rectangle(DisplayDC, X, Y, width, height);
	Invalidate();
}

void DrawLine(int X, int Y, int X2, int Y2, int Color) {
	
	MoveToEx(DisplayDC, X, Y, NULL);
	LineTo(DisplayDC, X2, Y2);
	Invalidate();
}

void OpenDisplay() {
	windowThread = new 	std::thread(OpenWindow);
}

void ShutDownDisplay() {
	if (windowThread != 0) {
		windowThread->join();
		delete(windowThread);
	}
}

void DrawImage(HBITMAP hBitmap, int X, int Y, int width, int height ) {

	HDC hdcMem = CreateCompatibleDC(winddowDc);
	HANDLE oldBitmap = SelectObject(hdcMem, hBitmap);
	BitBlt(DisplayDC, X, Y, width, height, hdcMem, 0, 0, SRCCOPY);

	SelectObject(hdcMem, oldBitmap);
	DeleteDC(hdcMem);

	Invalidate();
}



HDC CreateImageDC() {
	return CreateCompatibleDC(winddowDc);
}
void FreeDC(HDC DC) {
	DeleteDC(DC);
}
HANDLE LoadFileImage(const char* path){
	HANDLE image = LoadImage(NULL, path, IMAGE_BITMAP,0,0, LR_LOADFROMFILE ) ;
	return image;
}

void FreeImage(HANDLE handle) {
	DeleteObject(handle);
}