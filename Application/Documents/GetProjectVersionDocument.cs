using Application.Core;
using Domain.Documents;
using Domain.ProjectHierarchy;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Documents
{
    public class GetProjectVersionDocumentQuery : IRequest<Result<Document>>
    {
        public Guid ProjectId { get; set; }
        public int VersionNum { get; set; }
        public string FilePath { get; set; }
    }

    public class GetProjectVersionDocumentQueryHandler : IRequestHandler<GetProjectVersionDocumentQuery, Result<Document>>
    {
        private readonly DataContext _dataContext;

        public GetProjectVersionDocumentQueryHandler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<Result<Document>> Handle(GetProjectVersionDocumentQuery request, CancellationToken cancellationToken)
        {
            ContentFile? contentFile = await _dataContext
                                                .ContentFiles
                                                .FirstOrDefaultAsync(x => 
                                                    x.ProjectVersion.Project.Id == request.ProjectId
                                                    && x.ProjectVersion.Num == request.VersionNum
                                                    && x.ProjectVersion.Path == request.FilePath);

            if (contentFile == null)
                return Result<Document>.Failure($"Content file '{request.FilePath}' of version {request.VersionNum} not found");

            Document document = Document.LoadFromXml(contentFile.FullPath);

            return Result<Document>.Success(document);
        }
    }
}
