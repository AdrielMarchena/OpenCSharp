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
                window.Run();
            }
        }
    }
}
