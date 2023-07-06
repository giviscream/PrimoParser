using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Core
{
    public interface IRequestDataExtractor<T>
    {
        Task<Result<T>> GetRequestData(HttpRequest request);
    }
}
