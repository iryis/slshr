namespace SlashrNext.Objects;

public class XKCD
{
    public XKCD(string month, int num, string link, string year, string news, string safeTitle, string transcript, string alt, string img, string title, string day)
    {
        this.month = month;
        this.num = num;
        this.link = link;
        this.year = year;
        this.news = news;
        safe_title = safeTitle;
        this.transcript = transcript;
        this.alt = alt;
        this.img = img;
        this.title = title;
        this.day = day;
    }

    public string month { get; set; }
    public int num { get; set; }
    public string link { get; set; }
    public string year { get; set; }
    public string news { get; set; }
    public string safe_title { get; set; }
    public string transcript { get; set; }
    public string alt { get; set; }
    public string img { get; set; }
    public string title { get; set; }
    public string day { get; set; }
}