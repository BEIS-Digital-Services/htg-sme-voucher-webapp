using Newtonsoft.Json;
using RestSharp;

namespace Beis.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    [TestFixture]
    public class IndesserConnectionTests
    {
        private string _tokenConnectionUrl;
        private string _companyCheckUrl;
        private FakeLogger _fakeLogger;
        private string _clientKey;
        private string _clientSecret;
        private string _connectionTimeOut;
        private IRestClientFactory _fakeRestClientFactory;
        private static IMemoryCache _fakeIMemoryCache;
        private IOptions<IndesserConnectionOptions> _fakeOptions;
        private string _companyId;

        [SetUp]
        public void Setup()
        {
            _fakeIMemoryCache = new MemoryCache(new MemoryCacheOptions());
            _fakeRestClientFactory = new FakeRestClientFactory().ResetResponses();
            _fakeLogger = new FakeLogger();
            _tokenConnectionUrl = "fakeTokenConnectionUrl";
            _companyCheckUrl = "fakeCompanyCheckUrl";
            _clientKey = "fakeClientKey";
            _clientSecret = "fakeClientSecret";
            _connectionTimeOut = "-1";
            _companyId = "fakeCompanyId";
            _fakeOptions = Options.Create<IndesserConnectionOptions>(new IndesserConnectionOptions
            {
                IndesserTokenUrl = _tokenConnectionUrl,
                IndesserClientId = _clientKey,
                IndesserClientSecret = _clientSecret,
                IndesserCompanyCheckUrl = _companyCheckUrl,
                IndesserConnectionTimeOut = _connectionTimeOut
            });
        }

        [Test]
        public void ProcessRequestHandlesException()
        {
            const string errorMessage = "#Fail";
            var exception = new ApplicationException(errorMessage);

            var mockRestClientFactory = new Mock<IRestClientFactory>();
            mockRestClientFactory.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<int>()))
                .Throws(exception);

            var logger = MockLogger
                            .Factory(_fakeLogger)
                            .Object
                            .CreateLogger<IndesserConnection>();

            var connection = new IndesserConnection(
                _fakeOptions,
                mockRestClientFactory.Object,
                logger, _fakeIMemoryCache);

            var result = connection.ProcessRequest(_companyId, new DefaultHttpContext());

            Assert.That(result.IsFailed);
            Assert.AreEqual(errorMessage, result.Errors.Single().Message);            
            Assert.That(FakeLogger.LogErrorCalled);
        }

        [Test]
        public void HandlesConnectionError()
        {
            var tokenBytes = GetInvalidTokenBytes();
            var responses = GetOnlyBadResponses();

            _fakeIMemoryCache.Set("connectionToken", tokenBytes);

            _fakeRestClientFactory = new FakeRestClientFactory(responses).ResetResponses();

            var logger = MockLogger.Factory(_fakeLogger).Object.CreateLogger<IndesserConnection>();

            var connection = new IndesserConnection(
                _fakeOptions,
                _fakeRestClientFactory,
                logger, _fakeIMemoryCache);

            var result = connection.ProcessRequest(_companyId, new DefaultHttpContext());

            Assert.That(result.IsFailed);
            Assert.AreEqual("No Indesser connection established", result.Errors.Single().Message);
        }

        [Test]
        public void HandlesTokenCacheMissAndRepeatedReadCompanyDataAttemptsUntilSuccess()
        {
            var invalidCachedToken = GetInvalidTokenBytes();
            var responses = GetMultipleFailuresAttemptingToReadCompanyDataWithSuccess();
            
            _fakeIMemoryCache.Set("connectionToken", invalidCachedToken);
            _fakeRestClientFactory = new FakeRestClientFactory(responses).ResetResponses();

            var logger = MockLogger.Factory(_fakeLogger).Object.CreateLogger<IndesserConnection>();

            var connection = new IndesserConnection(
                _fakeOptions,
                _fakeRestClientFactory,
                logger, _fakeIMemoryCache);

            var result = connection.ProcessRequest(_companyId, new DefaultHttpContext());

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void HandlesTokenCacheMissAndRepeatedReadCompanyDataAttemptsWithoutSuccess()
        {
            var tokenBytes = GetValidTokenBytes();
            var responses = GetMultipleFailuresAttemptingToReadCompanyDataWithFailure();

            _fakeIMemoryCache.Set("connectionToken", tokenBytes);
            _fakeRestClientFactory = new FakeRestClientFactory(responses).ResetResponses();

            var logger = MockLogger.Factory(_fakeLogger).Object.CreateLogger<IndesserConnection>();

            var connection = new IndesserConnection(
                _fakeOptions,
                _fakeRestClientFactory,
                logger, _fakeIMemoryCache);

            var result = connection.ProcessRequest(_companyId, new DefaultHttpContext());

            Assert.That(result.IsFailed);
            Assert.That(result.Errors.Single().Message.Contains("Indesser API call failed"));
            Assert.That(result.Errors.Single().Message.Contains(((int)HttpStatusCode.BadGateway).ToString()));
        }

        [Test]
        public void HandlesTokenCacheMissTokenApiRefreshFailureAndMultipleCompanyApiFailures()
        {
            var responses = new RestResponse[]
            {
                new() { StatusCode = HttpStatusCode.BadGateway, ErrorMessage = "#FakeErrorMessage 1" },
                new() { StatusCode = HttpStatusCode.OK, Content = GetValidTokenContent() },
                new() { StatusCode = HttpStatusCode.BadGateway, ErrorMessage = "#FakeErrorMessage 2" },
                new() { StatusCode = HttpStatusCode.BadGateway, ErrorMessage = "#FakeErrorMessage 3" },
                new() { StatusCode = HttpStatusCode.BadGateway, ErrorMessage = "#FakeErrorMessage 4" },
                new() { StatusCode = HttpStatusCode.BadGateway, ErrorMessage = "#FakeErrorMessage 5" },
                new() { StatusCode = HttpStatusCode.OK, Content = GetValidCompanyResponse() }
            };

            _fakeRestClientFactory = new FakeRestClientFactory(responses).ResetResponses();

            var logger = MockLogger.Factory(_fakeLogger).Object.CreateLogger<IndesserConnection>();

            var connection = new IndesserConnection(
                _fakeOptions,
                _fakeRestClientFactory,
                logger, _fakeIMemoryCache);

            var result = connection.ProcessRequest(_companyId, new DefaultHttpContext());

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void ReturnsValidResponse()
        {
            var tokenBytes = GetValidTokenBytes();
            var expectedResponse = GetValidCompanyResponse();

            var response = new RestResponse
            {
                StatusCode = HttpStatusCode.OK,
                Content = expectedResponse
            };

            _fakeIMemoryCache.Set("connectionToken", tokenBytes);
            _fakeRestClientFactory = new FakeRestClientFactory(response).ResetResponses();

            var logger = MockLogger.Factory(_fakeLogger).Object.CreateLogger<IndesserConnection>();

            var connection = new IndesserConnection(
                _fakeOptions,
                _fakeRestClientFactory,
                logger, _fakeIMemoryCache);

            var result = connection.ProcessRequest(_companyId, new DefaultHttpContext());

            Assert.That(result.IsSuccess);
            Assert.AreEqual("fakeCompanyNumber", result.Value.Identification.companyNumber);
            Assert.AreEqual("fakeCompany", result.Value.Identification.name);
            Assert.AreEqual("fakeKey", result.Value.Characteristics.Single().Name);
            Assert.AreEqual("fakeValue", result.Value.Characteristics.Single().Value);
            Assert.AreEqual("fakeCompany", result.Value.Identification.name);
            Assert.AreEqual("1 Victoria St", result.Value.Identification.RegisteredOffice.line1);
            Assert.AreEqual("SW1H 0ET", result.Value.Identification.RegisteredOffice.postcode);
            Assert.AreEqual("X", result.Value.ScoresAndLimits.ScoreGrade);
            Assert.AreEqual(10, result.Value.ScoresAndLimits.ProtectScore);
        }

        private static IEnumerable<RestResponse> GetOnlyBadResponses()
        {
            var responses = new List<RestResponse>();

            while (responses.Count < 10)
            {
                responses.Add(new RestResponse
                {
                    StatusCode = HttpStatusCode.BadGateway,
                    ErrorMessage = $"#FakeErrorMessage {responses.Count + 1}"
                });
            }

            return responses;
        }

        private static IEnumerable<IRestResponse> GetMultipleFailuresAttemptingToReadCompanyDataWithSuccess() =>
            new List<IRestResponse>
            {
                new RestResponse { StatusCode = HttpStatusCode.OK, Content = GetValidTokenContent()},
                new RestResponse { StatusCode = HttpStatusCode.BadGateway },
                new RestResponse { StatusCode = HttpStatusCode.BadGateway },
                new RestResponse { StatusCode = HttpStatusCode.BadGateway },
                new RestResponse { StatusCode = HttpStatusCode.BadGateway },
                new RestResponse { StatusCode = HttpStatusCode.OK, Content = GetValidCompanyResponse() }
            };

        private static IEnumerable<IRestResponse> GetMultipleFailuresAttemptingToReadCompanyDataWithFailure() =>
            new List<IRestResponse>
            {
                new RestResponse { StatusCode = HttpStatusCode.BadGateway },
                new RestResponse { StatusCode = HttpStatusCode.BadGateway },
                new RestResponse { StatusCode = HttpStatusCode.BadGateway },
                new RestResponse { StatusCode = HttpStatusCode.BadGateway },
                new RestResponse { StatusCode = HttpStatusCode.BadGateway }
            };

        private static string GetValidCompanyResponse() => @"
        {
          ""identification"": {
            ""registeredOffice"": {
              ""line1"": ""1 Victoria St"",
              ""line2"": null,
              ""line3"": null,
              ""line4"": null,
              ""postcode"": ""SW1H 0ET""
            },
            ""companyNumber"": ""fakeCompanyNumber"",
            ""name"": ""fakeCompany""
          },
          ""characteristics"": [
            {
              ""name"": ""fakeKey"",
              ""value"": ""fakeValue""
            }
          ],
          ""scoresAndLimits"": {
            ""scoreGrade"": ""X"",
            ""protectScore"": 10
          },
          ""financials"": [
            {
              ""financialData"": {
                ""bankOverdraft"": 0,
                ""capitalRedemptionReserves"": 0,
                ""capitalReserves"": 0,
                ""creditLimit"": 0,
                ""cashAndEquivalent"": 0,
                ""contingentLiability"": 0,
                ""currentAssetsInterCompanyBalances"": 0,
                ""currentLiabilitiesBankLoans"": 0,
                ""currentLiabilitiesBankLoansOrOverdraft"": 0,
                ""currentLiabilitiesDirectorsLoans"": 0,
                ""currentLiabilitiesFinanceLease"": 0,
                ""currentLiabilitiesFinanceOrHirePurchase"": 0,
                ""currentLiabilitiesHirePurchase"": 0,
                ""currentLiabilitiesInterCompanyBalances"": 0,
                ""depreciation"": 0,
                ""directorsFees"": 0,
                ""dividends"": 0,
                ""exceptionalCharges"": 0,
                ""exceptionalIncome"": 0,
                ""exports"": 0,
                ""goodwillReserves"": 0,
                ""grossProfit"": 0,
                ""increaseInCash"": 0,
                ""intangibleAssets"": 0,
                ""interestPaid"": 0,
                ""interestReceivable"": 0,
                ""investments"": 0,
                ""longTermDirectorsLoans"": 0,
                ""longTermLiabilitiesBankLoans"": 0,
                ""longTermLiabilitiesFinanceLeaseObligation"": 0,
                ""longTermLiabilitiesFinanceLeaseOrHirePurchase"": 0,
                ""longTermLiabilitiesHirePurchaseLoan"": 0,
                ""longTermLiabilitiesOwedToGroupCompanies"": 0,
                ""netCashflowBeforeFinancing"": 0,
                ""netCashflowFromFinancing"": 0,
                ""netCashflowFromOperatingActivities"": 0,
                ""netCashflowFromROIAndServicing"": 0,
                ""NumberofEmployees"": 5,
                ""operatingProfit"": 0,
                ""otherCurrentAssets"": 0,
                ""otherCurrentLiabilities"": 0,
                ""otherLongTermBorrowing"": 0,
                ""otherLongTermLiabilities"": 0,
                ""otherShortTermBorrowing"": 0,
                ""otherReserves"": 0,
                ""paidUpEquity"": 0,
                ""profitAfterTax"": 0,
                ""profitAndLossAccount"": 0,
                ""profitBeforeTax"": 0,
                ""retainedEarnings"": 0,
                ""retainedProfits"": 0,
                ""revaluationReserves"": 0,
                ""shareholdersFunds"": 0,
                ""sharePremiumAccount"": 0,
                ""stock"": 0,
                ""tangibleFixedAssets"": 0,
                ""taxation"": 0,
                ""tangibleNetWorth"": 0,
                ""totalAssets"": 0,
                ""totalCurrentAssets"": 0,
                ""totalCurrentLiabilities"": 0,
                ""totalFixedAssets"": 0,
                ""totalLiabilities"": 0,
                ""totalLongTermBorrowings"": 0,
                ""totalLongTermLiabilities"": 0,
                ""totalOtherLongTermLiabilities"": 0,
                ""totalReserves"": 0,
                ""totalShortTermBorrowings"": 0,
                ""tradeCreditors"": 0,
                ""tradeDebtors"": 0,
                ""turnover"": 0,
                ""wages"": 0,
                ""workingCapital"": 0
              }
            }
          ]
        }";

        private static byte[] GetValidTokenBytes()
        {
            var validTokenContent = GetValidTokenContent();
            return Encoding.ASCII.GetBytes(validTokenContent);
        }

        private static byte[] GetInvalidTokenBytes()
        {
            return Encoding.ASCII.GetBytes(string.Empty);
        }

        private static string GetValidTokenContent()
        {
            dynamic obj = new JObject();
            obj.accessToken = "fakeAccessToken";
            obj.access_token = "fakeAccessToken";
            obj.currentStatus = "FAKE STATUS";
            obj.expiresIn = 18000;
            obj.expires_in = 18000;
            obj.timeAcquired = DateTime.Now;

            return JsonConvert.SerializeObject(obj);
        }
    }
}