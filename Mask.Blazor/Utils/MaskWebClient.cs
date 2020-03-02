using Mask.Blazor.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mask.Blazor.Utils
{
    public class MaskWebClient : WebClient
    {
        /// <summary>
        /// 抢口罩地址
        /// </summary>
        const string APPOINTMENT = @"https://national.eshiyun.info/masks-manage/api/main/getMaskMaskorder";

        /// <summary>
        /// 获取商店列表地址
        /// </summary>
        const string SHOPLIST = @"https://national.eshiyun.info/masks-manage/api/main/getMaskServiceinfoList";

        /// <summary>
        /// 用于检查是否预约成功
        /// </summary>
        const string MYORDER = @"https://national.eshiyun.info/masks-manage/api/main/getMaskMaskorderList";

        /// <summary>
        /// 刷新RSA公钥
        /// </summary>
        const string RSAKEY = @"https://national.eshiyun.info/appportal/api/main/rsaKey";

        private int timeout;

        /// <summary>
        /// 设置UA,content-type,编码
        /// </summary>
        public MaskWebClient()
        {
            this.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.122 Safari/537.36");
            this.Headers.Add(HttpRequestHeader.ContentType, "application/json;charset=UTF-8");
            this.Headers.Add("X-Requested-With", "mobi.wonders.android.apps.smy");
            this.Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// 获取商店列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<ShopModel>> GetShopList()
        {
            try
            {
                this.timeout = 30000;
                var json = await this.UploadStringTaskAsync(SHOPLIST, "POST", @"{""serviceName"":"""",""page"":1,""limit"":200,""lng"":""110.299877"",""lat"":""20.014208"",""area"":""""}");
                return JArray.Parse(JObject.Parse(JObject.Parse(json)["result"].ToString())["list"].ToString()).Select(p => new ShopModel()
                {
                    serviceAddress = p["serviceAddress"].ToString(),
                    serviceName = p["serviceName"].ToString(),
                    id = p["id"].ToString()
                }).OrderBy(p => p.serviceName).ToList();
            }
            catch
            {
                throw new Exception("获取药店列表失败");
            }
        }

        /// <summary>
        /// 预约口罩
        /// </summary>
        /// <param name="parm"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool MakeAppointment(RequestParm parm, out string result)
        {
            var json = JsonConvert.SerializeObject(parm);
            try
            {
                this.timeout = 5000;
                result = this.UploadString(APPOINTMENT, "POST", json);
                if (result.Contains("200") || result.Contains("成功") || result.Contains("5天"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                result = string.Empty;
                return false;
            }
        }

        public bool IsSuccessed(string idCard, string targetDate, out string code)
        {
            var json = JsonConvert.SerializeObject(new { idcard = idCard.RSAEncrypt(), limit = 20, page = 1, subscribeChannel = 0 });
            this.timeout = 5000;
            var result = this.UploadString(MYORDER, "POST", json);
            if (result.Contains(targetDate))
            {
                code = JArray.Parse(JObject.Parse(JObject.Parse(result)["result"].ToString())["list"].ToString()).First(p => p["subscribeDate"].ToString() == targetDate)["orderNo"].ToString();
                return true;
            }
            else
            {
                code = string.Empty;
                return false;
            }
        }

        public string GetRSAKey()
        {
            try
            {
                var json = this.UploadString(RSAKEY, "POST", "");
                return JObject.Parse(JObject.Parse(json)["data"].ToString())["rsaPublicKey"].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 设置超时时间 30 秒
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            var req = base.GetWebRequest(address);
            req.Timeout = 10000;
            return req;
        }
    }
}
