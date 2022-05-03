using System.Collections.Generic;
using System.Net;
using BEIS.HelpToGrow.Voucher.Web.Tests.Eligibility;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount.Services
{
    [TestFixture]
    public class CompanyHouseConnectionTests
    {
        [SetUp]
        public void Setup()
        {
            FakeRestClient.ResetResponseCount();
        }

        [Test]
        public void NotFound()
        {
            var responses = new List<IRestResponse> {new RestResponse()};
            var fakeRestClientFactory = new FakeRestClientFactory(responses);
            var companyHouseApiUrl = "https://fake.url/";
            var fakeApiKey = "fake api key";
            var connectionTimeOut = "1000";

            var sut = new CompanyHouseConnection(fakeRestClientFactory, companyHouseApiUrl, fakeApiKey,
                connectionTimeOut);

            var response = sut.ProcessRequest("fake company id", new DefaultHttpContext());

            Assert.AreEqual(HttpStatusCode.NotFound, response.HttpStatusCode);
        }

        [Test]
        public void Found()
        {
            var companyHouseResponse = new CompanyHouseResponse {CompanyName = "fake company name"};
            var content = JsonConvert.SerializeObject(companyHouseResponse).Replace("CompanyName", "company_name");
            var restResponse = new RestResponse
            {
                StatusCode = HttpStatusCode.OK,
                Content = content
            };
            var responses = new List<IRestResponse> {restResponse};
            var fakeRestClientFactory = new FakeRestClientFactory(responses);
            var companyHouseApiUrl = "https://fake.url/";
            var fakeApiKey = "fake api key";
            var connectionTimeOut = "1000";

            var sut = new CompanyHouseConnection(fakeRestClientFactory, companyHouseApiUrl, fakeApiKey,
                connectionTimeOut);

            var response = sut.ProcessRequest("fake company id", new DefaultHttpContext());

            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.AreEqual("fake company name", response.CompanyName);
        }

        [Test]
        public void Model()
        {
            var response = new CompanyHouseResponse
            {
                CompanyNumber = $"fake {nameof(CompanyHouseResponse.CompanyNumber)}",
                CompanyName = $"fake {nameof(CompanyHouseResponse.CompanyName)}",
                CompanyStatus = $"fake {nameof(CompanyHouseResponse.CompanyStatus)}",
                SicCodes = new[] {"fake", "sic", "codes"},
                CreationDate = $"fake {nameof(CompanyHouseResponse.CreationDate)}",
                RegisteredOfficeAddress = new RegisteredOfficeAddress
                {
                    AddressLine1 = $"fake {nameof(RegisteredOfficeAddress.AddressLine1)}",
                    AddressLine2 = $"fake {nameof(RegisteredOfficeAddress.AddressLine2)}",
                    Locality = $"fake {nameof(RegisteredOfficeAddress.Locality)}",
                    Country = $"fake {nameof(RegisteredOfficeAddress.Country)}",
                    PostalCode = $"fake {nameof(RegisteredOfficeAddress.PostalCode)}"
                },
                CompanyType = $"fake {nameof(CompanyHouseResponse.CompanyType)}",
                HasInsolvencyHistory = true,
                Jurisdiction = $"fake {nameof(CompanyHouseResponse.Jurisdiction)}",
                RegisteredOfficeDisputed = true,
                UndeliverableRegisteredOfficeAddress = true
            };

            var json =
                JsonConvert
                    .SerializeObject(response)
                    .Replace("\"CompanyNumber", "\"company_number")
                    .Replace("\"CompanyName", "\"company_name")
                    .Replace("\"CompanyStatus", "\"company_status")
                    .Replace("\"SicCodes", "\"sic_codes")
                    .Replace("\"CreationDate", "\"date_of_creation")
                    .Replace("\"RegisteredOfficeAddress", "\"registered_office_address")
                    .Replace("\"AddressLine1", "\"address_line_1")
                    .Replace("\"AddressLine2", "\"address_line_2")
                    .Replace("\"Country", "\"country")
                    .Replace("\"Locality", "\"locality")
                    .Replace("\"PostalCode", "\"postal_code")
                    .Replace("\"CompanyType", "\"type")
                    .Replace("\"HasInsolvencyHistory", "\"has_insolvency_history")
                    .Replace("\"Jurisdiction", "\"jurisdiction")
                    .Replace("\"RegisteredOfficeDisputed", "\"registered_office_is_in_dispute")
                    .Replace("\"UndeliverableRegisteredOfficeAddress", "\"undeliverable_registered_office_address");

            var result = JsonSerializer.Deserialize<CompanyHouseResponse>(json);

            Assert.NotNull(result);
            Assert.AreEqual(response.CompanyNumber, result.CompanyNumber); //TODO figure out why this is
            Assert.AreEqual(response.CompanyName, result.CompanyName);
            Assert.AreEqual(response.CompanyName, result.CompanyName);
            Assert.AreEqual(response.CompanyStatus, result.CompanyStatus);
            Assert.AreEqual(response.SicCodes, result.SicCodes);
            Assert.AreEqual(response.CreationDate, result.CreationDate);
            Assert.AreEqual(response.RegisteredOfficeAddress.AddressLine1, result.RegisteredOfficeAddress.AddressLine1);
            Assert.AreEqual(response.RegisteredOfficeAddress.AddressLine2, result.RegisteredOfficeAddress.AddressLine2);
            Assert.AreEqual(response.RegisteredOfficeAddress.Country, result.RegisteredOfficeAddress.Country);
            Assert.AreEqual(response.RegisteredOfficeAddress.Locality, result.RegisteredOfficeAddress.Locality);
            Assert.AreEqual(response.RegisteredOfficeAddress.PostalCode, result.RegisteredOfficeAddress.PostalCode);
            Assert.AreEqual(response.CompanyType, result.CompanyType);
            Assert.AreEqual(response.HasInsolvencyHistory, result.HasInsolvencyHistory);
            Assert.AreEqual(response.Jurisdiction, result.Jurisdiction);
            Assert.AreEqual(response.RegisteredOfficeDisputed, result.RegisteredOfficeDisputed);
            Assert.AreEqual(response.UndeliverableRegisteredOfficeAddress, result.UndeliverableRegisteredOfficeAddress);
        }
    }
}