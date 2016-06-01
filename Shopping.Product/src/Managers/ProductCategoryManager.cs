﻿using DryIoc;
using DryIocAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Cache.Interfaces;
using ZKWeb.Plugins.Common.Base.src.Repositories;
using ZKWeb.Plugins.Shopping.Product.src.Database;
using ZKWeb.Plugins.Shopping.Product.src.Model;
using ZKWeb.Server;
using ZKWeb.Utils.Collections;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Plugins.Shopping.Product.src.Managers {
	/// <summary>
	/// 商品类目管理器
	/// </summary>
	[ExportMany, SingletonReuse]
	public class ProductCategoryManager : ICacheCleaner {
		/// <summary>
		/// 商品类目的缓存时间，默认是180秒
		/// </summary>
		public TimeSpan CategoryCacheTime { get; set; }
		/// <summary>
		/// 类目的缓存
		/// 缓存中的类目包含属性和属性值
		/// </summary>
		protected MemoryCache<long, ProductCategory> CategoryCache { get; set; }
		/// <summary>
		/// 类目列表的缓存
		/// 缓存中的类目不包含属性和属性值
		/// </summary>
		protected MemoryCache<int, List<ProductCategory>> CategoryListCache { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public ProductCategoryManager() {
			var configManager = Application.Ioc.Resolve<ConfigManager>();
			CategoryCacheTime = TimeSpan.FromSeconds(
				configManager.WebsiteConfig.Extra.GetOrDefault(ExtraConfigKeys.ProductCategoryCacheTime, 180));
			CategoryCache = new MemoryCache<long, ProductCategory>();
			CategoryListCache = new MemoryCache<int, List<ProductCategory>>();
		}

		/// <summary>
		/// 查找类目，找不到时返回null
		/// </summary>
		/// <param name="categoryId">类目Id</param>
		/// <returns></returns>
		public virtual ProductCategory FindCategory(long categoryId) {
			// 从缓存获取
			var category = CategoryCache.GetOrDefault(categoryId);
			if (category != null) {
				return category;
			}
			// 从数据库获取
			UnitOfWork.ReadData<ProductCategory>(r => {
				category = r.Get(c => c.Id == categoryId && !c.Deleted);
				// 同时获取属性信息，并保存到缓存
				if (category != null) {
					category.Properties.ToList();
					category.Properties.SelectMany(p => p.PropertyValues).ToList();
					CategoryCache.Put(categoryId, category, CategoryCacheTime);
				}
			});
			return category;
		}

		/// <summary>
		/// 获取类目列表
		/// </summary>
		/// <returns></returns>
		public virtual IReadOnlyList<ProductCategory> GetCategoryList() {
			// 从缓存获取
			var categoryList = CategoryListCache.GetOrDefault(0);
			if (categoryList != null) {
				return categoryList;
			}
			// 从数据库获取
			categoryList = UnitOfWork.ReadData<ProductCategory, List<ProductCategory>>(r => {
				return r.GetMany(c => !c.Deleted).ToList();
			});
			// 保存到缓存
			CategoryListCache.Put(0, categoryList, CategoryCacheTime);
			return categoryList;
		}

		/// <summary>
		/// 清理缓存
		/// </summary>
		public virtual void ClearCache() {
			CategoryCache.Clear();
			CategoryListCache.Clear();
		}
	}
}
