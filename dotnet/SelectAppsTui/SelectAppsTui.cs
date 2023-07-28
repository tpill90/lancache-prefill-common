﻿namespace LancachePrefill.Common.SelectAppsTui
{
    public partial class SelectAppsTui
    {
        private readonly bool _showReleaseDate;
        private readonly bool _showPlaytime;
        private ListView _listView;
        private TextField _searchBox;
        private StatusBar _statusBar;
        private Label _headerLabel;

        /// <summary>
        /// Used to determine which key was used to exit the TUI.  Will be either Enter/ESC.
        /// </summary>
        private Key _exitKeyPress = Key.Unknown;

        private AppInfoDataSource ListViewDataSource => (AppInfoDataSource)_listView.Source;

        /// <summary>
        /// Tracks which apps were selected when they opened up the select-apps interface.
        /// Used to determine if the user has made any differing selections.
        /// </summary>
        private readonly List<string> _previouslySelectedApps;

        public SelectAppsTui(List<TuiAppInfo> availableGames, bool showReleaseDate = true, bool showPlaytime = true)
        {
            _showReleaseDate = showReleaseDate;
            _showPlaytime = showPlaytime;

            _previouslySelectedApps = availableGames.Where(e => e.IsSelected)
                                                    .Select(e => e.AppId)
                                                    .ToList();

            InitLayout(availableGames);

            // Configuring status bar actions
            _statusBar.Items = new StatusItem[] {
                new StatusItem(Key.Esc, "~ESC~ to Quit", () =>
                {
                    // Prevents a user from making changes and accidentally losing them by hitting escape
                    if (UserHasUnsavedChanges())
                    {
                        var message = $"If you exit the app, any changes you have made will be lost. {Environment.NewLine}  Are you sure that you want to exit?";
                        int userSelection = MessageBox.Query(width: 60, height: 6, "Unsaved changes!",
                            message, defaultButton: 1, "Discard changes", "Cancel");

                        // 1 refers to the second button in the MessageBox, in this case "Cancel".  Exiting the handler so we don't lose changes
                        if (userSelection == 1)
                        {
                            return;
                        }
                    }
                    _exitKeyPress = Key.Esc;
                    Application.RequestStop(Application.Top);
                    Application.Top.SetNeedsDisplay();
                }),
                new StatusItem (Key.CharMask, "~↑/↓/PgUp/PgDn~ to navigate", null),
                new StatusItem (Key.CharMask, "~Space~ to select", null),
                new StatusItem (Key.CtrlMask | Key.A, "~CTRL-A~ Select All", () =>
                {
                    ListViewDataSource.SetAllSelected(true);
                    _listView.SetNeedsDisplay();
                }),
                new StatusItem (Key.CtrlMask | Key.C, "~CTRL-C~ Clear All", () =>
                {
                    ListViewDataSource.SetAllSelected(false);
                    _listView.SetNeedsDisplay();
                }),
                new StatusItem (Key.Enter, "~Enter~ to Save", () =>
                {
                    // Prevents the user from hitting ENTER without selecting anything to prefill
                    if (!ListViewDataSource.SelectedApps.Any())
                    {
                        var message = $"No apps have been selected!  {Environment.NewLine}" +
                                            "At least one app is required to continue, and can be selected using the space bar.";
                        MessageBox.Query(width: 80, height: 6, "No apps selected!",message, "Close");

                        return;
                    }
                    _exitKeyPress = Key.Enter;
                    Application.RequestStop(Application.Top);
                    Application.Top.SetNeedsDisplay();
                })
            };

            _headerLabel.Text = ListViewDataSource.FormatHeaderString();
        }

        public Key Run()
        {
            _searchBox.SetFocus();
            Application.Run(Application.Top);
            Application.Shutdown();
            return _exitKeyPress;
        }

        #region Event handlers

        private void SearchBox_OnTextChanged(ustring obj)
        {
            var searchText = _searchBox.Text.ToString();

            ListViewDataSource.FilterItems(searchText);
            _listView.MoveHome();
            _listView.SetNeedsDisplay();
        }

        private void SortName_OnClicked()
        {
            ListViewDataSource.SortTitle();
            _listView.SetNeedsDisplay();
        }

        private void SortYear_OnClicked()
        {
            ListViewDataSource.SortYear();
            _listView.SetNeedsDisplay();
        }

        private void SortPlaytime_OnClicked()
        {
            ListViewDataSource.SortPlaytime();
            _listView.SetNeedsDisplay();
        }

        private void SortSelected_OnClicked()
        {
            ListViewDataSource.SortSelected();
            _listView.SetNeedsDisplay();
        }

        #endregion


        private bool UserHasUnsavedChanges()
        {
            var currentlySelected = ListViewDataSource.SelectedApps.Select(e => e.AppId).ToHashSet();
            currentlySelected.SymmetricExceptWith(_previouslySelectedApps);

            // If there are any differences at all, then the user has unsaved changes
            return currentlySelected.Any();
        }
    }
}