using seareng.Core;
using System.Net.Mime;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();

app.MapGet("/index", async () =>
{
    return "indexing...";

    var se = new SearchEngine();
    var folderPath = "C:\\Users\\nnnn\\Desktop\\aspnetcore\\src\\";
    await se.IndexFolder(folderPath);

});

app.MapGet("/search", () =>
{
    var se = new SearchEngine();
    var result = se.SearchQuery("hello world");
    return result.Select(x => new { x.filePath, x.weight });
});


app.MapGet("/", () =>   Results.Redirect("/index.html", permanent:true));

app.UseStaticFiles();

app.Run();





//var exes = (await se.GetFilesInDirectory(folderPath)).Select(x => Path.GetExtension(x)).Distinct();
//foreach(var ex in exes)
//{
//    Console.WriteLine(ex);
//}


