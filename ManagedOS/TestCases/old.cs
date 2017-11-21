
static class Main
{

    static int a()
    {
        Console.PrintLine("hello");
        return 1;
    }
}



public static extern class Display
{
    public static extern void OpenDisplay();
    public static extern void ShutDownDisplay();
    public static extern void DrawRectangle(int X, int Y, int width, int height, int Color);
    public static extern void FillRectangle(int X, int Y, int width, int height, int Color);
    public static extern void DrawLine(int X, int Y, int X2, int Y2, int Color);
    public static extern void DrawImage(DeviceContext DC, int X, int Y, int width, int height);

}

public extern class DeviceContext
{

}

public static extern class GraphicsDevice
{
    public static extern DeviceContext CreateImageDC();
    public static extern void FreeDC(DeviceContext DC);
    public static extern DeviceContext LoadFileImage(string path);
    public static extern void FreeImage(DeviceContext handle);
}



public static extern class Console
{
    public static extern void PrintLine(string text);
    public static extern void Print(string text);
    public static extern string ReadLine();
    public static extern void Printint(int number);
}



public static extern class Thread
{
    public static extern void Sleep(int ms);
}

public class Bitmap
{
    DeviceContext Image;
    public DeviceContext GetDC()
    {
        return Image;
    }
    public void LoadImage(string Part)
    {
        Image = GraphicsDevice.LoadFileImage(Part);

    }
}


public extern class IEnumerable
{

}