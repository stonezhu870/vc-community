﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.CatalogModule.Web.Model
{
	public class AssetBase
	{
		public string Id { get; set; }
		public string RelativeUrl { get; set; }
		public string Url { get; set; }
		public string TypeId { get; set; }
	    public string Group { get; set; }
		public string Name { get; set; }
		public string LanguageCode { get; set; }
	
	}
}
