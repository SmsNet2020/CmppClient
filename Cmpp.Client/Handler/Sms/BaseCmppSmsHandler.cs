using Sms.Protocol.Cmpp2;
using Sms.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Sms.Protocol;

namespace Cmpp.Client.Handler.Sms
{
	public abstract class BaseCmppSmsHandler
	{

		private MyMemoryCache cache = MyMemoryCache.CreateNew(null);

		public IClient client;

		public virtual void Cmpp2LongSmsRespondAssembly(CmppMessageReceiveArgs<CmppHead, CmppSubmitResp, Item<MsgEx>> msgEx)
		{
			List<CmppMessageReceiveArgs<CmppHead, CmppSubmitResp, Item<MsgEx>>> listSubmitResp;
			var item = msgEx.Item;
			var cmppPacket = item.Request as SmsPacket;
			if (item.BusinessEx.IsLong)//长短信
			{
				if (!Cmpp2LongSmsRespondAssemblyCache(msgEx))//等批
				{
					return;
				}
				listSubmitResp = cache
				.GetCacheItem<List<CmppMessageReceiveArgs<CmppHead, CmppSubmitResp, Item<MsgEx>>>>(msgEx.Item.BusinessEx.Id);
			}
			else
			{
				listSubmitResp = new List<CmppMessageReceiveArgs<CmppHead, CmppSubmitResp, Item<MsgEx>>>() { msgEx };
			}

			this.Cmpp2SubmitResp(listSubmitResp);
		}

		public abstract void CmppDeliver(CmppMessageReceiveArgs<CmppHead, CmppDeliver> msg);
		/// <summary>
		/// 连接成功后回调
		/// </summary>
		public abstract void ConnectSuccessCallBack();
		/// <summary>
		/// 连接断开后的回调
		/// </summary>
		public abstract void ConnectCloseCompleteCallBack();

		protected abstract void Cmpp2SubmitResp(List<CmppMessageReceiveArgs<CmppHead, CmppSubmitResp, Item<MsgEx>>> msgs);

		public abstract void SubmitTimeOutHandle(Item<MsgEx> item);

		private bool Cmpp2LongSmsRespondAssemblyCache(CmppMessageReceiveArgs<CmppHead, CmppSubmitResp, Item<MsgEx>> msgEx)
		{
			var listSubmitRespCache = cache
				.GetCacheItem<List<CmppMessageReceiveArgs<CmppHead, CmppSubmitResp, Item<MsgEx>>>>(msgEx.Item.BusinessEx.Id);
			var result = false;
			if (listSubmitRespCache == null)//不存在响应
			{
				listSubmitRespCache = new List<CmppMessageReceiveArgs<CmppHead, CmppSubmitResp, Item<MsgEx>>>() { msgEx };
				result = false;
			}
			else//存在响应
			{
				listSubmitRespCache.AddRange(listSubmitRespCache);
				result = listSubmitRespCache.Count == msgEx.Item.BusinessEx.Count;
			}
			DateTime absoluteExpiration = DateTime.Now.AddSeconds(60);
			cache.GetOrAddCacheItem(msgEx.Item.BusinessEx.Id, listSubmitRespCache, null, absoluteExpiration);
			return result;
		}
	}
}
