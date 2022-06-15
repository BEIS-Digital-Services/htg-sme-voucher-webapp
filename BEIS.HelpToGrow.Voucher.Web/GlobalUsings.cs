global using System;
global using System.Reflection;
global using System.IO;
global using System.Net;
global using System.Text;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.Threading.Tasks;
global using System.ComponentModel.DataAnnotations;
global using System.Linq;
global using System.Text.RegularExpressions;
global using System.Text.Json.Serialization;
global using System.Globalization;
global using System.Runtime.CompilerServices;
global using System.Threading;
global using System.Web;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.Extensions.Configuration;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Options;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.Extensions.Caching.Distributed;

global using CsvHelper;
global using CsvHelper.Configuration;
global using FluentResults;
global using Notify.Exceptions;

global using Beis.HelpToGrow.Voucher.Web.Common;
global using Beis.HelpToGrow.Voucher.Web.Config;
global using Beis.HelpToGrow.Voucher.Web.Controllers;
global using Beis.HelpToGrow.Voucher.Web.Enums;
global using Beis.HelpToGrow.Voucher.Web.Models;
global using Beis.HelpToGrow.Voucher.Web.Models.Applicant;
global using Beis.HelpToGrow.Voucher.Web.Models.CompaniesHouse;
global using Beis.HelpToGrow.Voucher.Web.Models.FCA;
global using Beis.HelpToGrow.Voucher.Web.Models.NewToSoftware;
global using Beis.HelpToGrow.Voucher.Web.Models.Product;
global using Beis.HelpToGrow.Voucher.Web.Models.Voucher;
global using Beis.HelpToGrow.Voucher.Web.Services;
global using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Rules;
global using Beis.HelpToGrow.Voucher.Web.Services.FCAServices;
global using Beis.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
global using Beis.HelpToGrow.Voucher.Web.Services.Eligibility;
global using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification;
global using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Extensions;

global using Beis.HelpToGrow.Persistence;
global using Beis.HelpToGrow.Persistence.Models;

global using Beis.HelpToGrow.Repositories;
global using Beis.HelpToGrow.Repositories.Enums;
global using Beis.HelpToGrow.Repositories.Interfaces;

global using Beis.HelpToGrow.Common.Enums;
global using Beis.HelpToGrow.Common.Config;
global using Beis.HelpToGrow.Common.Interfaces;
global using Beis.HelpToGrow.Common.Models;
global using Beis.HelpToGrow.Common.Services;





