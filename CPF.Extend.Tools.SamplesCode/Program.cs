using System;
using CPF.Platform;
using CPF.Windows;
#if !Net4
using CPF.Skia;

#endif

namespace CPF.Extend.Tools.SamplesCode
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.Initialize(
#if Net4
               (OperatingSystemType.Windows, new WindowsPlatform(), new CPF.GDIPlus.GDIPlusDrawingFactory())
#else
                (OperatingSystemType.Windows, new WindowsPlatform(), new SkiaDrawingFactory()),
                (OperatingSystemType.OSX, new CPF.Mac.MacPlatform(), new SkiaDrawingFactory()),//如果需要支持Mac才需要
                (OperatingSystemType.Linux, new CPF.Linux.LinuxPlatform(), new SkiaDrawingFactory())//如果需要支持Linux才需要
#endif
            );
            Application.Run(new Window1());
        }
    }
}
