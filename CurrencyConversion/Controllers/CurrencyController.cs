using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CurrencyConversion.Models.Currency;
using CurrencyConversion.Services;

namespace CurrencyConversion.Controllers
{
    public class CurrencyController : Controller
    {
        protected ICurrencyService CurrencyService { get; set; }

        public CurrencyController(ICurrencyService currencyService)
        {
            CurrencyService = currencyService;
        }

        /// <summary>
        /// The main currency conversion action which presents the options for the user.
        /// </summary>
        [GET("/")]
        public ActionResult Index()
        {
            Dictionary<string, string> currencies = CurrencyService.GetCurrencies();
            var vm = new IndexModel();
            vm.AvailableCurrencies = new SelectList(currencies, "Key", "Value");
            vm.SourceCurrency = "USD";      // Initial default
            vm.DestinationCurrency = "JPY"; // Initial default
            return View(vm);
        }

        /// <summary>
        /// Returns the result to an AJAX request which will display the returned content result.
        /// </summary>
        [POST("/convert")]
        public ActionResult Convert(IndexModel vm)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join("<br />", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                return new ContentResult{ Content = errors };
            }

            decimal exchangeRate;
            try
            {
                exchangeRate = CurrencyService.GetExchangeRate(vm.SourceCurrency, vm.DestinationCurrency);
            }
            catch (CurrencyServiceException ex)
            {
                return new ContentResult {Content = ex.Message};
            }

            decimal newValue = vm.SourceAmount * exchangeRate;
            return new ContentResult { Content = newValue.ToString() };
        }
    }
}
