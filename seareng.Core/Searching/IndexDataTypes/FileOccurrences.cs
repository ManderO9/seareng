using System.Text.Json.Serialization;

namespace seareng.Core;

/// <summary>
/// A class that will contain info that is related to a particular term on 
/// how many occurrences does the term has in this file
/// </summary>
public class FileOccurrences
{
    /// <summary>
    /// The id of the file
    /// </summary>
    [JsonPropertyName("i")]
    public required int FileId { get; set; }

    /// <summary>
    /// The number of occurrences of the term in this file
    /// </summary>
    [JsonPropertyName("o")]
    public required int Occurrences { get; set; }
}


