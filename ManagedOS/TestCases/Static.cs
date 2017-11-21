
static class Main
{
   
    static int a(){
        TestClass.SetQ(12);
        return TestClass.GetQ() + 1;
    }
}

public static class TestClass
{
    static int Q = 94;
    public static int A()
    {
        return 5;
    }
    public static void SetQ(int newQ)
    {
        Q = newQ;
    }
    public static int GetQ()
    {
        return Q;
    }
    public static int B()
    {
        return 1;
    }
}


static class OLdmain
{

    static int a()
    {
        Display.OpenDisplay();
        Thread.Sleep(100);
        Bitmap bmp = new Bitmap();
        bmp.LoadImage("D:\OSBackGround\yuri.bmp");

        Display.DrawImage(bmp.GetDC(), 0, 0, 800, 600);

        Display.FillRectangle(0, 180, 320, 200, 0);
        Display.FillRectangle(0, 180, 20, 200, 0);
        return 1;
    }
}

