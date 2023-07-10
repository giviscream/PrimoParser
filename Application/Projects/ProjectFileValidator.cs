using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Projects
{
    public class ProjectFileExtractor : IRequestDataExtractor<IFormFile>
    {
        async Task<Result<IFormFile>> IRequestDataExtractor<IFormFile>.GetRequestData(HttpRequest request)
        {
            if (!request.HasFormContentType)
                return Result<IFormFile>.Failure("Not a form content type");

            var form = await request.ReadFormAsync();

            var prjFile = form.Files?.FirstOrDefault(x => x.Name == "ProjectName");

            if (prjFile == null)
                return Result<IFormFile>.Failure("There is no project file");

            if (prjFile.Length == 0)
                return Result<IFormFile>.Failure("File cannot be empty");

            if (prjFile.ContentType != "application/zip" && prjFile.ContentType != "application/x-zip-compressed")
                return Result<IFormFile>.Failure("Incorrect project file. Waiting for .ZIP");

            return Result<IFormFile>.Success(prjFile);
            
        }
    }
}
