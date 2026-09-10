using System;
using System.Collections.Generic;
using System.Text;

using Avalonia;

namespace EquipmentBorrowing.Desktop;


internal class program
{
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
