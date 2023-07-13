
using Domain.ProjectHierarchy;
using System.IO.Compression;
using System.Text.Json;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Application.Projects;
using Application.ProjectVersions;
using Application.ContentFiles;
using Application.Documents;
using MediatR;
using Application.Core;
using AutoMapper;
using Domain.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddDbContext<DataContext>(opt => opt.UseSqlite(@"Data Source=..\Storage\PrimoPrjsdb.db"), ServiceLifetime.Singleton);
builder.Services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IRequestDataExtractor<IFormFile>, ProjectFileExtractor>();
builder.Services.AddAutoMapper(typeof(MappingProfiles));


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
    async (IMediator mediator, 
            IRequestDataExtractor<IFormFile?> reqFileExtractor,
            IMapper _mapper,
            HttpRequest request, 
            [FromRoute]Guid Id) =>
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

        var newVersionDto = _mapper.Map<ProjectVersionDto>(newVersion);

        return Results.Ok(newVersionDto);
    }).Accepts<IFormFile>("multipart/form-data");



app.MapGet("/project/{id}/changes",
    async (IMediator mediator, [FromRoute] Guid Id) =>
    {
        var diffContentQueryRes = await mediator.Send(new GetProjectLastChangesQuery() { Id = Id });

        if (!diffContentQueryRes.IsSuccess)
            return Results.BadRequest(diffContentQueryRes.Error);

        return Results.Ok(diffContentQueryRes.Value);

    });

app.MapGet("/document/{id}",
    async (IMediator mediator, [FromRoute] Guid Id) =>
    {
        var docQueryRes = await mediator.Send(new GetDocumentQuery() { Id = Id });

        if (!docQueryRes.IsSuccess)
            return Results.BadRequest(docQueryRes.Error);

        return Results.Ok(docQueryRes.Value);
    });

//app.MapGet("document/{id}/changes",
//    async([FromRoute] Guid Id, )
//    )

//app.MapGet("/version/{versionId}/ltw", 
//    async([FromRoute] Guid Id
//        , [FromQuery(Name ="Id")] Guid ltwId
//        , DataContext db) =>
//    {
        
//    });

app.Run();
