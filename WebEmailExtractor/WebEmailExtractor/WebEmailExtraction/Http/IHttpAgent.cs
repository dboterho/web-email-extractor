namespace WebEmailExtractor.WebEmailExtraction.Http
{
    public interface IHttpAgent
    {

        string GetWebPageMarkup(string url);

    }
}
