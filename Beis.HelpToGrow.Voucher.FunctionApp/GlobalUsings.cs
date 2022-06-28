global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;

global using Microsoft.AspNetCore.DataProtection;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.AspNetCore.WebUtilities;
global using Microsoft.Azure.Functions.Worker;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Hosting;

global using Beis.HelpToGrow.Repositories;
global using Beis.HelpToGrow.Repositories.Interfaces;
global using Beis.HelpToGrow.Persistence;
global using Beis.HelpToGrow.Persistence.Models;

global using Beis.HelpToGrow.Common.Config;
global using Beis.HelpToGrow.Common.Interfaces;
global using Beis.HelpToGrow.Common.Services;

global using Beis.HelpToGrow.Common.Voucher.Interfaces;
global using Beis.HelpToGrow.Common.Voucher.Config;
global using Beis.HelpToGrow.Common.Voucher.Services;
global using Beis.HelpToGrow.Voucher.FunctionApp.Extensions;


