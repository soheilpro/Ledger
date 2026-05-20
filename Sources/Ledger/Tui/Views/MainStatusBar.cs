using System;
using Terminal.Gui.Views;

namespace Ledger.Tui.Views
{
    internal class MainStatusBar : StatusBar
    {
        public MainStatusBar(ITuiController controller)
        {
            Text = "Ready.";
        }
    }
}
