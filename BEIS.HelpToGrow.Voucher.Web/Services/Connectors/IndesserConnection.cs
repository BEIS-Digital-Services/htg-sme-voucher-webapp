using Newtonsoft.Json;
using RestSharp;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Beis.HelpToGrow.Voucher.Web.Services.Connectors
{
    public class IndesserConnection : IIndesserHttpConnection<IndesserCompanyResponse>
    {
        private const string Accept = "application/json";
        private readonly string _tokenConnectionUrl;
        private readonly string _companyCheckUrl;
        private readonly IRestClientFactory _restClientFactory;
        private readonly ILogger<IndesserConnection> _logger;
        private readonly string _clientKey;
        private readonly string _clientSecret;
        private readonly int _connectionTimeOut;
        private readonly IMemoryCache _cacheService;

        public IndesserConnection(IOptions<IndesserConnectionOptions> options,
            IRestClientFactory restClientFactory,
            ILogger<IndesserConnection> logger,
            IMemoryCache memoryCache)
        {
            _tokenConnectionUrl = options.Value.TokenConnectionUrl;
            _clientKey = options.Value.ClientKey;
            _clientSecret = options.Value.ClientSecret;
            _companyCheckUrl = options.Value.CompanyCheckUrl;
            _connectionTimeOut = int.Parse(options.Value.ConnectionTimeOut);
            _restClientFactory = restClientFactory;
            _logger = logger;
            _cacheService = memoryCache;
        }

        public Result<IndesserCompanyResponse> ProcessRequest(string companyId, HttpContext httpContext)
        {
            var attempts = 0;
            const int maxAttempts = 5;
            var acceptedStatusCodes = new[] { HttpStatusCode.OK, HttpStatusCode.NoContent };

            try
            {
                var connectionToken = GetConnectionToken();

                if (connectionToken == null)
                {
                    _logger.LogError("No connection could be established to the Indesser API.");
                    return Result.Fail("No Indesser connection established");
                }

                var indesserApiUrl = $"{_companyCheckUrl}/{companyId}?characteristicId=5&financialOptions=All";

                var client = _restClientFactory.Create(indesserApiUrl, _connectionTimeOut);

                var request = new RestRequest(Method.GET);
                request.AddHeader("efx-client-correlation-id", companyId);
                request.AddHeader("Authorization", "Bearer " + connectionToken.AccessToken);

                IRestResponse response;

                do
                {
                    attempts += 1;

                    response = client.Execute(request);

                    if (!acceptedStatusCodes.Contains(response.StatusCode))
                    {
                        Thread.Sleep(1000 * 2 ^ attempts); // exponential back off
                    }

                } while (!acceptedStatusCodes.Contains(response.StatusCode) && attempts < maxAttempts);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        {

                            return Result.Ok(JsonSerializer.Deserialize<IndesserCompanyResponse>(response.Content));
                        }

                    case HttpStatusCode.NoContent:
                        {
                            _logger.LogError($"Indesser API call - no records found: {new IndesserErrorResponse(response).AsJson()}");
                            return Result.Fail($"Indesser API call - no records found: {new IndesserErrorResponse(response).AsJson()}");
                        }
                    default:
                        {
                            _logger.LogError($"Indesser API call failed: {new IndesserErrorResponse(response).AsJson()}");
                            return Result.Fail($"Indesser API call failed: {new IndesserErrorResponse(response).AsJson()}");
                        }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Indesser API request");

                return Result.Fail(ex.Message);
            }
        }

        private ConnectionToken GetConnectionToken()
        {
            var initialAttempt = GetConnectionToken(true);

            return !IsValidToken(initialAttempt)
                ? GetConnectionToken(false)
                : initialAttempt;
        }

        private ConnectionToken GetConnectionToken(bool useCachedToken)
        {
            const int maxAttempts = 5;

            var attempts = 0;
            byte[] connectionTokenBytes;

            while (attempts <= maxAttempts)
            {
                _cacheService.TryGetValue("connectionToken", out connectionTokenBytes);

                var connectionToken = useCachedToken
                    ? GetConnectionToken(connectionTokenBytes)
                    : null;

                attempts += 1;

                if (IsValidToken(connectionToken))
                {
                    return connectionToken;
                }

                if (connectionToken is { CurrentStatus: "UPDATING" })
                {
                    Thread.Sleep(1000);
                    continue;
                }

                connectionToken ??= new ConnectionToken();

                var client = _restClientFactory.Create(_tokenConnectionUrl, _connectionTimeOut);

                connectionToken = GetNewToken(connectionToken, client);

                if (IsValidToken(connectionToken))
                {
                    return connectionToken;
                }

                Thread.Sleep(1000 * 2 ^ attempts); // exponential back off
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private ConnectionToken GetNewToken(ConnectionToken connectionToken, IRestClient client)
        {
            connectionToken.CurrentStatus = "UPDATING";

            _cacheService.Set("connectionToken", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(connectionToken)));

            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", Accept);

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(_clientKey + ":" + _clientSecret));
            request.AddHeader("Authorization", "Basic " + credentials);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scope", "sa.readprofile");

            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                connectionToken = JsonSerializer.Deserialize<ConnectionToken>(response.Content);
            }

            if (connectionToken?.AccessToken == null)
            {
                return null;
            }

            connectionToken.CurrentStatus = "UPDATED";
            connectionToken.TimeAcquired = DateTime.Now;
            _cacheService.Set("connectionToken", Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(connectionToken)));

            return connectionToken;
        }

        private static ConnectionToken GetConnectionToken(byte[] connectionTokenBytes) =>
            connectionTokenBytes != null
                ? JsonConvert.DeserializeObject<ConnectionToken>(Encoding.ASCII.GetString(connectionTokenBytes))
                : null;

        private static bool IsValidToken(ConnectionToken connectionToken)
        {
            if (connectionToken == null)
            {
                return false;
            }

            var expiresIn = Convert.ToDouble(connectionToken.ExpiresIn);
            var acquiredTime = connectionToken.TimeAcquired.AddSeconds(expiresIn);
            var currentTime = DateTime.Now;
            var difference = acquiredTime - currentTime;

            return difference.TotalMinutes > 5;
        }
    }
}