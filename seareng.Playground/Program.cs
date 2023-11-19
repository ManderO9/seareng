using seareng.Core;
using System.Text.Json;

var se = new SearchEngine();
var folderPath = "C:\\Users\\nnnn\\Desktop\\aspnetcore\\src\\";
await se.IndexFolder(folderPath);


//var exes = (await se.GetFilesInDirectory(folderPath)).Select(x => Path.GetExtension(x)).Distinct();
//foreach(var ex in exes)
//{
//    Console.WriteLine(ex);
//}


Console.WriteLine("finished program");
Console.ReadLine();
