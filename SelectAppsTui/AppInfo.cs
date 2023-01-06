namespace LancachePrefill.Common.SelectAppsTui
{
    //TODO document
    public sealed class AppInfo
    {
        public string AppId { get; init; }
        public string Title { get; init; }
        public DateTime? ReleaseDate { get; set; }

        public int? MinutesPlayed { get; set; }
        public decimal? HoursPlayed => MinutesPlayed == null ? null : (decimal)MinutesPlayed / 60;

        public bool IsSelected { get; set; }

        public AppInfo(string appId, string title)
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