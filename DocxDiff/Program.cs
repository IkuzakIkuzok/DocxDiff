
// (c) 2023 Kazuki KOHZUKI

using DocxDiff.Forms;

namespace DocxDiff;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new MainWindow());
    } // private static void Main ()
} // internal static class Program