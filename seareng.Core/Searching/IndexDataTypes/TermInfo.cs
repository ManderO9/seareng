using System.Text.Json.Serialization;

namespace seareng.Core;

/// <summary>
/// A class that contains info about each term that we are indexing
/// </summary>
public class TermInfo
{
    /// <summary>
    /// Contains the number of occurrences of the term in all files summed up
    /// </summary>
    [JsonPropertyName("n")]
    public required int TotalTermCount { get; set; }

    /// <summary>
    /// Contains the number of files that contain the term
    /// </summary>
    [JsonPropertyName("c")]
    public required int FilesCount { get; set; }

    /// <summary>
    /// The term we are indexing
    /// </summary>
    [JsonPropertyName("t")]
    public required string Term { get; set; }

    /// <summary>
    /// A list of all files that contain the term and the number of times the term appears in each file
    /// </summary>
    [JsonPropertyName("f")]
    public required List<FileOccurrences> FileOccurrences { get; set; }
}


