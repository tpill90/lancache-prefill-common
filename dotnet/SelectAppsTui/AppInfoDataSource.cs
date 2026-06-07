namespace LancachePrefill.Common.SelectAppsTui
{
    internal sealed class AppInfoDataSource : IListDataSource
    {
        public int Count => _currAppInfo != null ? _currAppInfo.Count : 0;

        private readonly int _maxItemLength;
        public int Length => _maxItemLength;

        private List<TuiAppInfo> _originalAppInfo;
        private List<TuiAppInfo> _currAppInfo;

        // TODO refactor these flags to just be "_showAdditionalColumns" instead of picking and choosing.
        // I don't want to implement all that conditional logic to be able to do so.
        private readonly bool _showReleaseDate;
        private readonly bool _showPlaytime;
        private readonly bool _showPurchaseDate;

        public List<TuiAppInfo> SelectedApps => _originalAppInfo.Where(e => e.IsSelected).ToList();

        public AppInfoDataSource(List<TuiAppInfo> itemList, bool showReleaseDate = true, bool showPlaytime = true, bool showPurchaseDate = false)
        {
            _currAppInfo = itemList;
            _showReleaseDate = showReleaseDate;
            _showPlaytime = showPlaytime;
            _showPurchaseDate = showPurchaseDate;

            _originalAppInfo = itemList.ToList();
            _maxItemLength = _currAppInfo.Select(e => FormatItemString(e).Length).Max();
        }

        public bool IsMarked(int item)
        {
            if (item >= 0 && item < _currAppInfo.Count)
            {
                return _currAppInfo[item].IsSelected;
            }

            return false;
        }

        public void SetMark(int item, bool value)
        {
            if (item >= 0 && item < _currAppInfo.Count)
            {
                _currAppInfo[item].IsSelected = value;
            }
        }

        public void SetAllSelected(bool isSelected)
        {
            foreach (var app in _currAppInfo)
            {
                app.IsSelected = isSelected;
            }
        }

        public void FilterItems(string searchText)
        {
            var currAppInfo = _originalAppInfo.Where(e => e.Title.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
            _currAppInfo = currAppInfo;
        }

        public void SortTitle()
        {
            _currAppInfo = _currAppInfo.OrderBy(e => e.Title, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private bool _sortYearToggle;
        public void SortYear()
        {
            if (_sortYearToggle)
            {
                _currAppInfo = _currAppInfo.OrderBy(e => e.ReleaseDate).ToList();
            }
            else
            {
                _currAppInfo = _currAppInfo.OrderByDescending(e => e.ReleaseDate).ToList();
            }
            _sortYearToggle = !_sortYearToggle;
        }

        public void SortPlaytime()
        {
            _currAppInfo = _currAppInfo.OrderByDescending(e => e.MinutesPlayed).ToList();
        }

        public void SortPurchaseDate()
        {
            _currAppInfo = _currAppInfo.OrderByDescending(e => e.PurchaseDate).ToList();
        }

        public void SortSelected()
        {
            _currAppInfo = _currAppInfo.OrderByDescending(e => e.IsSelected)
                                       .ThenBy(e => e.Title)
                                       .ToList();
        }

        /// <summary>
        /// Builds the string that is used as the TUI's header text.
        /// </summary>
        public string FormatHeaderString()
        {
            // If there are no extra columns enabled then just return the title only
            if (!_showPlaytime && !_showReleaseDate && !_showPurchaseDate)
            {
                // First column needs to be +3 additional characters, to account for the ' - ' added by the list control
                return String.Format("{0,-58}", "   Title");
            }

            return String.Format("{0,-58}{1,8}{2,17}{3,14}", "   Title", "Released", "Recent Playtime", "Purchased");
        }

        private string FormatItemString(TuiAppInfo item)
        {
            // We cannot use the built-in padding method as it doesn't consider unicode.
            // Titles with Chinese in them will throw off the padding for the whole row.
            var nameFormatted = item.Title.Truncate(55).PadRightUnicode(55);
            var hoursPlayed2Weeks = item.HoursPlayed != null ? $"{item.HoursPlayed:N1} hours" : null;

            return string.Format("{0}{1,8}{2,17}{3,14}",
                nameFormatted,
                item.ReleaseDate?.Date.ToString("yyyy"),
                hoursPlayed2Weeks,
                item.PurchaseDate?.Date.ToString("yyyy-MM-dd"));
        }

        public void Render(ListView container, ConsoleDriver driver, bool selected, int item, int col, int line, int width, int start = 0)
        {
            container.Move(col, line);
            RenderUstr(driver, FormatItemString(_currAppInfo[item]), width, start);
        }

        // A slightly adapted method from: https://github.com/gui-cs/Terminal.Gui/blob/fc1faba7452ccbdf49028ac49f0c9f0f42bbae91/Terminal.Gui/Views/ListView.cs#L433-L461
        private void RenderUstr(ConsoleDriver driver, ustring ustr, int width, int start = 0)
        {
            int used = 0;
            int index = start;
            while (index < ustr.Length)
            {
                (var rune, var size) = Utf8.DecodeRune(ustr, index, index - ustr.Length);
                var count = Rune.ColumnWidth(rune);
                if (used + count >= width)
                {
                    break;
                }
                driver.AddRune(rune);
                used += count;
                index += size;
            }

            while (used < width)
            {
                driver.AddRune(' ');
                used++;
            }
        }

        public IList ToList()
        {
            return _originalAppInfo;
        }
    }
}