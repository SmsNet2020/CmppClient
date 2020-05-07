using DotNetty.Transport.Channels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Cmpp.Client
{
	public class SessionMap : IAsyncDisposable
	{
		ConcurrentDictionary<Guid, Session> sessionMap;

		public SessionMap()
		{
			sessionMap = new ConcurrentDictionary<Guid, Session>();
		}

		public void AddSession(Session session)
		{
			sessionMap.TryAdd(Guid.NewGuid(), session);
		}

		public void TryRemove(IChannel channel)
		{
			foreach (var kv in sessionMap)
			{
				if (kv.Value.Channel == channel)
				{
					sessionMap.TryRemove(kv.Key, out var _);
					return;
				}
			}
		}

		public void TryRemove(Session session)
		{
			TryRemove(session.Channel);
		}

		public Session GetLoginSuccSessionRandom()
		{
			return sessionMap.Values.Where(x => x.Send).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
		}

		public Session GetSessionOrDefault(IChannel channel)
		{
			return sessionMap.Values.Where(x => x.Channel == channel).FirstOrDefault();
		}

		public Session GetSessionOrDefault(IChannelHandlerContext ctx)
		{
			return GetSessionOrDefault(ctx.Channel);
		}

		public async ValueTask DisposeAsync()
		{
			foreach (var session in sessionMap.Values)
			{
				await session.Channel.CloseAsync();
			}
		}

		public int Count => sessionMap.Count;
	}
}
