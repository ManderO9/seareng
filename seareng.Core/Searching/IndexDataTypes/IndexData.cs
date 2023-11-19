namespace seareng.Core;


/// <summary>
/// Represents the data that we will save in our index and use to answer queries and search for file matches
/// </summary>
public class IndexData
{
    /// <summary>
    /// The list of all terms in our index
    /// </summary>
    public required List<TermInfo> Terms { get; set; }

    /// <summary>
    /// The list of files in our index
    /// </summary>
    public required List<FileInfo> Files { get; set; }
}


