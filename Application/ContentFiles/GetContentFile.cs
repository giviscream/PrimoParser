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

namespace Application.ContentFiles
{
    public class GetContentFileQuery : IRequest<Result<ContentFile>>
    {
        public Guid Id { get; set; }
    }

    public class GetContentFileQueryHandler : IRequestHandler<GetContentFileQuery, Result<ContentFile>>
    {
        private readonly DataContext _dataContext;

        public GetContentFileQueryHandler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<Result<ContentFile>> Handle(GetContentFileQuery request, CancellationToken cancellationToken)
        {
            ContentFile? contentFile = await _dataContext.ContentFiles.Include(x => x.ProjectVersion).FirstOrDefaultAsync(x => x.Id == request.Id);

            if (contentFile == null)
                return Result<ContentFile>.Failure($"Content file with Id = {request.Id} not found");

            return Result<ContentFile>.Success(contentFile);
        }
    }
}
