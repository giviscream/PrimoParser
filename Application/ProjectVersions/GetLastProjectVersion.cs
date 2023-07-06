using Application.Core;
using Domain.ProjectHierarchy;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ProjectVersions
{
    public class GetLastProjectVersionQuery : IRequest<Result<ProjectVersion?>>
    {
        public Guid ProjectId { get; set; }
    }

    public class GetLastProjectVersionQueryHandler : IRequestHandler<GetLastProjectVersionQuery, Result<ProjectVersion?>>
    {
        private readonly DataContext _dataContext;

        public GetLastProjectVersionQueryHandler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<Result<ProjectVersion?>> Handle(GetLastProjectVersionQuery request, CancellationToken cancellationToken)
        {
            var lastVersion = await Task<int>.Run(() =>
            {
                var pVersions = _dataContext.ProjectVersions?.Where(x => x.Project.Id == request.ProjectId);

                return pVersions?.OrderByDescending(x => x.DateTime).FirstOrDefault();
            });

            return Result<ProjectVersion?>.Success(lastVersion);
        }
    }
}
