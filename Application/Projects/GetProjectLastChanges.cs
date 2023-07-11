using Application.Core;
using Application.ProjectVersions;
using Domain.ProjectHierarchy;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Projects
{
    public class GetProjectLastChangesQuery : IRequest<Result<ContentFile>>
    {
        public Guid Id { get; set; }
    }

    public class GetProjectLastChangesHandler : IRequestHandler<GetProjectLastChangesQuery, Result<ContentFile>>
    {
        private readonly DataContext _dataContext;

        public GetProjectLastChangesHandler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Result<ContentFile>> Handle(GetProjectLastChangesQuery request, CancellationToken cancellationToken)
        {
            
            Project project = await _dataContext.Projects.FindAsync(request.Id);

            if (project == null)
                return Result<ContentFile>.Failure("Project Not found");

            var lastVersion = await _dataContext.ProjectVersions.GetLastVersionAsync(request.Id);
            int lastVersionNum = lastVersion?.Num ?? 0;

            if (lastVersionNum == 0)
                return Result<ContentFile>.Failure("No data");

            if (lastVersionNum == 1)
                return Result<ContentFile>.Failure("Nothing to compare");

            ProjectVersion curVersion = await _dataContext.ProjectVersions.FirstOrDefaultAsync(x => x.Project.Id == project.Id && x.Num == lastVersionNum);

            ProjectVersion prevVersion = await _dataContext.ProjectVersions.FirstOrDefaultAsync(x => x.Project.Id == project.Id && x.Num == lastVersionNum - 1);

            ContentFile curContentFile = await _dataContext.ContentFiles.FirstOrDefaultAsync(x => x.ProjectVersion.Id == curVersion.Id);

            ContentFile prevContentFile = await _dataContext.ContentFiles.FirstOrDefaultAsync(x => x.ProjectVersion.Id == prevVersion.Id);

            ContentFile diffContent = curContentFile.GetDifferentContent(prevContentFile);
            
            return null;
        }
    }
}
