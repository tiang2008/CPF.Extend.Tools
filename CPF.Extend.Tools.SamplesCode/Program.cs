using System;
using CPF.Platform;
using CPF.Windows;
#if !Net4
using CPF.Skia;

#endif

namespace CPF.Extend.Tools.SamplesCode
{
    /// <summary>
    /// 应用程序的入口点。
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// 主函数
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        static void Main(string[] args)
        {
            // 初始化应用程序
            Application.Initialize(
#if Net4
               (OperatingSystemType.Windows, new WindowsPlatform(), new CPF.GDIPlus.GDIPlusDrawingFactory())
#else
            (OperatingSystemType.Windows, new WindowsPlatform(), new SkiaDrawingFactory()),
            (OperatingSystemType.OSX, new CPF.Mac.MacPlatform(), new SkiaDrawingFactory()),//如果需要支持Mac才需要
            (OperatingSystemType.Linux, new CPF.Linux.LinuxPlatform(), new SkiaDrawingFactory())//如果需要支持Linux才需要
#endif
            );
            // 运行主窗口
            Application.Run(new MainWindow());
        }
    }
}
