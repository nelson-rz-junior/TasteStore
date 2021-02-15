using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using TasteStore.Utility;

namespace TasteStore.Pages.Customer.Cart
{
    [Authorize]
    public class PaymentSuccessModel : PageModel
    {
        private readonly ILogger<PaymentSuccessModel> _logger;
        private readonly StripeSettings _options;

        public string ErrorMessage { get; set; }

        public PaymentSuccessModel(ILogger<PaymentSuccessModel> logger, IOptions<StripeSettings> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<IActionResult> OnGet(string sessionId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            RestClient client = new RestClient(_options.ApiBaseUrl);

            RestRequest request = new RestRequest("order/save", Method.POST);
            request.AddJsonBody(new CreateOrderApiRequest
            { 
                SessionId = sessionId, 
                UserId = claims.Value
            });

            var response = await client.ExecuteAsync<CreateOrderApiResponse>(request);

            if (response.Data != null && response.StatusCode == HttpStatusCode.Created)
            {
                _logger.LogInformation($"Order created successful: {response}");

                HttpContext.Session.SetInt32(SD.ShoppingCart, 0);

                return RedirectToPage("/Customer/Cart/OrderConfirmation", new { id = response.Data.OrderId });
            }

            _logger.LogInformation($"Error while creating order: {response}");

            ErrorMessage = response.ErrorMessage;

            return Page();
        }
    }
}
