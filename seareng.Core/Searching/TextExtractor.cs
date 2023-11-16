namespace seareng.Core;

public class TextExtractor
{

    /// <summary>
    /// Reads the content of the file as text and returns it, or an empty string if failed with an error message
    /// </summary>
    /// <param name="filePath">The path to the file we want to read</param>
    /// <returns>
    /// "content": The content of the file if successfully read.
    /// "successful": Whether we successfully read the file.
    /// "errorMessage": An error message if we failed.
    /// </returns>
    public async Task<(string content, bool successful, string errorMessage)> ReadTextFileContent(string filePath)
    {
        // Try catch any exceptions
        try
        {
            // Read file content as text
            var result = await File.ReadAllTextAsync(filePath);

            // Return the result
            return (result, true, string.Empty);
        }
        // If there was an exception
        catch(Exception ex)
        {
            // Return an error
            return (string.Empty, false, ex.Message);
        }
    }

}
