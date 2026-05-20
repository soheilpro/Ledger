using System;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Ledger.Tui.Views
{
    internal class MainContent : FrameView
    {
        public MainContent(ITuiController controller, IContext context)
        {
            BorderStyle = LineStyle.None;

            var x = new BalancesReportView(controller, context)
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };

            Add(x);
        }
    }
}
