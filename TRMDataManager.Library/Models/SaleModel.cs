using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.Models
{
	public class SaleModel
	{
		public List<SaleDetailModel> SaleDetails { get; set; }
	}
}