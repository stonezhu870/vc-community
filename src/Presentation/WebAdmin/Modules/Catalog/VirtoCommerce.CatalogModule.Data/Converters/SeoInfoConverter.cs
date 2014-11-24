﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Foundation.Frameworks.Extensions;
using foundation = VirtoCommerce.Foundation.Catalogs.Model;
using module = VirtoCommerce.CatalogModule.Model;
using foundationConfig = VirtoCommerce.Foundation.AppConfig.Model;

namespace VirtoCommerce.CatalogModule.Data.Converters
{
	public static class SeoInfoConverter
	{
		/// <summary>
		/// Converting to model type
		/// </summary>
		/// <param name="catalogBase"></param>
		/// <returns></returns>
		public static module.SeoInfo ToModuleModel(this foundationConfig.SeoUrlKeyword dbSeoInfo)
		{
			var retVal = new module.SeoInfo
			{
				Id = dbSeoInfo.SeoUrlKeywordId,
				SemanticUrl = dbSeoInfo.Keyword,
				ImageAltDescription = dbSeoInfo.ImageAltDescription,
				LanguageCode = dbSeoInfo.Language,
				MetaDescription = dbSeoInfo.MetaDescription,
				PageTitle = dbSeoInfo.Title
			};
			return retVal;
		}

		/// <summary>
		/// Converting to foundation type
		/// </summary>
		/// <param name="catalog"></param>
		/// <returns></returns>
		public static foundationConfig.SeoUrlKeyword ToFoundation(this module.SeoInfo seoInfo, module.CatalogProduct product)
		{
			var retVal = seoInfo.ToFoundation();
			retVal.KeywordValue = product.Id;
			retVal.KeywordType = (int)foundationConfig.SeoUrlKeywordTypes.Item;
			return retVal;
		}

		/// <summary>
		/// Converting to foundation type
		/// </summary>
		/// <param name="catalog"></param>
		/// <returns></returns>
		public static foundationConfig.SeoUrlKeyword ToFoundation(this module.SeoInfo seoInfo, module.Category category)
		{
			var retVal = seoInfo.ToFoundation();
			retVal.KeywordValue = category.Id;
			retVal.KeywordType = (int)foundationConfig.SeoUrlKeywordTypes.Category;
			return retVal;
		}

		private static foundationConfig.SeoUrlKeyword ToFoundation(this module.SeoInfo seoInfo)
		{
			var retVal = new foundationConfig.SeoUrlKeyword
			{
				Keyword = seoInfo.SemanticUrl,
				Language = seoInfo.LanguageCode,
				MetaDescription = seoInfo.MetaDescription,
				Title = seoInfo.PageTitle,
				ImageAltDescription = seoInfo.ImageAltDescription
			};
			if (seoInfo.Id != null)
			{
				retVal.SeoUrlKeywordId = seoInfo.Id;
			}
			return retVal;
		}

		/// <summary>
		/// Patch CatalogLanguage type
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		public static void Patch(this foundationConfig.SeoUrlKeyword source, foundationConfig.SeoUrlKeyword target)
		{
			if (target == null)
				throw new ArgumentNullException("target");

			//Simply patch
			if (source.Keyword != null)
				target.Keyword = source.Keyword;
			if (source.MetaDescription != null)
				target.MetaDescription = source.MetaDescription;
			if (source.ImageAltDescription != null)
				target.ImageAltDescription = source.ImageAltDescription;
			if (source.Title != null)
				target.Title = source.Title;
		}

	}

	public class SeoInfoComparer : IEqualityComparer<foundationConfig.SeoUrlKeyword>
	{
		#region IEqualityComparer<SeoUrlKeyword> Members

		public bool Equals(foundationConfig.SeoUrlKeyword x, foundationConfig.SeoUrlKeyword y)
		{
			return x.SeoUrlKeywordId == y.SeoUrlKeywordId;
		}

		public int GetHashCode(foundationConfig.SeoUrlKeyword obj)
		{
			return obj.SeoUrlKeywordId.GetHashCode();
		}

		#endregion
	}

}