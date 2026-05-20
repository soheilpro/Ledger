using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Ledger.Reports;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Ledger.Tui.Views
{
    internal class BalancesReportView : FrameView
    {
        public BalancesReportView(ITuiController controller, IContext context)
        {
            BorderStyle = LineStyle.None;

            var headerLabel = new Label()
            {
                X = 1,
                Y = 1,
                Title = "Balances",
            };

            var descriptionLabel = new Label()
            {
                X = 1,
                Y = Pos.Bottom(headerLabel),
                Title = "View balances.",
            };

            var amountColumnStyle = new ColumnStyle()
            {
                Alignment = Alignment.End,
                Format = " #,##0.## ",
            };

            var tableView = new TableView()
            {
                X = 0,
                Y = Pos.Bottom(descriptionLabel) + 1,
                Width = Dim.Fill(0),
                Height = Dim.Fill(1),
                Style = new TableStyle()
                {
                    AlwaysShowHeaders = true,
                    ShowHorizontalBottomLine = true,
                    ColumnStyles = new Dictionary<int, ColumnStyle>()
                    {
                        [2] = amountColumnStyle,
                        [3] = amountColumnStyle,
                        [4] = amountColumnStyle,
                        [5] = amountColumnStyle,
                    },
                },
                FullRowSelect = true,
                Table = new TableSource(GetReport(context)),
            };

            Add(headerLabel);
            Add(descriptionLabel);
            Add(tableView);
        }

        private BalanceReport GetReport(IContext context)
        {
            context.JournalManager.ReloadJournal();

            var reportBuilder = new BalanceReportBuilder()
            {
                Journal = context.JournalManager.Journal,
                Book = "default",
                AccountQuery = "*",
                Index = null,
                AssetQuery = null,
                IncludeZeroBalances = false,
            };

            return reportBuilder.GetReport();
        }
    }

    internal class TableSource : ITableSource
    {
        private readonly BalanceReport _report;

        public TableSource(BalanceReport report)
        {
            _report = report;
        }

        public int Columns => 6 + 1;

        public int Rows => _report.Items.Length;

        public string[] ColumnNames => ["Account", "Asset", "Total Debit", "Total Credit", "Balance Debit", "Balance Credit", ""];

        public object this[int row, int col]
        {
            get
            {
                return col switch
                {
                    0 => _report.Items[row].Account,
                    1 => _report.Items[row].Asset,
                    2 => _report.Items[row].TotalDebit,
                    3 => _report.Items[row].TotalCredit,
                    4 => _report.Items[row].BalanceDebit,
                    5 => _report.Items[row].BalanceCredit,
                    _ => "",
                };
            }
        }
    }
}
