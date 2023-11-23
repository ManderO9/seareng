using seareng.Core;
using System.Buffers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();

var folderPath = "C:\\Users\\nnnn\\Desktop\\aspnetcore\\src\\";


app.MapGet("/index", async () =>
{
    var se = new SearchEngine();
    await se.IndexFolder(folderPath);

    return "finished indexing";
});

app.MapPost("/search", async (context) =>
{
    var result = await context.Request.BodyReader.ReadAsync();
    var str = Encoding.UTF8.GetString(result.Buffer.ToArray());

    var se = new SearchEngine();
    var files = se.SearchQuery(str);
    await context.Response.WriteAsJsonAsync(
        files.Select(x => new { File = x.filePath.Substring(folderPath.Length), Weight = x.weight }));
});

app.MapGet("/", () => Results.Redirect("/index.html", permanent: true));



app.UseStaticFiles();

app.Run();





//var exes = (await se.GetFilesInDirectory(folderPath)).Select(x => Path.GetExtension(x)).Distinct();
//foreach(var ex in exes)
//{
//    Console.WriteLine(ex);
//}


