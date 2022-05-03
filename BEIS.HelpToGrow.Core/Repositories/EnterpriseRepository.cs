using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.Htg.VendorSme.Database;
using Beis.Htg.VendorSme.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Core.Repositories
{
    public class EnterpriseRepository: IEnterpriseRepository
    {
        private readonly HtgVendorSmeDbContext _context;
        private readonly ILogger<EnterpriseRepository> _logger;

        public EnterpriseRepository(HtgVendorSmeDbContext context, ILogger<EnterpriseRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddEnterprise(enterprise enterprise)
        {
            _logger.LogInformation("Executing EnterpriseRepository.AddEnterprise at {@time} for company house number : {@companies_house_no} and fca : {@fca}", DateTime.Now, enterprise.companies_house_no, enterprise.fca_no);
            try
            {
                try
                {
                    await _context.enterprises.AddAsync(enterprise);
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "There was an error adding an enterprise : {@enterprise}", JsonSerializer.Serialize(enterprise));
                    
                    throw;
                }
            }
            finally
            {
                _logger.LogInformation("EnterpriseRepository.AddEnterprise completed at {@time} for company house number : {@companies_house_no} and fca : {@fca}", DateTime.Now, enterprise.companies_house_no, enterprise.fca_no);
            }                       
        }

        public async Task<enterprise> GetEnterpriseByCompanyNumber(string companyNumber)
        {
            _logger.LogInformation("Executing EnterpriseRepository.GetEnterpriseByCompanyNumber at {@time} for company house number {@companies_house_no}", DateTime.Now, companyNumber);
            try
            {
                return await _context.enterprises.OrderByDescending(x => x.enterprise_created_date).FirstOrDefaultAsync(ent => ent.companies_house_no == companyNumber);
            }
            finally
            {
                _logger.LogInformation("EnterpriseRepository.GetEnterpriseByCompanyNumber completed at {@time} for company house number {@companies_house_no}", DateTime.Now, companyNumber);
            }
            
        }

        public async Task<enterprise> GetEnterpriseByFCANumber(string fcaNumber)
        {
            _logger.LogInformation("Executing EnterpriseRepository.GetEnterpriseByCompanyNumber at {@time} for fca number {@fca}", DateTime.Now, fcaNumber);
            try
            {
                return await _context.enterprises.OrderByDescending(x => x.enterprise_created_date).FirstOrDefaultAsync(ent => ent.fca_no.ToLower() == fcaNumber.ToLower());
            }
            finally
            {
                _logger.LogInformation("EnterpriseRepository.GetEnterpriseByCompanyNumber completed at {@time} for fca {@fca}", DateTime.Now, fcaNumber);
            }            
        }

        public async Task<enterprise> GetEnterprise(long enterpriseId)
        {
            _logger.LogInformation("Executing EnterpriseRepository.GetEnterprise at {@time} for enterprise {@enterprise}", DateTime.Now, enterpriseId);
            try
            {
                return await _context.enterprises.FirstOrDefaultAsync(ent => ent.enterprise_id == enterpriseId);
            }
            finally
            {
                _logger.LogInformation("EnterpriseRepository.GetEnterprise completed at {@time} for enterprise {@enterprise}", DateTime.Now, enterpriseId);
            }
            
        }

        public async Task<enterprise> UpdateEnterprise(enterprise enterprise)
        {
            _logger.LogInformation("Executing EnterpriseRepository.UpdateEnterprise at {@time} for enterprise {@enterprise}", DateTime.Now, enterprise.enterprise_id);
            try
            {
                try
                {
                    var persisted = await GetEnterprise(enterprise.enterprise_id);

                    persisted.eligibility_status_id = enterprise.eligibility_status_id;
                    persisted.company_postcode = enterprise.company_postcode;
                    persisted.company_age = enterprise.company_age;
                    persisted.company_trading_status = enterprise.company_trading_status;
                    persisted.company_gazette_data = enterprise.company_gazette_data;
                    persisted.company_financial_providers_no = enterprise.company_financial_providers_no;
                    persisted.company_disqualified_directors = enterprise.company_disqualified_directors;
                    persisted.company_account_filing_ind = enterprise.company_account_filing_ind;
                    persisted.company_abnormal_filing_ind = enterprise.company_abnormal_filing_ind;
                    persisted.company_holding_ind = enterprise.company_holding_ind;
                    persisted.company_address_changes_ind = enterprise.company_address_changes_ind;
                    persisted.company_multi_match_ind = enterprise.company_multi_match_ind;
                    persisted.enterprise_size_id = enterprise.enterprise_size_id;
                    persisted.risk_profile_score = enterprise.risk_profile_score;
                    persisted.scorecheck_score = enterprise.scorecheck_score;
                    persisted.marketing_consent = enterprise.marketing_consent;
                    await _context.SaveChangesAsync();

                    return persisted;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "There was an error updating an enterprise : {@enterprise}", JsonSerializer.Serialize(enterprise));
                    
                    throw;
                }
            }
            finally
            {
                _logger.LogInformation("EnterpriseRepository.UpdateEnterprise completed at {@time} for enterprise {@enterprise}", DateTime.Now, enterprise.enterprise_id);
            }            
        }
    }
}