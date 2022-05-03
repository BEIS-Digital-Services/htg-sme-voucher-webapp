using System.Collections.Generic;
using BEIS.HelpToGrow.Voucher.Web.Services;
using RestSharp;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    public class FakeRestClientFactory : IRestClientFactory
    {
        private readonly List<IRestResponse> _executionResponses;

        public FakeRestClientFactory()
        {
            _executionResponses = new List<IRestResponse>();
        }

        public FakeRestClientFactory(IRestResponse executionResponse) : this()
        {
            _executionResponses.Add(executionResponse);
        }

        public FakeRestClientFactory(IEnumerable<IRestResponse> executionResponses) : this()
        {
            _executionResponses.AddRange(executionResponses);
        }

        public IRestClient Create(string indesserApiUrl, int connectionTimeOut) =>
            new FakeRestClient().SetExecutionResponses(_executionResponses);

        public IRestClientFactory ResetResponses()
        {
            FakeRestClient.ResetResponseCount();

            return this;
        }
    }
}