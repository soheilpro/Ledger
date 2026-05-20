using System;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Ledger.Tui.Views
{
    internal class MainWindow : Window
    {
        public MainWindow(ITuiController controller, IContext context)
        {
            BorderStyle = LineStyle.None;

            var menuBar = new MainMenuBar(controller);

            var content = new MainContent(controller, context)
            {
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
            };

            var statusBar = new MainStatusBar(controller);

            Add(menuBar);
            Add(content);
            Add(statusBar);
        }
    }
}
