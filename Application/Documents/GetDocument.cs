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
    public class GetDocumentQuery : IRequest<Result<Document>>
    {
        public Guid Id { get; set; }
    }

    public class GetDocumentQueryHandler : IRequestHandler<GetDocumentQuery, Result<Document>>
    {
        private readonly DataContext _dataContext;

        public GetDocumentQueryHandler(DataContext dbContext)
        {
            this._dataContext = dbContext;
        }
        public async Task<Result<Document>> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
        {
            ContentFile? contentFile = await _dataContext.ContentFiles
                                            .Include(pV => pV.ProjectVersion)
                                            .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (contentFile == null)
                return Result<Document>.Failure($"Content file  Id={request.Id} Not found");

            Document doc = Document.LoadFromXml(contentFile.FullPath);

            return Result<Document>.Success(doc);
        }
    }
}
