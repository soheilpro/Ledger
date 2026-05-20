using System;
using Ledger.Tui.Views;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;

namespace Ledger.Tui
{
    internal class TuiController : ITuiController
    {
        private IContext _context;

        private IApplication _app;

        public TuiController(IContext context)
        {
            _context = context;
        }

        public void Run()
        {
            // ConfigurationManager.Enable(ConfigLocations.All);
            // ThemeManager.Theme = "TurboPascal 5";

            _app = Application.Create();
            _app.Init();

            using var mainWindow = new MainWindow(this, _context)
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill(1),
                Height = Dim.Fill(1),
            };

            _app.Run(mainWindow);

            _app.Dispose();
        }

        public void Exit()
        {
            _app.RequestStop();
        }
    }
}
