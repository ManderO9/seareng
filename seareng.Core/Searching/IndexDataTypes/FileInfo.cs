using System.Text.Json.Serialization;

namespace seareng.Core;

/// <summary>
/// A class that will contain info about a file
/// </summary>
public class FileInfo
{
    /// <summary>
    /// The id of the file
    /// </summary>
    [JsonPropertyName("i")]
    public required int FileId { get; set; }

    /// <summary>
    /// The path on disc for that file
    /// </summary>
    [JsonPropertyName("p")]
    public required string Path { get; set; }

    /// <summary>
    /// The total number of terms in this file
    /// </summary>
    [JsonPropertyName("c")]
    public required int TotalTermsCount { get; set; }
}
