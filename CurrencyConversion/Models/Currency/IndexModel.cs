using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CurrencyConversion.Models.Currency
{
    public class IndexModel
    {
        public SelectList AvailableCurrencies { get; set; }

        public string SourceCurrency { get; set; }
        public string DestinationCurrency { get; set; }

        [Required(), Range(0, Int32.MaxValue), Display(Name = "Source Amount")]
        public decimal SourceAmount { get; set; }
    }
}