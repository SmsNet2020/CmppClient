//using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Caching;
using System.Text;

namespace Sms.Common
{
	public class MyMemoryCache: System.Runtime.Caching.MemoryCache
	{

		public CacheEntryRemovedCallback RemovedCallback { get; set; }

		private  readonly Object _locker = new object(), _locker2 = new object();

		private MyMemoryCache(string name, NameValueCollection config = null) : base(name, config)
		{

		}


		public static MyMemoryCache CreateNew(CacheEntryRemovedCallback RemovedCallback) {
			var cache = new MyMemoryCache(Guid.NewGuid().ToString());
			cache.RemovedCallback = RemovedCallback;
			return cache;
		}

		/// <summary>
		/// 取缓存项，如果不存在则返回空
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public  T GetCacheItem<T>(String key)
		{
			try
			{
				return (T)this[key];
			}
			catch
			{
				return default(T);
			}
		}

		/// <summary>
		/// 取缓存项,如果不存在则新增缓存项
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="cachePopulate"></param>
		/// <param name="slidingExpiration"></param>
		/// <param name="absoluteExpiration"></param>
		/// <returns></returns>
		public T GetOrAddCacheItem<T>(String key, T cacheValue, TimeSpan? slidingExpiration = null, DateTime? absoluteExpiration = null)
		{
			if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("Invalid cache key");
			//if (cacheValue == null) throw new ArgumentNullException("cachePopulate");
			if (slidingExpiration == null && absoluteExpiration == null) throw new ArgumentException("Either a sliding expiration or absolute must be provided");

			if (this[key] == null)
			{
				lock (_locker)
				{
					if (this[key] == null)
					{
						//T cacheValue = cachePopulate();
						if (!typeof(T).IsValueType && ((object)cacheValue) == null) //如果是引用类型且为NULL则不存缓存
						{
							return cacheValue;
						}

						var item = new CacheItem(key, cacheValue);
						var policy = CreatePolicy(slidingExpiration, absoluteExpiration);

						Add(item, policy);
					}
				}
			}

			return (T)System.Runtime.Caching.MemoryCache.Default[key];
		}

		/// <summary>
		/// 取缓存项,如果不存在则新增缓存项
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="cachePopulate"></param>
		/// <param name="dependencyFilePath"></param>
		/// <returns></returns>
		public T GetOrAddCacheItem<T>(String key, Func<T> cachePopulate, string dependencyFilePath)
		{
			if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("Invalid cache key");
			if (cachePopulate == null) throw new ArgumentNullException("cachePopulate");

			if (this[key] == null)
			{
				lock (_locker2)
				{
					if (this[key] == null)
					{
						T cacheValue = cachePopulate();
						if (!typeof(T).IsValueType && ((object)cacheValue) == null) //如果是引用类型且为NULL则不存缓存
						{
							return cacheValue;
						}

						var item = new CacheItem(key, cacheValue);
						var policy = CreatePolicy(dependencyFilePath);

						Add(item, policy);
					}
				}
			}

			return (T)this[key];
		}

		/// <summary>
		/// 移除指定键的缓存项
		/// </summary>
		/// <param name="key"></param>
		public  void RemoveCacheItem(string key)
		{
			try
			{
				Remove(key);
			}
			catch
			{ }
		}

		private CacheItemPolicy CreatePolicy(TimeSpan? slidingExpiration, DateTime? absoluteExpiration)
		{
			var policy = new CacheItemPolicy();

			if (absoluteExpiration.HasValue)
			{
				policy.AbsoluteExpiration = absoluteExpiration.Value;
			}
			else if (slidingExpiration.HasValue)
			{
				policy.SlidingExpiration = slidingExpiration.Value;
			}

			policy.Priority = CacheItemPriority.Default;
			policy.RemovedCallback = RemovedCallback;
			return policy;
		}

		private CacheItemPolicy CreatePolicy(string filePath)
		{
			CacheItemPolicy policy = new CacheItemPolicy();
			policy.ChangeMonitors.Add(new HostFileChangeMonitor(new List<string>() { filePath }));
			policy.Priority = CacheItemPriority.Default;
			return policy;
		}
	}
}

