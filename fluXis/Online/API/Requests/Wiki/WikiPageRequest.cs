namespace fluXis.Online.API.Requests.Wiki;

public class WikiPageRequest : APIRequest
{
    protected override string Path => $"/fluxis-wiki/{page.Trim('/')}/en.md";
    protected override string RootUrl => APIClient.Endpoint.WebsiteRootUrl;

    public string ResponseString { get; private set; }

    private string page { get; }

    public WikiPageRequest(string page)
    {
        this.page = page;
    }

    protected override void PostProcess()
    {
        base.PostProcess();
        ResponseString = Request.GetResponseString();
    }
}
