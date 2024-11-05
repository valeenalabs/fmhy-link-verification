using System;
using Terminal.Gui;

namespace WikiLinkVerification;

class Program
{
    static void Main(string[] args)
    {
        // Initialize Terminal.Gui
        Application.Init();

        try
        {
            // Create main window
            var mainWindow = new MainWindow();
            
            // Run the application
            Application.Run(mainWindow);
        }
        finally
        {
            // Clean up
            Application.Shutdown();
        }
        
    }   
}