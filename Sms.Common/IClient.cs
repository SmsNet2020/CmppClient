using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sms.Common
{
	public interface IClient : IAsyncDisposable
	{
		Task<bool> SubmitSmsAsync(Sms sms);
	}
}
