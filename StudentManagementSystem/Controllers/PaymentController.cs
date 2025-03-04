using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Razorpay.Api;

namespace StudentManagementSystem.Controllers
{
    public class  PaymentController : Controller
    {
       private readonly string _key;
        private readonly string _secret;

        public PaymentController(IConfiguration configuration)
        {
            _key = configuration["Razorpay:Key"];
            _secret = configuration["Razorpay:Secret"];
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CreateOrder(int amount)
        {
            try
            {
                RazorpayClient client = new RazorpayClient(_key, _secret);

                Dictionary<string, object> options = new Dictionary<string, object>
                {
                    { "amount", amount * 100 },  // Amount in paise
                    { "currency", "INR" },
                    { "receipt", Guid.NewGuid().ToString() }
                };

                Order order = client.Order.Create(options);
                return Json(new { success = true, orderId = order["id"].ToString(), key = _key });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult VerifyPayment(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)
        {
            try
            {
                RazorpayClient client = new RazorpayClient(_key, _secret);
                Dictionary<string, string> attributes = new Dictionary<string, string>
                {
                    { "razorpay_payment_id", razorpay_payment_id },
                    { "razorpay_order_id", razorpay_order_id },
                    { "razorpay_signature", razorpay_signature }
                };

                Utils.verifyPaymentSignature(attributes);
                return Json(new { success = true, message = "Payment successful" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Payment verification failed: " + ex.Message });
            }
        } 
    }

}