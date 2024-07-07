using FancyCandles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockVII.Data
{
    /// <summary>
    /// 股票信息
    /// </summary>
    public class StockInfo : ICandle
    {
        /// <summary>
        /// 成交额(元)
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// 涨跌额
        /// </summary>
        public double Change { get; set; }

        /// <summary>
        /// 涨跌幅(%)
        /// </summary>
        public double ChangePercent { get; set; }

        /// <summary>
        /// 收盘价
        /// </summary>
        public double Close { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 最高价
        /// </summary>
        public double High { get; set; }

        /// <summary>
        /// 最低价
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        /// 开盘价
        /// </summary>
        public double Open { get; set; }

        /// <summary>
        /// 换手率(%)
        /// </summary>
        public double TurnoverRate { get; set; }

        /// <summary>
        /// 成交量(股)
        /// </summary>
        public double Volume { get; set; }

        /// <summary>
        /// 均价(元/股)
        /// </summary>
        public double Average => Amount / Volume;

        public DateTime t => Date;

        public double O => Open;

        public double H => High;

        public double L => Low;

        public double C => Close;

        public double V => Volume;

        public override string ToString()
        {
            return $"{Open} / {Close}";
        }

        /// <summary>
        /// 序列化
        /// </summary>
        public string Serialize()
        {
            var res = new List<string>()
            {
                Date.ToString(),
                Open.ToString(),
                Close.ToString(),
                High.ToString(),
                Low.ToString(),
                TurnoverRate.ToString(),
                Volume.ToString(),
                Amount.ToString(),
                Change.ToString(),
                ChangePercent.ToString()
            };

            return string.Join(",", res);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="serializedData">序列化的字符串</param>
        /// <returns>反序列化后的 StockInfo 对象</returns>
        public static StockInfo Deserialize(string serializedData)
        {
            var parts = serializedData.Split(',');
            var stockInfo = new StockInfo
            {
                Date = DateTime.Parse(parts[0]),
                Open = double.Parse(parts[1]),
                Close = double.Parse(parts[2]),
                High = double.Parse(parts[3]),
                Low = double.Parse(parts[4]),
                TurnoverRate = double.Parse(parts[5]),
                Volume = double.Parse(parts[6]),
                Amount = double.Parse(parts[7]),
                Change = double.Parse(parts[8]),
                ChangePercent = double.Parse(parts[9])
            };

            return stockInfo;
        }
    }
}
