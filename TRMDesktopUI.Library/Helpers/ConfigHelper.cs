﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.Library.Helpers
{
	public class ConfigHelper : IConfigHelper
	{
		//TODO: Move this from config to the API
		public decimal GetTaxRate()
		{
			string rateText = ConfigurationManager.AppSettings["taxRate"];
			bool IsValidTaxrate = Decimal.TryParse(rateText, out decimal output);

			if (IsValidTaxrate == false)
			{
				throw new ConfigurationErrorsException("The tax rate is not set uup properly");
			}

			return output;
		}
	}
}
