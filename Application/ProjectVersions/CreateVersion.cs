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
    public class CreateVersionCommand : IRequest<Result<Unit>>
    {
        public ProjectVersion NewVersion { get; set; }
    }

    public class CreateVersionCommandHandler : IRequestHandler<CreateVersionCommand, Result<Unit>>
    {
        private readonly DataContext _dataContext;

        public CreateVersionCommandHandler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<Result<Unit>> Handle(CreateVersionCommand request, CancellationToken cancellationToken)
        {
            await _dataContext.ProjectVersions.AddAsync(request.NewVersion);
            var success = await _dataContext.SaveChangesAsync() > 0;

            if (!success)
                return Result<Unit>.Failure("Failed to add project version");

            return Result<Unit>.Success(Unit.Value);
        }
    }

}
