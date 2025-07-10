using CSharpFunctionalExtensions;

namespace VagonWebSubmissionTool.Application.Services;

public interface IUicWagonNumbersService
{
    public Result<string, string> GetCountryCodeByUicNumber(string uicNumber);
}

public class UicWagonNumbersService : IUicWagonNumbersService
{
    private Dictionary<int, string> _countryCodes = new()
    {
        { 81, "A" },
        { 41, "AL" },
        { 88, "B" },
        { 52, "BG" },
        { 49, "BIH" },
        { 21, "BLR" },
        { 62, "CG" },
        { 54, "CZ" },
        { 80, "D" },
        { 86, "DK" },
        { 71, "E" },
        { 87, "F" },
        { 10, "FIN" },
        { 70, "GB" },
        { 73, "GR" },
        { 55, "H" },
        { 78, "HR" },
        { 85, "CH" },
        { 83, "I" },
        { 82, "L" },
        { 24, "LT" },
        { 25, "LV" },
        { 23, "MD" },
        { 65, "MK" },
        { 76, "N" },
        { 84, "NL" },
        { 94, "P" },
        { 51, "PL" },
        { 53, "RO" },
        { 74, "S" },
        { 56, "SK" },
        { 79, "SLO" },
        { 72, "SRB" },
        { 75, "TR" },
        { 22, "UA" },
        { 58, "AM" },
        { 57, "AZ" },
        { 33, "CN" },
        { 59, "KG" },
        { 31, "MGL" },
        { 20, "RUS" },
        { 66, "TJ" }
    };
    
    public Result<string, string> GetCountryCodeByUicNumber(string uicNumber)
    {
        uicNumber = SanitizeUicNumber(uicNumber);
        if (uicNumber.Length != 12)
        {
            return Result.Failure<string, string>("Invalid UIC Number");
        }

        var uicCountryCode = int.Parse(uicNumber[2..4]);
        return _countryCodes.TryGetValue(uicCountryCode, out var countryCode)
            ? Result.Success<string, string>(countryCode)
            : Result.Failure<string, string>("Invalid UIC Country Code");
    }

    private string SanitizeUicNumber(string uicNumber)
    {
        return uicNumber.Trim().Replace(" ", string.Empty).Replace("-", string.Empty);
    }
}