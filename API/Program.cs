
using Domain.ProjectHierarchy;
using System.IO.Compression;
using System.Text.Json;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Projects = Application.Projects;
using ProjectVersions = Application.ProjectVersions;
using Application.Projects;
using MediatR;
using Application.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlite(@"Data Source=..\Storage\PrimoPrjsdb.db"), ServiceLifetime.Singleton);
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IRequestDataExtractor<IFormFile?>, ProjectFileExtractor>();


var app = builder.Build();

string saveFolder = @"E:\Pet\Data";

app.UseCors(builder =>
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.MapPost("/project", 
    async (IMediator mediator, ProjectCreateDto project) =>
    {
        var result = await mediator.Send(new Projects.CreateProjectCommand() {Project = project});

        return Results.Ok(result.Value);
    });

app.MapGet("/project/{id}",
    async (IMediator mediator, [FromRoute] Guid Id) =>
    {
        var result = await mediator.Send(new Projects.GetDetailsQuery() { Id = Id });

        return Results.Ok(result.Value);

    });

app.MapPost("/project/{id}/addversion",
    async (IMediator mediator, IRequestDataExtractor<IFormFile?> reqFileExtractor, HttpRequest request, [FromRoute]Guid Id) =>
    {
        var projectQueryRes = await mediator.Send(new Projects.GetDetailsQuery() { Id = Id });

        if (projectQueryRes.Value == null)
            return Results.BadRequest();

        Project project = projectQueryRes.Value;

        var lastVersionQueryRes = await mediator.Send(new ProjectVersions.GetLastProjectVersionQuery() { ProjectId = Id });
        int lastVersionNum = lastVersionQueryRes.Value?.Num ?? 0;

        var result = await reqFileExtractor.GetRequestData(request);

        if (!result.IsSuccess)
            return Results.BadRequest(result.Value);

        var prjFile = result.Value;

        string prjFilePath = Path.Combine(saveFolder, prjFile.FileName);

        using (FileStream prjFileStream = new FileStream(prjFilePath, FileMode.Create))
        {
            await prjFile.CopyToAsync(prjFileStream);
        }
        
        string saveFolderPath = Path.Combine(Path.GetDirectoryName(prjFilePath), Guid.NewGuid().ToString(), Path.GetFileNameWithoutExtension(prjFilePath));
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

        var savePrjVersionRes = await mediator.Send(new ProjectVersions.CreateVersionCommand() { NewVersion = newVersion });

        //await db.ContentFiles.AddAsync(content);        
        //await db.SaveChangesAsync();


        return Results.Ok(newVersion);
    }).Accepts<IFormFile>("multipart/form-data");



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

        ContentFile prevContentFile = await db.ContentFiles.FirstOrDefaultAsync(x => x.ProjectVersion.Id == prevVersion.Id);

        ContentFile diffContent = curContentFile.GetDifferentContent(prevContentFile);

        return Results.Ok(diffContent);

    });

app.MapGet("/ltw/{id}",
    async ([FromRoute] Guid Id, DataContext db) =>
    {
        
        ContentFile contentFile = await db.ContentFiles
                                            .Include(pV => pV.ProjectVersion)
                                            .FirstOrDefaultAsync(x => x.Id == Id);
        

        LtwDocument ltwDocument = LtwDocument.LoadFromXml(contentFile.FullPath);
        

        return Results.Ok(ltwDocument);
    });

//app.MapGet("/version/{versionId}/ltw", 
//    async([FromRoute] Guid Id
//        , [FromQuery(Name ="Id")] Guid ltwId
//        , DataContext db) =>
//    {
        
//    });

app.Run();
