using FancyCandles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockVII.Data
{
    /// <summary>
    /// 公司信息
    /// </summary>
    public class CompanyInfo : ICandlesSource
    {
        /// <summary>
        /// 最小时间
        /// </summary>
        public static DateTime MinTime { get; set; } = new DateTime(2000, 1, 1);

        /// <summary>
        /// 每股净资产
        /// </summary>
        public double BPS { get; set; }

        /// <summary>
        /// 股票代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 每股收益
        /// </summary>
        public double EPS { get; set; }

        /// <summary>
        /// 行业
        /// </summary>
        public string Industry { get; set; }

        /// <summary>
        /// 最后更新日期
        /// </summary>
        public DateTime LastUpdate { get; set; } = MinTime;

        /// <summary>
        /// 公司名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 净资产收益率
        /// </summary>
        public double ROE { get; set; }

        /// <summary>
        /// 股票信息
        /// </summary>
        public List<StockInfo> StockInfos { get; set; } = new List<StockInfo>();

        /// <summary>
        /// 权重
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// 从档案中读取
        /// </summary>
        private void ReadFromFile()
        {
            var file = $"./Stock/{Code}.txt";
            if (File.Exists(file))
            {
                var data = File.ReadAllLines(file);
                LastUpdate = DateTime.Parse(data[0]);

                var stockInfo = new StockInfo();
                StockInfos = data.Skip(1).Select(d => StockInfo.Deserialize(d)).ToList();
            }
        }

        /// <summary>
        /// 将档案存到文件中
        /// </summary>
        private void WriteToFile()
        {
            var file = $"./Stock/{Code}.txt";
            var data = new List<string>() { LastUpdate.ToString() };
            data.AddRange(StockInfos.Select(s => s.Serialize()));
            File.WriteAllLines(file, data);
        }

        /// <summary>
        /// 下载并更新本地数据
        /// </summary>
        public async Task DownloadAndSaveAsync()
        {
            try
            {
                ReadFromFile();
                if (Newest) return;

                // 下载数据
                var now = DateTime.Now;
                var url = $"https://q.stock.sohu.com/hisHq?code=cn_{Code}&start={LastUpdate:yyyyMMdd}&end=20501231&stat=1&order=D&period=d&rt=jsonp";
                using (var http = new HttpClient())
                {
                    var response = await http.GetStringAsync(url);

                    var content = response.Replace("callback([", "").Replace("])", "");
                    var json = JsonConvert.DeserializeObject<JToken>(content);

                    var stockInfos = json["hq"].Value<JArray>().Reverse().ToList()
                        .Select(line => new StockInfo
                        {
                            Date = DateTime.Parse(line[0].ToString()),
                            Open = double.Parse(line[1].ToString()),
                            Close = double.Parse(line[2].ToString()),
                            Change = double.Parse(line[3].ToString()),
                            ChangePercent = double.Parse(line[4].ToString().TrimEnd('%')),
                            Low = double.Parse(line[5].ToString()),
                            High = double.Parse(line[6].ToString()),
                            Volume = double.Parse(line[7].ToString()) * 100,
                            Amount = Math.Round(double.Parse(line[8].ToString()) * 10000, 2),
                            TurnoverRate = double.Parse(line[9].ToString().TrimEnd('%')),
                        }).Where(s => s.Date > new DateTime(2000, 1, 1)).ToList();

                    if (stockInfos.Any())
                    {
                        var firstDate = stockInfos.Min(s => s.Date);
                        StockInfos.RemoveAll(s => s.Date >= firstDate);
                    }
                    StockInfos.AddRange(stockInfos);

                    LastUpdate = now;
                    WriteToFile();
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 数据是否已是最新
        /// </summary>
        public bool Newest
        {
            get
            {
                var today = DateTime.Today;
                if (DateTime.Now < today.AddHours(9).AddMinutes(25)) return LastUpdate > today;
                if (DateTime.Now < today.AddHours(15).AddMinutes(5)) return LastUpdate > DateTime.Now.AddMilliseconds(1);
                return LastUpdate > today;
            }
        }

        public TimeFrame TimeFrame => TimeFrame.Daily;

        public int Count => StockInfos.Count;

        public bool IsReadOnly => true;

        public ICandle this[int index] { get => StockInfos[index]; set => StockInfos[index] = (StockInfo)value; }

        public override string ToString()
        {
            return $"{Code} {Name}";
        }

        public void Clear()
        {
            StockInfos.Clear();
        }

        public int IndexOf(ICandle item)
        {
            return StockInfos.IndexOf((StockInfo)item);
        }

        public void Insert(int index, ICandle item)
        {
            StockInfos.Insert(index, (StockInfo)item);
        }

        public void RemoveAt(int index)
        {
            StockInfos.RemoveAt(index);
        }

        public void Add(ICandle item)
        {
            StockInfos.Add((StockInfo)item);
        }

        public bool Contains(ICandle item)
        {
            return StockInfos.Contains((StockInfo)item);
        }

        public void CopyTo(ICandle[] array, int arrayIndex)
        {
            StockInfos.Cast<ICandle>().ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(ICandle item)
        {
            return StockInfos.Remove((StockInfo)item);
        }

        public IEnumerator<ICandle> GetEnumerator()
        {
            return StockInfos.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return StockInfos.GetEnumerator();
        }
    }
}
