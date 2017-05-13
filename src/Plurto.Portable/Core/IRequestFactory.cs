using Plurto.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plurto.Core
{
    public interface IRequestFactory
    {
        IRequestFactory AddAuthorizationHeader(Func<string> authBuilder);
        Task<ResponseData> GetResponseAsync();
        Task<ResponseData> GetPostResponseAsync(IEnumerable<QueryParameter> postForm);
        Task<ResponseData> GetPostMultipartResponseAsync(IEnumerable<QueryParameter> postForm, UploadFile uploadFile);
    }
}
