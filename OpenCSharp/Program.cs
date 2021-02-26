using Engine;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using GlmNet;
namespace OpenCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
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
        }
    }
}
