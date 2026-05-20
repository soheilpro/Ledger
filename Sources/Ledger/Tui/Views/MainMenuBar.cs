using System;
using Terminal.Gui.Drivers;
using Terminal.Gui.Input;
using Terminal.Gui.Views;

namespace Ledger.Tui.Views
{
    internal class MainMenuBar : MenuBar
    {
        private readonly ITuiController _controller;

        public MainMenuBar(ITuiController controller)
        {
            _controller = controller;

            var fileMenu = new MenuBarItem("_File", [
                new MenuItem() {
                    Title = "E_xit",
                    Key = new Key(KeyCode.CtrlMask | KeyCode.X),
                    Action = () => _controller.Exit(),
                }
            ]);

            var viewMenu = new MenuBarItem("_View", [
                new MenuItem() {
                    Title = "_Balances",
                    Key = new Key(KeyCode.CtrlMask | KeyCode.B),
                },
                new MenuItem() {
                    Title = "Balance _Check",
                    Key = new Key(KeyCode.CtrlMask | KeyCode.C),
                },
                new MenuItem() {
                    Title = "_Entry Items",
                    Key = new Key(KeyCode.CtrlMask | KeyCode.E),
                },
                new MenuItem() {
                    Title = "_Profit / Loss",
                    Key = new Key(KeyCode.CtrlMask | KeyCode.P),
                },
                new MenuItem() {
                    Title = "_Net Worth",
                    Key = new Key(KeyCode.CtrlMask | KeyCode.N),
                },
            ]);

            var journalMenu = new MenuBarItem("_Journal", [
                new MenuItem() {
                    Title = "_Reload",
                    Key = new Key(KeyCode.CtrlMask | KeyCode.R),
                }
            ]);

            Add(fileMenu);
            Add(viewMenu);
            Add(journalMenu);
        }
    }
}
