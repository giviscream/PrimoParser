using Application.Core;
using Domain.ProjectHierarchy;
using MediatR;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ContentFiles
{
    public class CreateContentFileCommand : IRequest<Result<Unit>>
    {
        public ProjectVersion ProjectVersion { get; set; }
    }

    public class CreateContentFileCommandHandler : IRequestHandler<CreateContentFileCommand, Result<Unit>>
    {
        private readonly DataContext _dataContext;

        public CreateContentFileCommandHandler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Result<Unit>> Handle(CreateContentFileCommand request, CancellationToken cancellationToken)
        {
            ContentFile contentFile = new ContentFile(request.ProjectVersion);

            _dataContext.ContentFiles.Add(contentFile);
            bool succeed = await _dataContext.SaveChangesAsync() > 0;

            if (!succeed)
                return Result<Unit>.Failure("Failed to create content file");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
