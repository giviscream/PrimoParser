using Application.Core;
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
    public class GetDetailsQuery : IRequest<Result<Project?>>
    {
        public Guid Id { get; set; }
    }

    public class GetDetailsQueryHandler : IRequestHandler<GetDetailsQuery, Result<Project?>>
    {
        private readonly DataContext _dataContext;

        public GetDetailsQueryHandler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Result<Project?>> Handle(GetDetailsQuery request, CancellationToken cancellationToken)
        {
            var project = await _dataContext.Projects.FirstOrDefaultAsync(x => x.Id == request.Id);

            return Result<Project?>.Success(project);
        }
    }
}
