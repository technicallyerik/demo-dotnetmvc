using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyConversion.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace CurrencyConversion.Tests
{
    [TestFixture]
    public class CurrencyServiceTest
    {
        /// <summary>
        /// Full integration test that easily allows us to test the API end-to-end 
        /// </summary>
        [Test]
        [Category("IgnoreOnBuildServer")]
        public void GetCurrencies_Integration_Test()
        {
            CurrencyService serv = new CurrencyService();
            Dictionary<string, string> currencies = serv.GetCurrencies();
            Assert.That(currencies, Is.Not.Empty);
        }

        /// <summary>
        /// Test to ensure GetCurrencies properly parses an object of key value pairs into a dictionary.
        /// </summary>
        [Test]
        public void GetCurrencies_Should_Parse_Object_As_Dictionary()
        {
            // Mock out the call to the web service, and return a defined 
            // JSON result similar to what the API would return.
            var mocks = new MockRepository();
            CurrencyService serv = mocks.PartialMock<CurrencyService>();
            string dummyResult = "{\"ABC\" : \"Alphabet Currency\", \"MTG\" : \"Magic Currency\"}";
            Expect.Call(serv.GetJsonFromWebService(null, null)).IgnoreArguments().Return(dummyResult);
            mocks.ReplayAll();

            // Call GetCurrencies and ensure the dummy result was parsed correctly.
            var result = serv.GetCurrencies();
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(2));
            mocks.VerifyAll();
        }

        /// <summary>
        /// Full integration test that easily allows us to test the API end-to-end 
        /// </summary>
        [Test]
        [Category("IgnoreOnBuildServer")]
        public void GetExchangeRate_Integration_Test()
        {
            CurrencyService serv = new CurrencyService();
            decimal rate = serv.GetExchangeRate("USD", "JPY");
            Assert.That(rate, Is.Not.EqualTo(0));
        }

        /// <summary>
        /// Ensures the exchange rate is approprately extracted from a typical response.
        /// </summary>
        [Test]
        public void GetExchangeRate_Should_Return_Appropriate_Rate()
        {
            // Mock out the call to the web service, and return a defined 
            // JSON result similar to what the API would return.
            var mocks = new MockRepository();
            CurrencyService serv = mocks.PartialMock<CurrencyService>();
            string dummyResult = "{\"rates\" : {\"ABC\" : 12.345, \"MTG\" : .0123}}";
            Expect.Call(serv.GetJsonFromWebService(null, null)).IgnoreArguments().Return(dummyResult);
            mocks.ReplayAll();

            // Call GetCurrencies and ensure the dummy result was parsed correctly.
            var result = serv.GetExchangeRate("USD", "MTG");
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(.0123));
            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(typeof(CurrencyServiceException), ExpectedMessage = "Invalid App ID provided", MatchType = MessageMatch.Contains)]
        public void GetExchangeRate_Error_Should_Throw_Proper_Exception()
        {
            // Mock out the call to the web service, and return a defined 
            // JSON result similar to what the API would return.
            var mocks = new MockRepository();
            CurrencyService serv = mocks.PartialMock<CurrencyService>();
            string dummyResult = "{ \"error\": true, \"status\": 401, \"message\": \"invalid_app_id\", \"description\": \"Invalid App ID provided\" }";
            Expect.Call(serv.GetJsonFromWebService(null, null)).IgnoreArguments().Return(dummyResult);
            mocks.ReplayAll();

            // Call GetCurrencies and ensure the exception is thrown
            var result = serv.GetExchangeRate("USD", "MTG");
            mocks.VerifyAll();
        }
    }
}
