using System;

namespace Sms.Common
{
    public interface IGW : IAsyncDisposable
    {
        void Action(object obj);
    }

    public interface IOFGW : IGW
    {

	}

    public interface IIFGW : IGW
    {

    }
}
