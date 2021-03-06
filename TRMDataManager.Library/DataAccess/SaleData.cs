﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
	public class SaleData
	{
		public void SaveSale(SaleModel saleInfo, string cashierId)
		{
			//TODO: Make this SOLID/DRY/Better
			//Start filling in the models we will save to the database
			List<SaleDetailDBModel> details = new List<SaleDetailDBModel>();
			ProductData products = new ProductData();
			var taxRate = ConfigHelper.GetTaxRate()/100;

			foreach (var item in saleInfo.SaleDetails)
			{
				var detail = new SaleDetailDBModel
				{
					ProductId = item.ProductId,
					Quantity = item.Quantity
				};

				//get the information about this porduct
				var productInfo = products.GetProductById(detail.ProductId);
				if (productInfo == null)
				{
					throw new Exception($"The Product Id of { detail.ProductId} could not be found in the database.");
				}

				detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

				if (productInfo.IsTaxable)
				{
					detail.Tax = (detail.PurchasePrice * taxRate);
				}

				details.Add(detail);
			}
			//create the sale model
			SaleDBModel sale = new SaleDBModel
			{
				SubTotal = details.Sum(x => x.PurchasePrice),
				Tax = details.Sum(x => x.Tax),
				CashierId = cashierId
			};

			sale.Total = sale.SubTotal + sale.Tax;

			//save the sale model

			using(SqlDataAccess sql = new SqlDataAccess())
			{
				try
				{
					sql.StartTransaction("TRMData");

					//save the sale model
					sql.SaveDataInTransaction("dbo.spSale_Insert", sale);

					//get the Id from the sale mode
					sale.Id = sql.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

					//finish filling in the sale detail models
					foreach (var item in details)
					{
						item.SaleId = sale.Id;
						//save the sale deatail models
						sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
					}

					sql.CommitTransaction();
				}
				catch
				{
					sql.RollbackTransaction();
					throw;
				}
			}
		}

		public List<SaleReportModel> GetSaleReport()
		{
			SqlDataAccess sql = new SqlDataAccess();
			var output = sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "TRMData");

			return output;
		}



	}
}
