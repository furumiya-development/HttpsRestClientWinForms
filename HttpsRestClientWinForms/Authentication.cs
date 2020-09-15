using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace HttpsRestClientWinForms
{
    public class Authentication
    {
        private IDictionary<string, string> DicParam = new Dictionary<string, string>();
        private string Uri = "";
        private string User = "";
        private const string BASIC = "Basic ";
        private const string DIGEST = "Digest ";
        private const string AUTHENTICATE = "WWW-Authenticate";
        private const string MD5 = "MD5";
        private const string SHA1 = "SHA1PRNG";
        private string Wnonce = "";
        private string WnonceCnt = "00000001";
        private string Wcnonce = "";

        public Authentication()
        {
        }

        public IDictionary<string, string> Analysis(IDictionary<string, List<string>> dics)
        {
            string Auths = dics[AUTHENTICATE][0];

            if (Auths.StartsWith(BASIC))
            {
                DicParam.Add(AUTHENTICATE, BASIC.Replace(" ", ""));
                Auths = Auths.Replace(BASIC, "");
                Match mkey = new Regex("(.*?)=\"").Match(Auths);
                Match mvalue = new Regex("\"(.*?)\"").Match(Auths);
                DicParam.Add(mkey.Groups[1].Value, mvalue.Groups[1].Value);
            }
            else if (Auths.StartsWith(DIGEST))
            {
                DicParam.Add(AUTHENTICATE, DIGEST.Replace(" ", ""));
                Auths = Auths.Replace(DIGEST, "");
                DigestAnalysis(Auths);
            }

            return DicParam;
        }

        public static string BasicRequestHeader()
        {
            string base64str = BASIC;

            base64str = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format(UserID + ":" + Password)));

            return base64str;
        }

        private void DigestAnalysis(string auths)
        {
            string[] auth = auths.Split(", ");
            foreach (string one in auth)
            {
                //項目の取り出し
                //()で囲むと一致する部分を取り出すgroup(1)でその文字(ここでは=)を除去
                Match mkey = GetRegexMatch("(.*?)=\"", one);
                //""で囲まれた内容を取り出し項目とともにMapへ格納
                Match mvalue = GetRegexMatch("\"(.*?)\"", one);
                DicParam.Add(mkey.Groups[1].Value, mvalue.Groups[1].Value);
            }
        }

        private Match GetRegexMatch(string pattern, string mstr)
        {
            return new Regex(pattern).Match(mstr);
        }

        public string DigestRequest(string res)
        {
            string req = DIGEST + "username=\"" + this.User + "\", realm=\"" + DicParam["realm"] + "\", nonce=\"";
            req += DicParam["nonce"] + "\", uri=\"" + this.Uri + "\", algorithm=MD5, qop=";
            req += DicParam["qop"] + ", nc=" + WnonceCnt + ", cnonce=\"" + Wcnonce + "\", response=\"" + res + "\"";
            //System.Diagnostics.Debug.WriteLine(req);

            return req;
        }

        public string DigestRespons(string a1, string a2, string nonce, string qop)
        {
            string res = "";

            //if(Wnonce != nonce)
            //{

            //}
            Wcnonce = GetRandomStr();
            res = a1 + ":" + nonce + ":" + WnonceCnt + ":" + Wcnonce + ":" + qop + ":" + a2;
            res = ToMd5(res);

            return res;
        }

        public string DigestResponseA1(string user, string pass, string realm)
        {
            this.User = user;
            return ToMd5((user + ":" + realm + ":" + pass));
        }

        public string DigestResponseA2(string method)
        {
            return ToMd5((method + ":" + this.Uri));
        }

        private string ToMd5(string md5str)
        {
            var md5hex = new StringBuilder();
            using (var md5 = new MD5CryptoServiceProvider())
            {
                byte[] md5byte = md5.ComputeHash(Encoding.UTF8.GetBytes(md5str));

                foreach (byte b in md5byte)
                {
                    md5hex.Append(b.ToString("x2"));
                }
            }

            return md5hex.ToString();
        }

        private string GetRandomStr()
        {
            var rndstr = "abcdefghijklmnopqrstuvwxyz0123456789";
            var bytes = new byte[8];
            var randhex = new StringBuilder();

            using (var rand = new RNGCryptoServiceProvider())
            {
                for (var i = 0; i < 16; i++)
                {
                    rand.GetBytes(bytes);
                    var seed = BitConverter.ToInt32(bytes, 0);
                    var pos = (new Random(seed)).Next(rndstr.Length);
                    var c = rndstr.Substring(pos, 1);
                    randhex.Append(c);
                }
            }

            return randhex.ToString();
        }

        public static string UserID { get; set; }
        public static string Password { get; set; }
    }
}