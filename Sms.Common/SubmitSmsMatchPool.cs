using System;
using System.Collections.Concurrent;
using System.Runtime.Caching;
using System.Threading;

namespace Sms.Common
{
	public class SubmitSmsMatchPool<T>
	{
		MyMemoryCache cache;

		public Action<Item<T>> timeOutHandle;

		public int QueueMaxLength { get; set; }
		public int TimeOutMs { get; set; }

		public SubmitSmsMatchPool(int queueLength, int timeOutMs)
		{
			QueueMaxLength = queueLength;
			TimeOutMs = timeOutMs;
			cache = MyMemoryCache.CreateNew(RemovedCallback);
		}


		public bool Add(object owner, object request, T exObj, string seq)
		{
			if (cache.GetCount() >= QueueMaxLength)
			{
				return false;
			}

			var now = UnixTimestampMs();
			var timeout = now + TimeOutMs;
			var qi = new Item<T>
			{
				Owner = owner,
				Request = request,
				IsMatch = false,
				BusinessEx = exObj,
				EnTime = now
			};

			cache.GetOrAddCacheItem(seq, qi, null, DateTime.Now.AddMilliseconds(TimeOutMs));
			return true;
		}

		public bool Match(object owner, string seq, out Item<T> item)
		{
			item = cache.GetCacheItem<Item<T>>(seq);
			bool match = item != null;
			if (match)
			{
				var now = UnixTimestampMs();
				item.DeTime = now;
				cache.RemoveCacheItem(seq);
			}
			return item != null;
		}



		/// <summary>
		/// 格林威治时间1970年01月01日00时00分00秒(北京时间1970年01月01日08时00分00秒)起至现在的总毫秒数
		/// </summary>
		/// <returns></returns>
		public long UnixTimestampMs()
		{
			return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
		}

		private void RemovedCallback(CacheEntryRemovedArguments arguments)
		{
			if (arguments.RemovedReason == CacheEntryRemovedReason.Expired)
			{
				var item = arguments.CacheItem.Value as Item<T>;
				if (item != null)
				{
					timeOutHandle?.Invoke(item);
				}
			}
		}
	}


	public class Item<T>
	{
		/// <summary>
		/// 拥有者一般是channel
		/// </summary>
		public Object Owner { get; set; }

		/// <summary>
		/// cmpp二进制的数据包
		/// </summary>
		public Object Request { get; set; }

		/// <summary>
		/// 匹配
		/// </summary>
		public bool IsMatch { get; set; }

		/// <summary>
		/// 扩展业务
		/// </summary>
		public T BusinessEx { get; set; }

		/// <summary>
		/// 出队列的时间
		/// </summary>
		public long DeTime { get; set; }

		/// <summary>
		/// 进入队列的时间
		/// </summary>
		public long EnTime { get; set; }

		public override string ToString()
		{
			return $"{{{nameof(Owner)}={Owner}, {nameof(Request)}={Request}, {nameof(IsMatch)}={IsMatch.ToString()}, {nameof(BusinessEx)}={BusinessEx}, {nameof(DeTime)}={DeTime.ToString()}, {nameof(EnTime)}={EnTime.ToString()}}}";
		}
	}
}
