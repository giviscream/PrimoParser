using Application.Core;
using Domain.ProjectHierarchy;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.ProjectVersions
{
    public class SaveNewProjectVersionCommand : IRequest<Result<ProjectVersion>>
    {
        public Stream NewVersionFileStream { get; private set; }
        public int VersionNum { get; private set; }
        public string FilePath { get; private set; }
        public Project Project { get; private set; }
        public SaveNewProjectVersionCommand(Stream newVersionFileStream, int versionNum, string folderPath, Project project)
        {
            NewVersionFileStream = newVersionFileStream;
            VersionNum = versionNum;
            FilePath = folderPath;
            Project = project;
        }
    }

    public class SaveNewProjectVersionCommandHandler : IRequestHandler<SaveNewProjectVersionCommand, Result<ProjectVersion>>
    {
        private readonly DataContext _dataContext;

        public SaveNewProjectVersionCommandHandler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<Result<ProjectVersion>> Handle(SaveNewProjectVersionCommand request, CancellationToken cancellationToken)
        {
            ProjectVersion newVersion = null;
            try
            {
                using (FileStream prjFileStream = new FileStream(request.FilePath, FileMode.Create))
                {
                    await request.NewVersionFileStream.CopyToAsync(prjFileStream);
                }

                string saveFolderPath = Path.Combine(Path.GetDirectoryName(request.FilePath), Guid.NewGuid().ToString(), Path.GetFileNameWithoutExtension(request.FilePath));
                await Task.Run(() => ZipFile.ExtractToDirectory(request.FilePath, saveFolderPath));

                newVersion = new ProjectVersion()
                {
                    Num = request.VersionNum,
                    Path = saveFolderPath,
                    DateTime = DateTime.Now,
                    Project = request.Project
                };

                string prjHierarchyStructFilePath = Path.Combine(saveFolderPath, "primo_hierarchy.json");

                using (FileStream prjStructStream = new FileStream(prjHierarchyStructFilePath, FileMode.Create))
                {
                    await JsonSerializer.SerializeAsync<ProjectVersion>(prjStructStream, newVersion);
                }
            }
            catch (Exception ex) 
            {
                return Result<ProjectVersion>.Failure($"Failed to save project version. Error = {ex.Message}");
            }
            
            
            await _dataContext.ProjectVersions.AddAsync(newVersion);
            var success = await _dataContext.SaveChangesAsync() > 0;

            if (!success)
                return Result<ProjectVersion>.Failure("Failed to add project version");

            return Result<ProjectVersion>.Success(newVersion);
            
        }
    }

}
