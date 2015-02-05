﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Domain.Order.Model;
using VirtoCommerce.Foundation.Frameworks;
using VirtoCommerce.Foundation.Money;

namespace VirtoCommerce.OrderModule.Data.Model
{
	public class ShipmentEntity : OperationEntity, IStockOutOperation
	{
		public ShipmentEntity()
		{
			Items = new NullCollection<LineItemEntity>();
			InPayments = new NullCollection<PaymentInEntity>();
			Discounts = new NullCollection<DiscountEntity>();
			Addresses = new NullCollection<AddressEntity>();
		}
		
		[StringLength(64)]
		public string OrganizationId { get; set; }
		[StringLength(64)]
		public string FulfilmentCenterId { get; set; }
		[StringLength(64)]
		public string EmployeeId { get; set; }
		[StringLength(64)]
		public string ShipmentMethodCode { get; set; }
		[StringLength(16)]
		public string WeightUnit { get; set; }
		public decimal? WeightValue { get; set; }
		public decimal? VolumetricWeight { get; set; }
		[StringLength(16)]
		public string DimensionUnit { get; set; }
		public decimal? DimensionHeight { get; set; }
		public decimal? DimensionLength { get; set; }
		public decimal? DimensionWidth { get; set; }


		public string CustomerOrderId { get; set; }
		public virtual CustomerOrderEntity CustomerOrder { get; set; }

		public virtual ObservableCollection<LineItemEntity> Items { get; set; }
		public virtual ObservableCollection<PaymentInEntity> InPayments { get; set; }
		public virtual ObservableCollection<AddressEntity> Addresses { get; set; }

		public virtual ObservableCollection<DiscountEntity> Discounts { get; set; }

		#region IStockOperation Members
		[NotMapped]
		IEnumerable<IPosition> IStockOperation.Positions
		{
			get { return Items; }
		}

		#endregion
	}
}
