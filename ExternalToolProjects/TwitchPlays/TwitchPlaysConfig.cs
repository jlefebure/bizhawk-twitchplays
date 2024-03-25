using System.IO;
using BizHawk.Common.PathExtensions;

namespace TwitchPlays;

public class TwitchPlaysConfig
{
    public string? Login { get; set; }
    public string? OauthToken { get; set; }
    public string? Channel { get; set; }
    
    public static string ControlDefaultPath => Path.Combine(PathUtils.ExeDirectoryPath, "tpp.json");

}