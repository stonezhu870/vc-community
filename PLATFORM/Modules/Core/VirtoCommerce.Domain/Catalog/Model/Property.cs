﻿using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Domain.Catalog.Model
{
	public class Property : AuditableEntity
	{
		public string CatalogId { get; set; }
		public Catalog Catalog { get; set; }
		public string CategoryId { get; set; }
		public Category Category { get; set; }
		public string Name { get; set; }
		public bool Required { get; set; }
		public bool Dictionary { get; set; }
		public bool Multivalue { get; set; }
		public bool Multilanguage { get; set; }
		public PropertyValueType ValueType { get; set; }
		public PropertyType Type { get; set; }
		public ICollection<PropertyAttribute> Attributes { get; set; }
		public ICollection<PropertyDictionaryValue> DictionaryValues { get; set; }
		public ICollection<PropertyDisplayName> DisplayNames { get; set; }
	}
}