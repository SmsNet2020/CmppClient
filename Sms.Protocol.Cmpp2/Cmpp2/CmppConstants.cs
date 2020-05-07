namespace Sms.Protocol.Cmpp2
{
    /// <summary>
    /// 数据编码格式。
    /// </summary>
    public enum CmppEncoding
    {
        /// <summary>
        /// ASCII 编码。
        /// </summary>
        ASCII = 0,
        /// <summary>
        /// 二进制数据。
        /// </summary>
        Binary = 4,
        /// <summary>
        /// UCS2 编码。
        /// </summary>
        UCS2 = 8,
        /// <summary>
        /// 特殊UCS2 编码。
        /// </summary>
        Special = 9,
        /// <summary>
        /// GB2312 编码。
        /// </summary>
        GBK = 15,
        /// <summary>
        /// 闪信 
        /// </summary>
        Flash = 24
    }

    /// <summary>
    /// 计费用户。
    /// </summary>
    public enum FeeUserType : byte
    {
        /// <summary>
        /// 对源终端计费。
        /// </summary>
        From = 1,
        /// <summary>
        /// 对目的终端计费。
        /// </summary>
        Termini = 0,
        /// <summary>
        /// 对 SP 计费。
        /// </summary>
        SP = 2,
        /// <summary>
        /// 对指定用户计费（由 feeUser 指定）。
        /// </summary>
        FeeUser = 3
    }

    /// <summary>
    /// 资费类别。
    /// </summary>
    public enum FeeType : byte
    {
        /// <summary>
        /// 对“计费用户号码”免费。
        /// </summary>
        Free = 1,
        /// <summary>
        /// 对“计费用户号码”按条计信息费。
        /// </summary>
        One = 2,
        /// <summary>
        /// 对“计费用户号码”按包月收取信息费。
        /// </summary>
        Month = 3
    }

    /// <summary>
    /// Constants for CMPP 2.0
    /// </summary>
    public static class CmppConstants
    {
        public const int LongMessageHeadSize = 140; 

        /// <summary>
        /// CMPP 版本。
        /// </summary>
        public const byte Version = 0x20;
        /// <summary>
        /// Size of the cmpp header.
        /// </summary>
        public const int HeaderSize = 12;
        /// <summary>
        /// Size of fixes sized messages.
        /// </summary>
        public static class PackageBodySize
        {
            /// <summary>
            /// CMPP_CANCEL
            /// </summary>
            public const int CmppCancel = 8;
            /// <summary>
            /// CMPP_CANCEL_RESP
            /// </summary>
            public const int CmppCancelResp = 4;
            /// <summary>
            /// CMPP_ACTIVE_TEST
            /// </summary>
            public const int CmppActiveTest = 0;
            /// <summary>
            /// CMPP_ACTIVE_TEST_RESP
            /// </summary>
            public const int CmppActiveTestResp = 1;
            /// <summary>
            /// CMPP_CONNECT
            /// </summary>
            public const int CmppConnect = 27;
            /// <summary>
            /// CMPP_CONNECT_RESP
            /// </summary>
            public const int CmppConnectResp = 18;
            /// <summary>
            /// CMPP_DILIVER_RESP
            /// </summary>
            public const int CmppDeliverResp = 12;
            /// <summary>
            /// CMPP_REPORT
            /// </summary>
            public const int CmppReport = 60;
            /// <summary>
            /// CMPP_SUBIT_RESP
            /// </summary>
            public const int CmppSubmitResp = 9;
        }

        /// <summary>
        /// SMS Encoding Options
        /// </summary>
        public static class Encoding
        {
            #region SMS数据格式定义
            /// <summary>
            /// ASCII 串。
            /// </summary>
            public const byte ASCII = 0;
            /// <summary>
            /// 二进制信息。
            /// </summary>
            public const byte Binary = 4;
            /// <summary>
            /// UCS2编码。
            /// </summary>
            public const byte UCS2 = 8;
            /// <summary>
            /// 特殊UCS2编码。
            /// </summary>
            public const byte Special = 9;
            /// <summary>
            /// 含GB汉字。
            /// </summary>
            public const byte GBK = 15;
            /// <summary>
            /// 特殊UCS2编码。
            /// </summary>
            public const byte Flash = 24;
            #endregion
        }

        #region COMMAND_ID 定义
        /// <summary>
        /// CMPP message command id list.
        /// </summary>
        public static class CommandCode
        {
            /// <summary>
            /// 请求连接。
            /// </summary>
            public const uint Connect = 0x00000001;
            /// <summary>
            /// 请求连接应答。
            /// </summary>
            public const uint ConnectResp = 0x80000001;
            /// <summary>
            /// 终止连接。
            /// </summary>
            public const uint Terminate = 0x00000002;
            /// <summary>
            /// 终止连接应答。
            /// </summary>
            public const uint TerminateResp = 0x80000002;
            /// <summary>
            /// 提交短信。
            /// </summary>
            public const uint Submit = 0x00000004;
            /// <summary>
            /// 提交短信应答。
            /// </summary>
            public const uint SubmitResp = 0x80000004;
            /// <summary>
            /// 短信下发。
            /// </summary>
            public const uint Deliver = 0x00000005;
            /// <summary>
            /// 下发短信应答。
            /// </summary>
            public const uint DeliverResp = 0x80000005;
            /// <summary>
            /// 发送短信状态查询。
            /// </summary>
            public const uint Query = 0x00000006;
            /// <summary>
            /// 发送短信状态查询应答。
            /// </summary>
            public const uint QueryResp = 0x80000006;
            /// <summary>
            /// 删除短信。
            /// </summary>
            public const uint Cancel = 0x00000007;
            /// <summary>
            /// 删除短信应答。
            /// </summary>
            public const uint CancelResp = 0x80000007;
            /// <summary>
            /// 激活测试。
            /// </summary>
            public const uint ActiveTest = 0x00000008;
            /// <summary>
            /// 激活测试应答。
            /// </summary>
            public const uint ActiveTestResp = 0x80000008;
            /// <summary>
            /// 网络故障。
            /// </summary>
            public const uint Error = 0xFFFFFFFF;
        }
        #endregion
        public static class ConnectRespStatus
        {
            /// <summary>
            /// 正确
            /// </summary>
            public const byte Success = 0;
            /// <summary>
            /// 消息结构错
            /// </summary>
            public const byte MessageError = 1;
            /// <summary>
            /// 非法源地址
            /// </summary>
            public const byte Illegal = 2;
            /// <summary>
            /// 认证错
            /// </summary>
            public const byte IdentityError = 3;
            /// <summary>
            /// 版本太高
            /// </summary>
            public const byte VersionHigh = 4;
            /// <summary>
            /// 其他错误
            /// </summary>
            public const byte OtherError = 5;
            /// <summary>
            /// 长短信错误
            /// </summary>
            public const byte LongSmsError = 6;
            /// <summary>
            /// 超速
            /// </summary>
            public const byte Overspeed = 8;
            /// <summary>
            /// 不存在业务
            /// </summary>
            public const byte NonexistentBusiness = 9;
            /// <summary>
            /// 接入码错误
            /// </summary>
            public const byte AccessCodeError = 10;
            /// <summary>
            /// 不存在产品
            /// </summary>
            public const byte NonexistentProduct = 11;
            /// <summary>
            /// 写入队列失败
            /// </summary>
            public const byte WriteQueueFailed = 12;
            /// <summary>
            /// 添加用户失败
            /// </summary>
            public const byte AddUserFailed = 13;
            /// <summary>
            /// 添加会话失败
            /// </summary>
            public const byte AddSessionFailed = 14;
            /// <summary>
            /// 账户被禁用
            /// </summary>
            public const byte AccountDisabled = 100;
            /// <summary>
            /// 余额不足
            /// </summary>
            public const byte LackBalance = 101;
            /// <summary>
            /// 非法用户提交短信
            /// </summary>
            public const byte IllegalUserSubmitSms = 102;
            /// <summary>
            /// 连接过多
            /// </summary>
            public const byte MuchConnection = 110;
        }
    }
}
