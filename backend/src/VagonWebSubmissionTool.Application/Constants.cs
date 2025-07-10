namespace VagonWebSubmissionTool.Application;

public static class Constants
{
    public const string VagonWebDomain = "www.vagonweb.cz";
    public const string HttpClientName = "vagonWEB";
    
    public static Uri VagonWebHost => new($"https://{VagonWebDomain}");
}