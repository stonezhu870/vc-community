﻿namespace VirtoCommerce.CatalogModule.Model
{
	public class PropertyValue
	{
		public string Id { get; set; }
		public string PropertyId { get; set; }
		public string PropertyName { get; set; }
		public string ValueId { get; set; }
		public string Value { get; set; }
		public PropertyValueType ValueType { get; set; }
		public string LanguageCode { get; set; }
	}
}