using Application.Core;
using Domain.Analyzers;
using Domain.Documents;
using Domain.DocumentsChanges;
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
    public class GetDocumentChangesQuery : IRequest<Result<DocumentChanges>>
    {
        public Guid DocumentId { get; set; }
    }

    public class GetDocumentChangesQueryHandle : IRequestHandler<GetDocumentChangesQuery, Result<DocumentChanges>>
    {
        private readonly DataContext _dataContext;

        public GetDocumentChangesQueryHandle(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<Result<DocumentChanges>> Handle(GetDocumentChangesQuery request, CancellationToken cancellationToken)
        {

            ContentFile? contentFile = await _dataContext.ContentFiles
                                                .Include(x => x.ProjectVersion)
                                                .Include(x => x.ProjectVersion.Project)
                                                .FirstOrDefaultAsync(x => x.Id == request.DocumentId);

            if (contentFile == null)
                return Result<DocumentChanges>.Failure($"Content file with Id = {request.DocumentId} not found");

            if (contentFile.ProjectVersion.Num < 2)
                return Result<DocumentChanges>.Failure($"Content file with Id = {request.DocumentId} is Original");

            Document document = Document.LoadFromXml(contentFile.FullPath);

            ProjectVersion projectVersion = contentFile.ProjectVersion;
            ContentFile? contentFilePrevVersion = await _dataContext
                                                        .ContentFiles
                                                        .Include(x => x.ProjectVersion)
                                                        .Include(x => x.ProjectVersion.Project)
                                                        .FirstOrDefaultAsync(x =>
                                                            x.ProjectVersion.Project.Id == projectVersion.Project.Id
                                                            && x.ProjectVersion.Num == projectVersion.Num - 1
                                                            && x.Path == contentFile.Path);

            if (contentFilePrevVersion == null)
                return Result<DocumentChanges>.Failure($"Previous version of Content file with Id = {request.DocumentId} Not found");

            Document documentPrevVersion = Document.LoadFromXml(contentFilePrevVersion.FullPath);

            DocumentChanges documentChanges = new DocumentChangesAnalyzer().GetChanges(document, documentPrevVersion);

            return Result<DocumentChanges>.Success(documentChanges);
        }
    }
}
