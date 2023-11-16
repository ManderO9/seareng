namespace seareng.Core;

public class SearchEngine
{
    public List<string> SearchQuery(string query)
    {
        // Get query terms

        // For each term

        // do something

        return new();
    }


    public async Task IndexFolder(string folderPath)
    {
        // Get the files in the directory we want to index
        var files = await GetFilesInDirectory(folderPath);
    
        // For each file
        foreach(var file in files)
        {
            // Switch the type of the file


        }

    }

    public async Task<List<string>> GetFilesInDirectory(string folderPath)
    {
        // Create the list of files we want to return
        var filePaths = new List<string>();

        // Get sub directories in this folder
        var subDirectories = Directory.GetDirectories(folderPath);

        // Add all the files in the current directory
        filePaths.AddRange(Directory.GetFiles(folderPath));

        // For each sub directory
        foreach(var directory in subDirectories)
        {
            // Add the files in that directory
            filePaths.AddRange(await GetFilesInDirectory(directory));
        }

        // Return the list of files
        return filePaths;
    }


}
