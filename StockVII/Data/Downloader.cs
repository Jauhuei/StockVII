using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockVII.Data
{
    /// <summary>
    /// 下载器
    /// </summary>
    public static class Downloader
    {
        /// <summary>
        /// 获取沪深 300 公司数据。
        /// </summary>
        public static async Task<List<CompanyInfo>> GetHS300CompanyInfosAsync()
        {
            try
            {
                var url = "https://datacenter-web.eastmoney.com/api/data/v1/get?sortColumns=SECURITY_CODE&sortTypes=-1&pageSize=500&pageNumber=1&reportName=RPT_INDEX_TS_COMPONENT&columns=SECUCODE%2CSECURITY_CODE%2CTYPE%2CSECURITY_NAME_ABBR%2CCLOSE_PRICE%2CINDUSTRY%2CREGION%2CWEIGHT%2CEPS%2CBPS%2CROE%2CTOTAL_SHARES%2CFREE_SHARES%2CFREE_CAP&quoteColumns=f2%2Cf3&quoteType=0&source=WEB&client=WEB&filter=(TYPE%3D%221%22)";
                using (var http = new HttpClient())
                {
                    var response = await http.GetAsync(url);
                    var content = await response.Content.ReadAsStringAsync();

                    var json = JsonConvert.DeserializeObject<JToken>(content);
                    return json["result"]["data"].Select(x =>
                    new CompanyInfo()
                    {
                        Code = x["SECURITY_CODE"].ToString(),
                        Name = x["SECURITY_NAME_ABBR"].ToString(),
                        Industry = x["INDUSTRY"].ToString(),
                        Region = x["REGION"].ToString(),
                        Weight = Convert.ToDouble(x["WEIGHT"]),
                        EPS = Convert.ToDouble(x["EPS"]),
                        BPS = Convert.ToDouble(x["BPS"]),
                        ROE = Convert.ToDouble(x["ROE"])
                    }).OrderBy(c => c.Code)
                    .ToList();
                }
            }
            catch (Exception)
            {
                return new List<CompanyInfo>();
            }
        }
    }
}
