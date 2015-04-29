﻿using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Pricing.Services;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Domain.Search.Services;

namespace VirtoCommerce.SearchModule.Data.Services
{
    public class CatalogItemIndexBuilder : ISearchIndexBuilder
    {
		private const int DatabasePartitionSizeCount = 5000; // the size of data loaded from database at one time
		private const int PartitionSizeCount = 100; // the maximum partition size, keep it smaller to prevent too big of the sql requests and too large messages in the queue

		private readonly ISearchProvider _searchProvider;
		private readonly ICatalogSearchService _catalogSearchService;
		private readonly IPricingService _pricingService;
		private readonly IItemService _itemService;
		private readonly ICategoryService _categoryService;
		private readonly IPropertyService _propertyService;
		public CatalogItemIndexBuilder(ISearchProvider searchProvider, ICatalogSearchService catalogSearchService, 
									   IItemService itemService, IPricingService pricingService, 
									   ICategoryService categoryService, IPropertyService propertyService)
		{
			_searchProvider = searchProvider;
			_itemService = itemService;
			_catalogSearchService = catalogSearchService;
			_pricingService = pricingService;
			_categoryService = categoryService;
			_propertyService = propertyService;
		}
		#region ISearchIndexBuilder Members
		public string DocumentType
		{
			get
			{
				return "catalogitem";
			}
		}

		public IEnumerable<IDocument> CreateDocuments(Partition partition)
		{
			if (partition == null)
				throw new ArgumentNullException("partition");

			if (partition.Keys == null)
				throw new ArgumentNullException("partition.Keys");


			var index = 0;
			//Trace.TraceInformation(String.Format("Processing documents starting {0} of {1} - {2}%", source.Start, source.Total, (source.Start * 100 / source.Total)));
			foreach (var productId in partition.Keys)
			{
				var doc = new ResultDocument();
				IndexItem(ref doc, productId);
				yield return doc;
				index++;
			}
		}

		public void PublishDocuments(string scope, IDocument[] documents)
		{
			foreach (var doc in documents)
			{
				_searchProvider.Index(scope, DocumentType, doc);
			}

			_searchProvider.Commit(scope);
			_searchProvider.Close(scope, DocumentType);
		}

		public void RemoveDocuments(string scope, string[] documents)
		{
			foreach (var doc in documents)
			{
				_searchProvider.Remove(scope, DocumentType, "__key", doc);
			}
			_searchProvider.Commit(scope);
		}

	
		public IEnumerable<Partition> GetPartitions(string scope, DateTime lastBuild)
		{
			var retVal = new List<Partition>();

			var criteria = new SearchCriteria
			{
				IndexDate = lastBuild,
				ResponseGroup = ResponseGroup.WithProducts,
				Count = 0				
			};
			var result = _catalogSearchService.Search(criteria);

			for (var start = 0; start < result.TotalCount; start += PartitionSizeCount)
			{
				criteria.Start = start;
				criteria.Count += PartitionSizeCount;
				//TODO: Need optimize search to result only product ids
				result = _catalogSearchService.Search(criteria);
				retVal.Add(new Partition(result.Products.Select(x => x.Id).ToArray(), OperationType.Index, "", start, result.TotalCount));
			}
			return retVal;
		}

		#endregion
	
		protected virtual void IndexItem(ref ResultDocument doc, string productId)
		{
			var item = _itemService.GetById(productId, ItemResponseGroup.ItemProperties | ItemResponseGroup.Categories);

			doc.Add(new DocumentField("__key", item.Id.ToLower(), new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));
			//doc.Add(new DocumentField("__loc", "en-us", new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));
			doc.Add(new DocumentField("__type", item.GetType().Name, new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));
			doc.Add(new DocumentField("__sort", item.Name, new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));
			doc.Add(new DocumentField("__hidden", (!item.IsActive).ToString().ToLower(), new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));
			doc.Add(new DocumentField("code", item.Code, new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));
			doc.Add(new DocumentField("name", item.Name, new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));
			//doc.Add(new DocumentField("startdate", item.StartDate, new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));
			//doc.Add(new DocumentField("enddate", item.P.HasValue ? item.EndDate : DateTime.MaxValue, new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));
			doc.Add(new DocumentField("createddate", item.CreatedDate, new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));
			doc.Add(new DocumentField("lastmodifieddate", item.ModifiedDate ??  DateTime.MaxValue, new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));
			doc.Add(new DocumentField("catalog", item.CatalogId.ToLower(), new[] { IndexStore.YES, IndexType.NOT_ANALYZED, IndexDataType.StringCollection }));
			doc.Add(new DocumentField("__outline", item.CatalogId.ToLower(), new[] { IndexStore.YES, IndexType.NOT_ANALYZED, IndexDataType.StringCollection }));

			// Index categories
			IndexItemCategories(ref doc, item);

			// Index custom properties
			IndexItemCustomProperties(ref doc, item);

			// Index item prices
			IndexItemPrices(ref doc, item);

			//Index item reviews
			//IndexReviews(ref doc, item);

			// add to content
			doc.Add(new DocumentField("__content", item.Name, new[] { IndexStore.YES, IndexType.ANALYZED, IndexDataType.StringCollection }));
			doc.Add(new DocumentField("__content", item.Code, new[] { IndexStore.YES, IndexType.ANALYZED, IndexDataType.StringCollection }));
		}

		protected virtual void IndexItemCustomProperties(ref ResultDocument doc, CatalogProduct item)
		{
			foreach (var propValue in item.PropertyValues.Where(x=>x.Value != null))
			{
				var properties = item.CategoryId != null ? _propertyService.GetCategoryProperties(item.CategoryId) : _propertyService.GetCatalogProperties(item.CatalogId);
				var property = properties.FirstOrDefault(x=> String.Equals(x.Name, propValue.PropertyName, StringComparison.InvariantCultureIgnoreCase) && x.ValueType == propValue.ValueType);
		
				var contentField = string.Format("__content{0}", property != null && (property.Multilanguage && !string.IsNullOrWhiteSpace(propValue.LanguageCode)) ? "_" + propValue.LanguageCode.ToLower() : string.Empty);

				switch (propValue.ValueType)
				{
					case PropertyValueType.LongText:
					case PropertyValueType.ShortText:
						doc.Add(new DocumentField(contentField, propValue.Value.ToString().ToLower(), new[] { IndexStore.YES, IndexType.ANALYZED, IndexDataType.StringCollection }));
						break;
				}

				if (doc.ContainsKey(propValue.PropertyName))
					continue;


				switch ((PropertyValueType)propValue.ValueType)
				{
					case PropertyValueType.Boolean:
					case PropertyValueType.DateTime:
					case PropertyValueType.Number:
						doc.Add(new DocumentField(propValue.PropertyName, propValue.Value, new[] { IndexStore.YES, IndexType.ANALYZED }));
						break;
					case PropertyValueType.LongText:
					case PropertyValueType.ShortText:
						doc.Add(new DocumentField(propValue.PropertyName, propValue.Value.ToString().ToLower(), new[] { IndexStore.YES, IndexType.ANALYZED }));
						break;
				}
			}
		}

		#region Category Indexing
		protected virtual void IndexItemCategories(ref ResultDocument doc, CatalogProduct item)
		{
			var category = _categoryService.GetById(item.CategoryId);
			doc.Add(new DocumentField(String.Format("sort{0}{1}", category.CatalogId, category.Id), category.Priority, new string[] { IndexStore.YES, IndexType.NOT_ANALYZED }));
			IndexCategory(ref doc, category);
		}

		protected virtual void IndexCategory(ref ResultDocument doc, Category category)
		{
			doc.Add(new DocumentField("catalog", category.CatalogId.ToLower(), new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));

			// get category path
			var outline = String.Join(";", category.Parents.Select(x=>x.Id));
			doc.Add(new DocumentField("__outline", outline.ToLower(), new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));

			// Now index all linked categories
			foreach (var link in category.Links)
			{
				var linkCategory = _categoryService.GetById(link.CategoryId);
				IndexCategory(ref doc, linkCategory);
			}
		}

		#endregion

		#region Price Lists Indexing
		protected virtual void IndexItemPrices(ref ResultDocument doc, CatalogProduct item)
		{
			var evalContext = new Domain.Pricing.Model.PriceEvaluationContext()
			{
				 ProductId = item.Id
			};

			var prices = _pricingService.EvaluateProductPrices(evalContext);

		
				foreach (var price in prices)
				{
					//var priceList = price.Pricelist;
					doc.Add(new DocumentField(String.Format("price_{0}_{1}", price.Currency, price.PricelistId), price.Sale ?? price.List, new[] { IndexStore.NO, IndexType.NOT_ANALYZED }));
					doc.Add(new DocumentField(String.Format("price_{0}_{1}_value", price.Currency, price.PricelistId), price.Sale == null ? price.List.ToString() : price.Sale.ToString(), new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));
				}

		}
		#endregion

		//protected virtual void IndexReviews(ref ResultDocument doc, CatalogProduct item)
		//{
		//	var reviews = ReviewRepository.Reviews.Where(r => r.ItemId == item.ItemId).ToArray();
		//	var count = reviews.Count();
		//	var avg = count > 0 ? Math.Round(reviews.Average(r => r.OverallRating), 2) : 0;
		//	doc.Add(new DocumentField("__reviewstotal", count, new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));
		//	doc.Add(new DocumentField("__reviewsavg", avg, new[] { IndexStore.YES, IndexType.NOT_ANALYZED }));
		//}

	


	
	}
}
