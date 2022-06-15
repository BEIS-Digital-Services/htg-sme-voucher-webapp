using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using RestSharp.Serialization;

namespace Beis.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    public class FakeRestClient : IRestClient
    {
        private static int _responseCount;
        private readonly List<IRestResponse> _executionResponses = new();

        public static void ResetResponseCount()
        {
            _responseCount = 0;
        }

        public IRestClient UseSerializer(Func<IRestSerializer> serializerFactory)
        {
            throw new NotImplementedException();
        }

        public IRestClient UseSerializer<T>() where T : IRestSerializer, new()
        {
            throw new NotImplementedException();
        }

        public IRestResponse<T> Deserialize<T>(IRestResponse response)
        {
            throw new NotImplementedException();
        }

        public IRestClient UseUrlEncoder(Func<string, string> encoder)
        {
            throw new NotImplementedException();
        }

        public IRestClient UseQueryEncoder(Func<string, Encoding, string> queryEncoder)
        {
            throw new NotImplementedException();
        }

        public IRestClient SetExecutionResponses(IEnumerable<IRestResponse> responses)
        {
            _executionResponses.AddRange(responses);

            return this;
        }

        public IRestResponse Execute(IRestRequest request)
        {
            return _executionResponses.Skip(_responseCount++).Take(1).Single();
        }

        public IRestResponse Execute(IRestRequest request, Method httpMethod)
        {
            throw new NotImplementedException();
        }

        public IRestResponse<T> Execute<T>(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public IRestResponse<T> Execute<T>(IRestRequest request, Method httpMethod)
        {
            throw new NotImplementedException();
        }

        public byte[] DownloadData(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public byte[] DownloadData(IRestRequest request, bool throwOnError)
        {
            throw new NotImplementedException();
        }

        public Uri BuildUri(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public string BuildUriWithoutQueryParameters(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public void ConfigureWebRequest(Action<HttpWebRequest> configurator)
        {
            throw new NotImplementedException();
        }

        public void AddHandler(string contentType, Func<IDeserializer> deserializerFactory)
        {
            throw new NotImplementedException();
        }

        public void RemoveHandler(string contentType)
        {
            throw new NotImplementedException();
        }

        public void ClearHandlers()
        {
            throw new NotImplementedException();
        }

        public IRestResponse ExecuteAsGet(IRestRequest request, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public IRestResponse ExecuteAsPost(IRestRequest request, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public IRestResponse<T> ExecuteAsGet<T>(IRestRequest request, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public IRestResponse<T> ExecuteAsPost<T>(IRestRequest request, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request, CancellationToken cancellationToken = new())
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request, Method httpMethod,
            CancellationToken cancellationToken = new())
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteAsync(IRestRequest request, Method httpMethod,
            CancellationToken cancellationToken = new())
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteAsync(IRestRequest request, CancellationToken cancellationToken = new())
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteGetAsync<T>(IRestRequest request, CancellationToken cancellationToken = new())
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecutePostAsync<T>(IRestRequest request, CancellationToken cancellationToken = new())
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteGetAsync(IRestRequest request, CancellationToken cancellationToken = new())
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecutePostAsync(IRestRequest request, CancellationToken cancellationToken = new())
        {
            throw new NotImplementedException();
        }

        public IRestClient UseSerializer(IRestSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsync(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsync<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsync(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, Method httpMethod)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsync<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, Method httpMethod)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsyncGet(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsyncPost(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsyncGet<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public RestRequestAsyncHandle ExecuteAsyncPost<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback, string httpMethod)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request, Method httpMethod)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecuteGetTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse<T>> ExecutePostTaskAsync<T>(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteTaskAsync(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteTaskAsync(IRestRequest request, CancellationToken token, Method httpMethod)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteTaskAsync(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteGetTaskAsync(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecuteGetTaskAsync(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecutePostTaskAsync(IRestRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IRestResponse> ExecutePostTaskAsync(IRestRequest request, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public void AddHandler(string contentType, IDeserializer deserializer)
        {
            throw new NotImplementedException();
        }

        public CookieContainer CookieContainer { get; set; }
        public bool AutomaticDecompression { get; set; }
        public int? MaxRedirects { get; set; }
        public string UserAgent { get; set; }
        public int Timeout { get; set; }
        public int ReadWriteTimeout { get; set; }
        public bool UseSynchronizationContext { get; set; }
        public IAuthenticator Authenticator { get; set; }
        public Uri BaseUrl { get; set; }
        public Encoding Encoding { get; set; }
        public bool ThrowOnDeserializationError { get; set; }
        public bool FailOnDeserializationError { get; set; }
        public bool ThrowOnAnyError { get; set; }
        public string ConnectionGroupName { get; set; }
        public bool PreAuthenticate { get; set; }
        public bool UnsafeAuthenticatedConnectionSharing { get; set; }
        [Obsolete] public IList<Parameter> DefaultParameters { get; } = new List<Parameter>();
        public string BaseHost { get; set; }
        public bool AllowMultipleDefaultParametersWithSameName { get; set; }
        public X509CertificateCollection ClientCertificates { get; set; }
        public IWebProxy Proxy { get; set; }
        public RequestCachePolicy CachePolicy { get; set; }
        public bool Pipelined { get; set; }
        public bool FollowRedirects { get; set; }
        public RemoteCertificateValidationCallback RemoteCertificateValidationCallback { get; set; }
    }
}