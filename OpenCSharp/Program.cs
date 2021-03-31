using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
#if RELEASE
using System;
#endif

namespace OpenCSharp
{
    public class GlobalArgs
    {
        //Not used right now
        static public bool ScreenClear = false;
    }
    class Program
    {

        static void SetParms(string[] args)
        {
            if (args.Length <= 0)
                return;

            for(int i = 0; i < args.Length; i++)
            {
                if(args[i].StartsWith("-clear"))
                {
                    GlobalArgs.ScreenClear = true;
                }
            }

        }

        static void Main(string[] args)
        {

            SetParms(args);
#if RELEASE
            try 
            {
#endif
            var nativeWindowSettings = new NativeWindowSettings()
                {
                    Size = new Vector2i(1024, 576),
                    Title = "Window",
                };

                using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
                {
                    double fps = 60;
                    window.VSync = OpenTK.Windowing.Common.VSyncMode.Off;
                    window.UpdateFrequency = fps;
                    window.RenderFrequency = fps;
                    window.Run();
                }
#if RELEASE
                }
                catch(Exception e)
                {
                    Console.WriteLine("Something go wrong, Error: " + e.Message);
                }
#endif
            }
    }
}
