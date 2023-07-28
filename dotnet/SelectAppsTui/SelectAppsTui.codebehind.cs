﻿using Color = Terminal.Gui.Color;
using Attribute = Terminal.Gui.Attribute;

namespace LancachePrefill.Common.SelectAppsTui
{
    [SuppressMessage("Naming", "CA1724: Type names should not match namespaces",
        Justification = "I don't want to change the namespace or class name, any other name might make it harder to understand")]
    public sealed partial class SelectAppsTui : IDisposable
    {
        #region Style overrides

        // Overrides for the default color scheme
        private readonly ColorScheme _buttonColorScheme = new ColorScheme
        {
            Normal = new Attribute(foreground: Color.White, background: Color.Black),
            HotNormal = new Attribute(foreground: Color.White, background: Color.Black),
            Focus = new Attribute(foreground: Color.BrightBlue, background: Color.Black),
            HotFocus = new Attribute(foreground: Color.BrightBlue, background: Color.Black),
        };

        /// <summary>
        /// Overrides the default symbol used by ListView when an item is selected
        /// </summary>
        private readonly char _listViewCheckedSymbol = '✓';

        /// <summary>
        /// Overrides the default symbol used by ListView when an item is not selected
        /// </summary>
        private readonly char _listViewUncheckedSymbol = ' ';

        /// <summary>
        /// Changes the default coloring of the "selected/focused" entry, as well as coloring previously selected apps.
        /// </summary>
        /// <param name="obj"></param>
        private void ListView_RowRender(ListViewRowEventArgs obj)
        {
            if (obj.Row == _listView.SelectedItem && _listView.HasFocus)
            {
                obj.RowAttribute = new Attribute(Color.Black, Color.Gray);
                return;
            }
            if (_listView.Source.IsMarked(obj.Row))
            {
                obj.RowAttribute = new Attribute(Color.Brown, Color.Black);
            }
        }

        #endregion

        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>")]
        private void InitLayout(List<TuiAppInfo> appInfos)
        {
            View.Driver.Checked = _listViewCheckedSymbol;
            View.Driver.UnChecked = _listViewUncheckedSymbol;

            var window = new Window("")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = Colors.TopLevel
            };
            Application.Top.Add(window);

            #region First Row - Sorting

            var sortLabel = new Label("Sort:")
            {
                X = 1,
                ColorScheme = _buttonColorScheme
            };

            // Selected
            var sortSelectedButton = new Button("Selected")
            {
                X = Pos.Right(sortLabel) + 1,
                ColorScheme = _buttonColorScheme
            };
            sortSelectedButton.Clicked += SortSelected_OnClicked;

            // Name
            var sortNameButton = new Button("Name")
            {
                X = Pos.Right(sortSelectedButton) + 1,
                ColorScheme = _buttonColorScheme
            };
            sortNameButton.Clicked += SortName_OnClicked;

            // Release year
            var sortYearButton = new Button("Year")
            {
                X = Pos.Right(sortNameButton) + 1,
                ColorScheme = _buttonColorScheme
            };
            sortYearButton.Clicked += SortYear_OnClicked;
            sortYearButton.Enabled = _showReleaseDate;

            // Playtime
            var sortPlaytimeButton = new Button("Recent Playtime")
            {
                X = Pos.Right(sortYearButton) + 1,
                ColorScheme = _buttonColorScheme
            };
            sortPlaytimeButton.Clicked += SortPlaytime_OnClicked;
            sortPlaytimeButton.Enabled = _showPlaytime;

            window.Add(sortLabel, sortNameButton, sortYearButton, sortPlaytimeButton, sortSelectedButton);

            #endregion

            #region Second Row

            var searchLabel = new Label("Search: ")
            {
                X = 1,
                Y = 2
            };
            _searchBox = new TextField
            {
                X = Pos.Right(searchLabel) + 1,
                Y = 2,
                Width = 50
            };
            _searchBox.TextChanged += SearchBox_OnTextChanged;
            _searchBox.AddKeyBinding(Key.CtrlMask | Key.A, Command.SelectAll);
            window.Add(searchLabel, _searchBox);

            #endregion

            var lineView = new LineView(Orientation.Horizontal)
            {
                Y = 3,
                Width = Dim.Fill()
            };
            window.Add(lineView);

            _headerLabel = new Label
            {
                Y = 4,
                Width = Dim.Fill()
            };
            window.Add((View)_headerLabel);

            var lineView2 = new LineView(Orientation.Horizontal)
            {
                Y = 5,
                Width = Dim.Fill()
            };
            window.Add(lineView2);

            _listView = new ListView
            {
                X = 1,
                Y = 6,
                Height = Dim.Fill(),
                Width = Dim.Fill(1),
                ColorScheme = new ColorScheme
                {
                    Normal = new Attribute(foreground: Color.Gray, background: Color.Black),
                    HotNormal = new Attribute(foreground: Color.Gray, background: Color.Black)
                },
                AllowsMarking = true,
                AllowsMultipleSelection = true
            };
            _listView.RowRender += ListView_RowRender;
            _listView.Source = new AppInfoDataSource(appInfos, _showReleaseDate, _showPlaytime);
            window.Add((View)_listView);

            _statusBar = new StatusBar
            {
                Visible = true,
                ColorScheme = new ColorScheme
                {
                    Normal = new Attribute(Color.White, Color.Black),
                    HotNormal = new Attribute(foreground: Color.BrightGreen, background: Color.Black),
                }
            };
            Application.Top.Add((View)_statusBar);
        }

        public void Dispose()
        {
            _listView.Dispose();
            _searchBox.Dispose();
            _headerLabel.Dispose();
            _statusBar.Dispose();
        }
    }
}