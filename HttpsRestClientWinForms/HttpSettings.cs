using System;
using System.Collections.Generic;
using System.Text;

namespace HttpsRestClientWinForms
{
    public class HttpSettings
    {
        private const string HTTP1_0 = "HTTP1.0";
        private const string HTTP1_1 = "HTTP1.1";
        private const string HTTP2 = "HTTP2";
        public string[] HttpVersion = { "", HTTP1_0, HTTP1_1, HTTP2 };
        public Dictionary<System.Net.SecurityProtocolType, string> SslProtocolDic = new Dictionary<System.Net.SecurityProtocolType, string>();
        private const string TLS1_1 = "TLSv1.1";
        private const string TLS1_2 = "TLSv1.2";
        private const string TLS1_3 = "TLSv1.3";
        public string[] SslProtocol = { "", TLS1_1, TLS1_2, TLS1_3 };

        public HttpSettings()
        {
            HttpVer = System.Net.HttpVersion.Version11;
            SslProtocolVer = System.Net.SecurityProtocolType.Tls12;
        }

        public Version HttpVer { get; set; }
        private string _HttpVerStr;
        public string HttpVerStr
        {
            get
            {
                return _HttpVerStr;
            }
            set
            {
                switch (value)
                {
                    case HTTP1_0:
                        HttpVer = System.Net.HttpVersion.Version10;
                        break;
                    case HTTP1_1:
                        HttpVer = System.Net.HttpVersion.Version11;
                        break;
                    case HTTP2:
                        HttpVer = System.Net.HttpVersion.Version20;
                        break;
                    default:
                        HttpVer = System.Net.HttpVersion.Unknown;
                        break;
                }
            }
        }

        public System.Net.SecurityProtocolType SslProtocolVer { get; set; }
        private string _SslProtocolVerStr;
        public string SslProtocolVerStr
        {
            get
            {
                return _SslProtocolVerStr;
            }
            set
            {
                switch (value)
                {
                    case TLS1_1:
                        SslProtocolVer = System.Net.SecurityProtocolType.Tls11;
                        //_sps = value;
                        break;
                    case TLS1_2:
                        SslProtocolVer = System.Net.SecurityProtocolType.Tls12;
                        break;
                    case TLS1_3:
                        SslProtocolVer = System.Net.SecurityProtocolType.Tls13;
                        break;
                    default:
                        SslProtocolVer = System.Net.SecurityProtocolType.SystemDefault;
                        break;
                }
            }

        }
    }
}