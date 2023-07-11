using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.ProjectHierarchy;
using Persistence;
using Application.Core;

namespace Application.Projects
{
    public class CreateProjectCommand : IRequest<Result<Project>>
    {
        public ProjectCreateDto Project { get; set; }
    }

    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Result<Project>>
    {
        private readonly DataContext _dataContext;

        public CreateProjectCommandHandler(DataContext dbContext)
        {
            this._dataContext = dbContext;
        }

        public async Task<Result<Project>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            Project newProject = new Project()
            {
                Name = request.Project.Name,
                DateTime = DateTime.Now
            };

            await _dataContext.Projects.AddAsync(newProject);
            bool succeed = await _dataContext.SaveChangesAsync() > 0;

            if (!succeed)
                return Result<Project>.Failure("Failed to create project");

            return Result<Project>.Success(newProject);
        }

    }
}
