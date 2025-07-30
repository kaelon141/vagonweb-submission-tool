namespace VagonWebSubmissionTool.Application.Services;

public interface ILanguageResolver
{
    public static sealed Language DefaultLanguage => Language.English;
    
    public Language Resolve();
}