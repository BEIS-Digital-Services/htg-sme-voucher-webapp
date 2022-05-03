using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.Htg.VendorSme.Database;
using Beis.Htg.VendorSme.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Core.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly HtgVendorSmeDbContext _context;
        private readonly ILogger<TokenRepository> _logger;

        public TokenRepository(HtgVendorSmeDbContext context, ILogger<TokenRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddToken(token token)
        {
            _logger.LogInformation("Executing TokenRepository.AddToken at {@time} for enterprise {@enterprise} and product {@product}", DateTime.Now, token.enterprise_id, token.product);
            try
            {
                try
                {
                    await _context.tokens.AddAsync(token);
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "There was an error adding a Token : {@token}", JsonSerializer.Serialize(token));

                    throw;
                }
            }
            finally
            {
                _logger.LogInformation("TokenRepository.AddToken completed at {@time} for enterprise {@enterprise} and product {@product}", DateTime.Now, token.enterprise_id, token.product);
            }
        }

        public async Task<token> GetToken(long enterpriseId, long productId)
        {
            _logger.LogInformation("Executing TokenRepository.GetToken at {@time} for enterprise {@enterprise} and product {@product}", DateTime.Now, enterpriseId, productId);
            try
            {
                return await _context.tokens.FirstOrDefaultAsync(t => t.enterprise_id == enterpriseId && t.product == productId);
            }
            finally
            {
                _logger.LogInformation("TokenRepository.GetToken completed at {@time} for enterprise {@enterprise} and product {@product}", DateTime.Now, enterpriseId, productId);
            }
        }

        public async Task<List<token>> GetEnterpriseNotRedeemedToken(int unactionedDays, bool reminder1, bool reminder2, bool reminder3)
        {
            try
            {
                var eligibleReminderDate = DateTime.UtcNow.AddDays(unactionedDays * -1);
                return await _context.tokens.Where(_ =>
                                                _.token_valid_from < eligibleReminderDate
                                                && _.redemption_status_id == 0
                                                && ((reminder1 && !_.reminder_1)
                                                    || (reminder2 && _.reminder_1 && !_.reminder_2)
                                                    || (reminder3 && _.reminder_1 && _.reminder_2 && !_.reminder_3))
                                                )
                                                .ToListAsync();
            }
            finally
            {
                _logger.LogInformation($"TokenRepository.GetEnterpriseNotRedeemedToken completed at {DateTime.Now} for {unactionedDays} days");
            }
        }

        public async Task UpdateReminderStatus(long tokenId, bool reminder1, bool reminder2, bool reminder3)
        {
            try
            {
                var token = _context.tokens.First(x => x.token_id == tokenId);

                try
                {
                    if (reminder1)
                    {
                        token.reminder_1 = true;
                    }
                    else if (reminder2)
                    {
                        token.reminder_2 = true;
                    }
                    else if (reminder3)
                    {
                        token.reminder_3 = true;
                        token.cancellation_status_id = 4;
                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"There was an error updating token reminder status for : {JsonSerializer.Serialize(token)}");
                    throw;
                }
            }
            finally
            {
                _logger.LogInformation($"TokenRepository.UpdateReminderStatus completed for token {tokenId} at {DateTime.Now}");
            }
        }

        public async Task<token> GetToken(long enterpriseId)
        {
            _logger.LogInformation("Executing TokenRepository.GetToken at {@time} for enterprise {@enterprise}", DateTime.Now, enterpriseId);
            try
            {
                return await _context.tokens.SingleOrDefaultAsync(t => t.enterprise_id == enterpriseId);
            }
            finally
            {
                _logger.LogInformation("TokenRepository.GetToken completed at {@time} for enterprise {@enterprise}", DateTime.Now, enterpriseId);
            }
        }

        public async Task<token> UpdateToken(token token)
        {
            _logger.LogInformation("Executing TokenRepository.UpdateToken at {@time} for token {@token}", DateTime.Now, token.token_id);

            try
            {
                var persisted = await GetToken(token.enterprise_id);

                persisted.cancellation_status_id = token.cancellation_status_id;
                await _context.SaveChangesAsync();

                return persisted;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "There was an error updating a token : {@token}", JsonSerializer.Serialize(token));

                throw;
            }
            finally
            {
                _logger.LogInformation("TokenRepository.UpdateToken completed at {@time} for token {@token}", DateTime.Now, token.token_id);
            }



        }
    }
}
