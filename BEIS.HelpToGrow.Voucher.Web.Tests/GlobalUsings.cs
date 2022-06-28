global using System;
global using System.IO;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.Linq;
global using System.Threading.Tasks;
global using System.Threading;
global using System.Net.Cache;
global using System.Net.Security;
global using System.Security.Cryptography.X509Certificates;
global using System.Net;
global using System.Text;

global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.Extensions.Hosting;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.Extensions.Configuration;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Extensions.Primitives;

global using HttpContextMoq;
global using Notify.Exceptions;
global using RestSharp.Authenticators;
global using RestSharp.Deserializers;
global using RestSharp.Serialization;
global using Newtonsoft.Json.Linq;
global using Moq;
global using NUnit.Framework;
global using FluentResults;

global using Beis.HelpToGrow.Persistence.Models;
global using Beis.HelpToGrow.Repositories.Interfaces;
global using Beis.HelpToGrow.Common.Config;
global using Beis.HelpToGrow.Common.Interfaces;
global using Beis.HelpToGrow.Common.Services;

global using Beis.HelpToGrow.Common.Voucher.Enums;
global using Beis.HelpToGrow.Common.Voucher.Interfaces;
global using Beis.HelpToGrow.Common.Voucher.Services;
global using Beis.HelpToGrow.Common.Voucher.Config;

global using Beis.HelpToGrow.Voucher.Web.Tests.Eligibility;
global using Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount;
global using Beis.HelpToGrow.Voucher.Web.Services;
global using Beis.HelpToGrow.Voucher.Web.Services.Eligibility;
global using Beis.HelpToGrow.Voucher.Web.Services.Connectors;
global using Beis.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
global using Beis.HelpToGrow.Voucher.Web.Services.FCAServices;
global using Beis.HelpToGrow.Voucher.Web.Models;
global using Beis.HelpToGrow.Voucher.Web.Models.FCA;
global using Beis.HelpToGrow.Voucher.Web.Models.NewToSoftware;
global using Beis.HelpToGrow.Voucher.Web.Models.Product;
global using Beis.HelpToGrow.Voucher.Web.Models.Applicant;
global using Beis.HelpToGrow.Voucher.Web.Models.Voucher;
global using Beis.HelpToGrow.Voucher.Web.Common;
global using Beis.HelpToGrow.Voucher.Web.Enums;
global using Beis.HelpToGrow.Voucher.Web.Config;
global using Beis.HelpToGrow.Voucher.Web.Controllers;
global using Beis.HelpToGrow.Voucher.Web.Models.CompaniesHouse;
global using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Rules;
global using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied;
global using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification;
global using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Extensions;








