using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plurto.Core;
using Plurto.Exceptions;
using System.Threading.Tasks;

namespace Plurto.Commands
{
    public interface ICommandBase
    {
        HttpVerb RecommandHttpVerb { get; }
        string Method { get; }
        List<QueryParameter> Parameters { get; }
        UploadFile UploadFile { get; set; }
        void AddParameter(string key, object value);
    }

    public sealed class CommandBase<T> : CommandBase
    {
        internal CommandBase(HttpVerb httpVerb, string method)
            : base(httpVerb, method)
        {
        }

        internal Func<ResponseData, T> Deserializer { get; set; }

        public PlurkCommand<T> Client(IRequestClient client)
        {
            Func<ResponseData, T> rethrowDeserializer = data =>
            {
                try
                {
                    return Deserializer(data);
                }
                catch (JsonSerializationException e)
                {
                    throw new PlurtoJsonSerializationException(data, e);
                }
            };
            return new PlurkCommand<T>(client, this, rethrowDeserializer, Secure);
        }
    }

    public class CommandBase : ICommandBase
    {
        public HttpVerb RecommandHttpVerb { get; private set; }
        public string Method { get; private set; }
        public List<QueryParameter> Parameters { get; private set; }
        public UploadFile UploadFile { get; set; }

        public bool? Secure { get; set; }

        public CommandBase(string method) : this (HttpVerb.Get, method)
        {
        }

        public CommandBase(HttpVerb recommandHttpVerb, string method)
        {
            RecommandHttpVerb = recommandHttpVerb;
            Method = method;
        }

        public void AddParameter(string key, object value)
        {
            if (value != null)
            {
                AddParameter(new QueryParameter(key, value.ToString()));
            }
        }

        public void AddParameter(string key, string value)
        {
            AddParameter(new QueryParameter(key, value));
        }

        public void AddParameter(QueryParameter parameter)
        {
            if (Parameters == null)
            {
                Parameters = new List<QueryParameter>();
            }
            Parameters.Add(parameter);
        }

        /// <summary>
        /// Set HTTPS request.
        /// </summary>
        /// <param name="secure">Default is null.</param>
        public void SetSecure(bool? secure)
        {
            Secure = secure;
        }
    }

    public class PlurkCommand<T> : PlurkCommand
    {
        private readonly Func<ResponseData, T> _deserializer;

        public PlurkCommand(IRequestClient client, CommandBase commandBase, Func<ResponseData, T> deserializer, bool? secure)
            : base(client, commandBase, secure)
        {
            _deserializer = deserializer;
        }

        public async Task<WebResult<T>> LoadWebResultAsync()
        {
            if (_deserializer == null)
            {
                throw new NotSupportedException();
            }
            var data = await GetResponseAsync();
            if (data.IsSuccessStatusCode)
            {
                return WebResult.Create(_deserializer(data));
            }
            else
            {
                var exception = new RequestFailException(data);
                return WebResult.CreateError<T>(exception);
            }
        }

        public async Task<T> LoadAsync()
        {
            var webResult = await LoadWebResultAsync();
            if (webResult.Error != null)
            {
                throw webResult.Error;
            }
            return webResult.Result;
        }

        public T Load()
        {
            if (Config.AlwaysThrowOnSynchronousWebRequest)
            {
                throw new SynchronousWebRequestException();
            }
            if (_deserializer == null)
            {
                throw new NotSupportedException();
            }
            
            var task = LoadAsync();
            task.Wait();

            if (task.Status == TaskStatus.RanToCompletion)
            {
                // Task completed sucessfully.
                return task.Result;
            }

            // Task failed.
            throw task.Exception;
        }
    }

    public class PlurkCommand
    {
        private readonly IRequestClient _client;
        private readonly CommandBase _commandBase;
        private readonly bool? _secure;

        public PlurkCommand(IRequestClient client,
                            CommandBase commandBase,
                            bool? secure)
        {
            _client = client;
            _commandBase = commandBase;
            _secure = secure;
        }

        public async Task<ResponseData> GetResponseAsync()
        {
            return await _client.GetResponseAsync(_commandBase.RecommandHttpVerb, _secure, _commandBase.Method,
                                                 _commandBase.Parameters, _commandBase.UploadFile);
        }

        public ResponseData GetResponse()
        {
            var task = GetResponseAsync();
            task.Wait();
            return task.Result;
        }

        public string GetResponseBody()
        {
            return GetResponse().Body;
        }

        public JObject GetJsonResponse()
        {
            return JObject.Parse(GetResponseBody());
        }
    }

    public static class CommandBaseExtensions
    {
        public static PlurkCommand Client(this CommandBase commandBase, IRequestClient client)
        {
            return new PlurkCommand(client, commandBase, commandBase.Secure);
        }

        public static PlurkCommand<T> Client<T>(this CommandBase<T> commandBase, IRequestClient client)
        {
            return new PlurkCommand<T>(client, commandBase, commandBase.Deserializer, commandBase.Secure);
        }

        internal static void SetDefaultJsonDeserializer<T>(this CommandBase<T> command)
        {
            command.Deserializer = response => JsonConvert.DeserializeObject<T>(response.Body);
        }

        internal static void SetSuccessTextDeserializer(this CommandBase<bool> command)
        {
            command.Deserializer = response => response.Body.Contains("\"ok\"");
        }

        internal static void SetJTokenDeserializer(this CommandBase<JToken> command)
        {
            command.Deserializer = response => JToken.Parse(response.Body);
        }
    }
}
