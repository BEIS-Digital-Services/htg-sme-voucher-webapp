
namespace BEIS.HelpToGrow.Voucher.Web.Services.FCAServices
{
    public class ProductPriceService : IProductPriceService
    {
        private readonly IProductPriceDescriptionRepository _repository;
        private readonly ILogger<ProductPriceService> _logger;

        public ProductPriceService(
            IProductPriceDescriptionRepository repository,
            ILogger<ProductPriceService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<string> GetProductPrice(long id)
        {
            try
            {
                var priceDescription = await _repository.GetProductPriceBaseDescription(id);

                var price =
                        priceDescription
                            .product_price_amount
                            .ToString("0.00");

                var period =
                        priceDescription
                            .product_price_base_description
                            .product_price_basedescription
                            .Split(',')
                            .First();

                var users =
                        priceDescription
                            .product_price_no_users > 1
                                ? "users"
                                : "user";

                return $"{price}, {period} for {priceDescription.product_price_no_users} {users}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error determining product price description for product id: {id}");

                return null;
            }
        }
    }
}