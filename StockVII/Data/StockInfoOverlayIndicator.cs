using FancyCandles;
using FancyCandles.Indicators;
using System.Windows;
using System.Windows.Media;

namespace StockVII.Data
{
    public abstract class StockInfoOverlayIndicator : OverlayIndicator
    {
        public StockInfo GetStockInfo(int candle_i) => CandlesSource[candle_i] as StockInfo;

        public override void OnRender(DrawingContext drawingContext, IntRange visibleCandlesRange, CandleExtremums visibleCandlesExtremums, double candleWidth, double gapBetweenCandles, double RenderHeight)
        {
            _CandleWidthPlusGap = candleWidth + gapBetweenCandles;
            _CandleWidth = candleWidth;

            _VisibleCandlesRange = visibleCandlesRange;
            _VisibleCandlesExtremums = visibleCandlesExtremums;
            _RenderHeight = RenderHeight;
        }

        private double _CandleWidthPlusGap;
        private double _CandleWidth;
        private IntRange _VisibleCandlesRange;
        private CandleExtremums _VisibleCandlesExtremums;
        private double _RenderHeight;

        protected Point GetPoint(int candle_i, double price)
        {
            return new Point(GetX(candle_i), GetY(price));
        }

        protected Point GetPoint(int candle_i)
        {
            return new Point(GetX(candle_i), GetY(GetIndicatorValue(_VisibleCandlesRange.Start_i + candle_i)));
        }

        protected double GetX(int candle_i)
        {
            return candle_i * _CandleWidthPlusGap + 0.5 * _CandleWidth; ;
        }

        protected double GetY(double price)
        {
            double range = _VisibleCandlesExtremums.PriceHigh - _VisibleCandlesExtremums.PriceLow;
            return (1.0 - (price - _VisibleCandlesExtremums.PriceLow) / range) * _RenderHeight;
        }
    }
}
