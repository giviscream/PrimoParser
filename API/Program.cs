
using Domain.ProjectHierarchy;
using System.IO.Compression;
using System.Text.Json;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Application.Projects;
using Application.ProjectVersions;
using Application.ContentFiles;
using MediatR;
using Application.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlite(@"Data Source=..\Storage\PrimoPrjsdb.db"), ServiceLifetime.Singleton);
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IRequestDataExtractor<IFormFile>, ProjectFileExtractor>();


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
        var result = await mediator.Send(new CreateProjectCommand() {Project = project});

        return Results.Ok(result.Value);
    });

app.MapGet("/project/{id}",
    async (IMediator mediator, [FromRoute] Guid Id) =>
    {
        var result = await mediator.Send(new GetDetailsQuery() { Id = Id });

        return Results.Ok(result.Value);

    });

app.MapPost("/project/{id}/addversion",
    async (IMediator mediator, IRequestDataExtractor<IFormFile?> reqFileExtractor, HttpRequest request, [FromRoute]Guid Id) =>
    {
        var projectQueryRes = await mediator.Send(new GetDetailsQuery() { Id = Id });

        if (projectQueryRes.Value == null)
            return Results.BadRequest($"No project with Id = {Id}");

        Project project = projectQueryRes.Value;

        var lastVersionQueryRes = await mediator.Send(new GetLastProjectVersionQuery() { ProjectId = Id });
        int lastVersionNum = lastVersionQueryRes.Value?.Num ?? 0;

        var reqFileExtractorRes = await reqFileExtractor.GetRequestData(request);

        if (!reqFileExtractorRes.IsSuccess)
            return Results.BadRequest(reqFileExtractorRes.Error);

        IFormFile? prjFile = reqFileExtractorRes.Value;
        
        string prjFilePath = Path.Combine(saveFolder, prjFile.FileName);

        var savePrjVersionRes = await mediator.Send(new SaveNewProjectVersionCommand(prjFile.OpenReadStream(), lastVersionNum + 1, prjFilePath, project));

        if (!savePrjVersionRes.IsSuccess)
            return Results.BadRequest(savePrjVersionRes.Error);

        var newVersion = savePrjVersionRes.Value;

        var saveContentFileRes = await mediator.Send(new CreateContentFileCommand() {ProjectVersion = newVersion});

        if (!saveContentFileRes.IsSuccess)
            return Results.BadRequest(saveContentFileRes.Error);

        return Results.Ok(newVersion);
    }).Accepts<IFormFile>("multipart/form-data");



app.MapGet("/project/{id}/changes",
    async (IMediator mediator, [FromRoute] Guid Id, DataContext db) =>
    {
        Project project = await db.Projects.FindAsync(Id);

        var lastVersionQueryRes = await mediator.Send(new GetLastProjectVersionQuery() { ProjectId = Id });
        int lastVersionNum = lastVersionQueryRes.Value?.Num ?? 0;

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
