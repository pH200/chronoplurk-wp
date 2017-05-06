// -----------------------------------------------------------------------
// <copyright file="RequestClient.cs" company="Ting-Yu Lin">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Plurto.Commands;
using System.Threading.Tasks;

namespace Plurto.Core
{
    public interface IRequestClient
    {
        Task<ResponseData> GetResponseAsync(HttpVerb recommandHttpVerb, bool? secure, string method, IEnumerable<QueryParameter> parameters, UploadFile uploadFile);
    }

    public static class RequestClientExtensions
    {
        public static PlurkCommand Request(this IRequestClient client, CommandBase commandBase)
        {
            return commandBase.Client(client);
        }

        public static PlurkCommand<T> Request<T>(this IRequestClient client, CommandBase<T> commandBase)
        {
            return commandBase.Client(client);
        }
    }
}
