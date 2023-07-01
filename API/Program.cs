
using Domain.ProjectHierarchy;
using System.IO.Compression;
using System.Text.Json;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Application.Projects;
//using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlite(@"Data Source=..\Storage\PrimoPrjsdb.db"), ServiceLifetime.Singleton);

var app = builder.Build();

string saveFolder = @"E:\Pet\Data";

app.UseCors(builder =>
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.MapPost("/project", 
    async (ProjectDto project, DataContext db) =>
    {
        Project newProject = new Project()
        {
            Name = project.Name,
            DateTime = DateTime.Now
        };
        
        await db.Projects.AddAsync(newProject);
        await db.SaveChangesAsync();

        return Results.Ok(newProject);
    });

app.MapPost("/project/{id}/addversion",
    async (HttpRequest request, [FromRoute]Guid Id, DataContext db) =>
    {
        var project = await db.Projects.FindAsync(Id);

        if (project == null)
            return Results.BadRequest();

        var lastVersionNum = await Task<int>.Run(() =>
            {
                var pVersions = db.ProjectVersions?.Where(x => x.Project.Id == Id);

                return (pVersions == null || pVersions.Count() == 0) ? 0 : pVersions.Max(x => x.Num);
            });


        if (!request.HasFormContentType)
            return Results.BadRequest();

        var form = await request.ReadFormAsync();

        var prjFile = form.Files?.FirstOrDefault(x => x.Name == "ProjectName");

        if (prjFile == null)
            return Results.BadRequest("There is no project file");

        if (prjFile.Length == 0)
            return Results.BadRequest("File cannot be empty");

        if (prjFile.ContentType != "application/zip" && prjFile.ContentType != "application/x-zip-compressed")
            return Results.BadRequest("Incorrect project file. Waiting for .ZIP");

        string prjFilePath = Path.Combine(saveFolder, prjFile.FileName);

        Directory.Delete(saveFolder, true);
        Directory.CreateDirectory(saveFolder);

        using (FileStream prjFileStream = new FileStream(prjFilePath, FileMode.Create))
        {
            await prjFile.CopyToAsync(prjFileStream);
        }
        
        string saveFolderPath = Path.Combine(Path.GetDirectoryName(prjFilePath), Path.GetFileNameWithoutExtension(prjFilePath));
        await Task.Run(() => ZipFile.ExtractToDirectory(prjFilePath, saveFolderPath));

        ProjectVersion newVersion = new ProjectVersion()
        {
            Num = lastVersionNum + 1,
            Path = saveFolderPath,
            DateTime = DateTime.Now,
            Project = project
        };

        string prjHierarchyStructFilePath = Path.Combine(saveFolderPath, "primo_hierarchy.json");

        using (FileStream prjStructStream = new FileStream(prjHierarchyStructFilePath, FileMode.Create))
        {
            await JsonSerializer.SerializeAsync<ProjectVersion>(prjStructStream, newVersion);
        }

        ContentFile content = new ContentFile(newVersion);

        await db.ProjectVersions.AddAsync(newVersion);
        await db.ContentFiles.AddAsync(content);        
        await db.SaveChangesAsync();


        return Results.Ok(newVersion);
    }).Accepts<IFormFile>("multipart/form-data");

app.MapGet("/project/{id}", 
    async([FromRoute] Guid Id, DataContext db) =>
    {
        var project = await db.ProjectVersions.FirstOrDefaultAsync(x => x.Id == Id);

        if (project == null)
            return Results.NoContent();

        return Results.Ok(project);

    });

app.MapGet("/workfile/{id}",
    async ([FromRoute] Guid Id, DataContext db) =>
    {
        var contentFile = await db.ContentFiles.FirstOrDefaultAsync(x => x.Id == Id);

        if (contentFile == null)
            return Results.NoContent();

        string filePath = contentFile.Path;
        LtwDocument ltwDocument = LtwDocument.LoadFromXml(filePath);

        return Results.Ok(ltwDocument);
    });

app.MapGet("/project/{id}/changes",
    async ([FromRoute] Guid Id, DataContext db) =>
    {
        Project project = await db.Projects.FindAsync(Id);

        var lastVersionNum = await Task<int>.Run(() =>
        {
            var pVersions = db.ProjectVersions?.Where(x => x.Project.Id == Id);

            return (pVersions == null || pVersions.Count() == 0) ? 0 : pVersions.Max(x => x.Num);
        });

        ProjectVersion curVersion = await db.ProjectVersions.FirstOrDefaultAsync(x => x.Project.Id == project.Id && x.Num == lastVersionNum);

        ProjectVersion prevVersion = await db.ProjectVersions.FirstOrDefaultAsync(x => x.Project.Id == project.Id && x.Num == lastVersionNum - 1);

        ContentFile curContentFile = await db.ContentFiles.FirstOrDefaultAsync(x => x.ProjectVersion.Id == curVersion.Id);

        ContentFile prevContentFile = await db.ContentFiles.FirstOrDefaultAsync(x => x.ProjectVersion.Id == curVersion.Id);

        ContentFile diffContent = curContentFile.GetDifferentContent(prevContentFile);

        return Results.Ok(diffContent);

    });

app.MapGet("/ltw/{id}",
    async ([FromRoute] Guid Id, DataContext db) =>
    {
        ContentFile content = await db.ContentFiles.FirstOrDefaultAsync(x => x.Id == Id);


    });

app.Run();
