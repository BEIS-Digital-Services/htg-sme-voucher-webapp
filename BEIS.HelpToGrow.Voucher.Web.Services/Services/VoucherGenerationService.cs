using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.Htg.VendorSme.Database.Models;
using BEIS.HelpToGrow.Voucher.Web.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.Web.Services
{
    public class VoucherGenerationService : IVoucherGenerationService
    {
        private readonly IConfiguration _configuration;
        private readonly IEncryptionService _encryptionService;
        private readonly ITokenRepository _tokenRepository;
        private readonly ILogger<VoucherGenerationService> _logger;

        public VoucherGenerationService(
            IConfiguration configuration,
            IEncryptionService encryptionService,
            ITokenRepository tokenRepository,
            ILogger<VoucherGenerationService> logger)
        {
            _configuration = configuration;
            _encryptionService = encryptionService;
            _tokenRepository = tokenRepository;
            _logger = logger;
        }

        public async Task<string> GenerateVoucher(vendor_company vendorCompany, enterprise enterprise, product product)
        {
            _logger.LogInformation("Executing VoucherGenerationService.GenerateVoucher at {@time} for enterprise {@enterprise} and product {@product}", DateTime.Now, enterprise.enterprise_id, product.product_id);
            try
            {
                var voucherCodeLength = int.Parse(_configuration["VoucherSettings:voucherCodeLength"]);
                var voucherCodeString = GenerateSetCode(voucherCodeLength);
                var token = await _tokenRepository.GetToken(enterprise.enterprise_id, product.product_id);
                string encryptedToken;
                if (token == null)
                {
                    const decimal balance = 5000;
                    encryptedToken = cleanToken(_encryptionService.Encrypt(voucherCodeString, vendorCompany.registration_id + vendorCompany.vendorid));

                    token = new token
                    {
                        token_code = voucherCodeString,
                        token_balance = balance,
                        enterprise_id = enterprise.enterprise_id,
                        token_valid_from = DateTime.Now,
                        token_expiry = DateTime.Now.AddDays(31),
                        redemption_status_id = 0,
                        product = product.product_id,
                        reconciliation_status_id = 0,
                        authorisation_code = GenerateSetCode(voucherCodeLength),
                        obfuscated_token = $"**********{encryptedToken.Substring(encryptedToken.Length - 4)}"
                    };

                    await _tokenRepository.AddToken(token);
                }
                else
                {
                    voucherCodeString = token.token_code;
                    encryptedToken = cleanToken(_encryptionService.Encrypt(voucherCodeString, vendorCompany.registration_id + vendorCompany.vendorid));
                }

                return encryptedToken;
            }
            catch(Exception e)
            { 
                throw e;
            }
            finally
            {
                _logger.LogInformation("VoucherGenerationService.GenerateVoucher completed at {@time} for enterprise {@enterprise} and product {@product}", DateTime.Now, enterprise.enterprise_id, product.product_id);
            }
        }

        private static string cleanToken(string encryptedToken)
        {
            return encryptedToken.EndsWith("==")
                ? encryptedToken[..^2]
                : encryptedToken;
        }

        public string GenerateSetCode(int length) =>
            length == 0
                ? IncompleteSetCode
                : CompleteSetCode(length);

        private static string IncompleteSetCode => "0";
        
        private static string CompleteSetCode(int length)
        {
            const int startNumber = 10;

            var rnd = new Random();
            var rangePaddingFormat = Enumerable.Range(1, length - 1).Aggregate("0", (current, _) => current + "0");
            var maxLength = 10;

            for (var i = 1; i < length; i++)
            {
                maxLength *= startNumber;
            }

            return rnd.Next(0, maxLength).ToString(rangePaddingFormat);
        }
    }
}