using System.Web.Mvc;
using CurrencyConversion.Controllers;
using CurrencyConversion.Models.Currency;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Mocks;
using CurrencyConversion.Services;

namespace CurrencyConversion.Tests.Controllers
{   
    [TestFixture]
    public class CurrencyControllerTest
    {
        /// <summary>
        /// Verify that a model populated with currencies is returned to the view.
        /// </summary>
        [Test]
        public void Verify_Index_Populates_Dropdowns()
        {
            // Create mock currency service
            var stubbedCurrencyService = MockRepository.GenerateStub<ICurrencyService>();
            var currencies = new Dictionary<string, string> {{"ABC", "Alphabet Currency"}, {"MTC", "Magic Currency"}};
            stubbedCurrencyService.Stub(s => s.GetCurrencies()).Return(currencies);

            // Call controller action
            var controller = new CurrencyController(stubbedCurrencyService);
            var index = (ViewResult)controller.Index();
            var indexModel = (IndexModel)index.Model;

            // Validate
            Assert.That(indexModel.AvailableCurrencies.Count(), Is.EqualTo(currencies.Count));
        }

        /// <summary>
        /// Verify that the end result is the source amount multiplied by the rate.
        /// </summary>
        [Test]
        public void Verify_Destination_Currency_Properly_Calculated()
        {
            // Create mock currency service
            var stubbedCurrencyService = MockRepository.GenerateStub<ICurrencyService>();
            var rate = (decimal)0.5d;
            stubbedCurrencyService.Stub(s => s.GetExchangeRate("ABC", "MTG")).Return(rate);

            // Call controller action
            var controller = new CurrencyController(stubbedCurrencyService);
            var indexModel = new IndexModel { SourceCurrency = "ABC", DestinationCurrency = "MTG", SourceAmount = 15 };
            var result = (ContentResult)controller.Convert(indexModel);

            // Validate
            Assert.That(result.Content, Is.EqualTo((indexModel.SourceAmount * rate).ToString()));
        }
    }
}
