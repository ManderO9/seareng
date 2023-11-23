using System;
using System.Text.Json;

namespace seareng.Core;

public class SearchEngine
{
    public List<(string filePath, double weight)> SearchQuery(string query)
    {
        // TODO: move this somewhere else
        // Read the index file
        var fileContent = File.ReadAllText("./data-index.json");

        // Create the index object from the read file
        var index = JsonSerializer.Deserialize<IndexData>(fileContent)!;

        // Get query terms
        var terms = Tokenize(query);

        // If there was no terms to search
        if(terms == null)
            // Return an empty list
            return new();

        // The list that will contain the id of the files to return as well as it's weight
        var filesResult = new Dictionary<int, double>();

        // For each term
        foreach(var term in terms)
        {
            // Get the term's entry in the index
            var indexEntry = index.Terms.FirstOrDefault(x => x.Term == term.Key);

            // If the term does not exist in the index
            if(indexEntry is null)
                // Skip it
                continue;

            // For each file that the term occurs in
            foreach(var file in indexEntry.FileOccurrences)
            {
                // Calculate a weight for this file
                var addedWeight = (double)file.Occurrences / indexEntry.FilesCount;
                
                // Add it to the list of weights and file ids
                filesResult[file.FileId] = filesResult.GetValueOrDefault(file.FileId) + addedWeight;
            }
        }

        // Order the files by their weight
        var result = filesResult.Select(x =>
             (index.Files.First(y => y.FileId == x.Key).Path,x.Value))
            .OrderByDescending(x => x.Value);

        // Return the result
        return result.ToList();
    }

    /*    
     [
         (term, totalOccurrenceOfTheTermInAllFiles, numberOfFilesContainingTheTerm, [(fileId, countOfTerm),(fileId, countOfTerm),(fileId, countOfTerm)]),
         (term, totalOccurrenceOfTheTermInAllFiles, numberOfFilesContainingTheTerm, [(fileId, countOfTerm),(fileId, countOfTerm),(fileId, countOfTerm)]),
         (term, totalOccurrenceOfTheTermInAllFiles, numberOfFilesContainingTheTerm, [(fileId, countOfTerm),(fileId, countOfTerm),(fileId, countOfTerm)])
     ],

     [
         (fileId, path, totalTermsCount),
         (fileId, path, totalTermsCount),
         (fileId, path, totalTermsCount),
     ],
     */


    public async Task IndexFolder(string folderPath)
    {
        // Get the files in the directory we want to index
        var files = (await GetFilesInDirectory(folderPath)).Take(500);

        // Create text extractor class
        var textExtractor = new TextExtractor();

        // A data structure that will contain the result of the initial indexation of the files
        // with the structure:
        // filePath: 
        //      [
        //          term: numberOfOccurrences
        //      ],
        //      totalNumberOfTerms
        var fileTerms = new Dictionary<string, (Dictionary<string, int> terms, int totalTermsCount)>();

        // TODO: tokenization, ie: the next loop is the slowest part of the operation, need to find a fix for it

        // For each file
        foreach(var filePath in files)
        {
            // Get file extension
            var fileExtension = Path.GetExtension(filePath);

            // Switch the type of the file
            switch(fileExtension)
            {
                // If it's a source file
                case ".cs":
                case ".js":
                case ".py":
                case ".ts":
                case ".cpp":
                case ".c":
                case ".java":
                case ".txt":
                    // Read the text of the file
                    var result = await textExtractor.ReadTextFileContent(filePath);

                    // If successfully read the file
                    if(result.successful)
                    {
                        // Get the tokens in the file
                        var tokens = Tokenize(result.content);

                        // If we got any tokens
                        if(tokens != null)
                            // Add them to the list of files with their terms
                            fileTerms.Add(filePath, (tokens, tokens.Sum(x => x.Value)));
                    }
                    // Otherwise, if we failed to read the file content
                    else
                    {
                        // TODO: log an error
                    }

                    // Break
                    break;
            }

        }

        // An incrementing counter that will contain and id of newly indexed files
        int fileId = 0;

        // The list of files that we have indexed
        var filesList = new List<FileInfo>();

        // TODO: maybe try to find another way, this line of code might be too slow
        // Get all the terms available in the file terms list
        var allTerms = fileTerms.Select(x => x.Value.terms.Select(y => y.Key))
                        .Aggregate((x, y) => x.Concat(y))
                        // Remove duplicates
                        .Distinct();

        // Create a list of term info from the list of terms that exist with initial values
        var termsList = allTerms.Select(x =>
            new TermInfo()
            {
                FileOccurrences = new List<FileOccurrences>(),
                FilesCount = 0,
                Term = x,
                TotalTermCount = 0
            }).ToDictionary(s => s.Term);

        // For each file that we have
        foreach(var fileEntry in fileTerms)
        {
            // Create file info object
            var info = new FileInfo()
            {
                Path = fileEntry.Key,
                FileId = fileId++,
                TotalTermsCount = fileEntry.Value.totalTermsCount
            };

            // For each term in the current file entry
            foreach(var term in fileEntry.Value.terms)
            {
                // Get the term entry from the list of terms 
                var termEntry = termsList[term.Key];

                // Add to the list of files that contain the term
                termEntry.FileOccurrences.Add(new()
                {
                    FileId = info.FileId,
                    Occurrences = term.Value
                });

                // Increment the files count
                termEntry.FilesCount += 1;

                // Add the number of occurrences to the total number of occurrences
                termEntry.TotalTermCount += term.Value;
            }

            // Add the file entry to the list of files
            filesList.Add(info);
        }

        // TODO: Maybe change the list of terms into a dictionary
        // Create the index object
        var index = new IndexData() { Files = filesList, Terms = termsList.Select(x => x.Value).ToList() };

        // TODO: delete testing code
        var serializedIndex = JsonSerializer.Serialize(index);
        await File.WriteAllTextAsync("./data-index.json", serializedIndex);


        //var fileStream = File.OpenWrite("./data-index.json");
        //await JsonSerializer.SerializeAsync(fileStream, fileTerms);

    }



    /// <summary>
    /// Characters that will separate between different tokens
    /// </summary>
    private char[] mTokenDelimiters = " .,;\r\n\t\\\"/'!?#$%^&*[]-+={}()<>:||".ToCharArray();

    /// <summary>
    /// Returns the number of occurrences of each term in the passed in text
    /// </summary>
    /// <param name="text">The text to search occurrences in</param>
    /// <returns></returns>
    /// <remarks>
    ///     The tokenization ignores terms that are only one character long,
    ///     It does not return numbers or any terms that contain at least one character as a digit,
    ///     they all get deleted.
    /// </remarks>
    public Dictionary<string, int>? Tokenize(string text)
    {
        // If the passed in text is empty
        if(string.IsNullOrWhiteSpace(text))
            // Don't return anything
            return null;

        // Create a dictionary that will contain the occurrences of each term
        var dictionary = new Dictionary<string, int>();

        // Split the passed in text into multiple terms
        var tokens = text.Split(mTokenDelimiters, StringSplitOptions.RemoveEmptyEntries);

        // For each term
        foreach(var token in tokens)
        {
            // Skip terms that are only one character long
            if(token.Length == 1)
                continue;

            // Skip terms that contain digits
            if(token.Any(char.IsDigit))
                continue;

            // Normalize it
            var lower = token.ToLower();

            // Add it's value to the dictionary
            dictionary[lower] = dictionary.GetValueOrDefault(lower) + 1;
        }

        // Return the dictionary
        return dictionary;
    }


    /// <summary>
    /// Returns a list of file paths for all the files contained in the passed in directory,
    /// recursively checks subdirectories as well
    /// </summary>
    /// <param name="folderPath">The director to check the files in</param>
    /// <returns></returns>
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
