namespace LancachePrefill.Common.SelectAppsTui
{
    //TODO document
    public sealed class TuiAppInfo
    {
        public string AppId { get; }
        public string Title { get; }
        public DateTime? ReleaseDate { get; set; }

        public int? MinutesPlayed { get; set; }
        public decimal? HoursPlayed => MinutesPlayed == null ? null : (decimal)MinutesPlayed / 60;

        public bool IsSelected { get; set; }

        public TuiAppInfo(string appId, string title)
        {
            AppId = appId;
            Title = title;
        }

        public override string ToString()
        {
            return $"{Title.EscapeMarkup()}";
        }
    }
}