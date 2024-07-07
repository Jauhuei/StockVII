using FancyCandles;
using System.Windows.Media;

namespace StockVII.Data
{
    public class Average : StockInfoOverlayIndicator
    {
        private Pen pen = new Pen(Brushes.Black, 1);

        public Pen Pen
        {
            get { return pen; }
            set
            {
                pen = (Pen)value.GetCurrentValueAsFrozen();
                OnPropertyChanged();
            }
        }

        public override string ShortName => "日均价";

        public override string FullName => "日均价";

        public static string StaticName => "日均价";

        public override string PropertiesEditorXAML
        {
            get
            {
                string xaml = $@"
                            <StackPanel>
                                <StackPanel.Resources>
                                    <fp:SymStringToNumberConverter x:Key=""symStringToNumberConverter""/>

                                    <Style x:Key=""horizontalCaption_"" TargetType=""TextBlock"">
                                        <Setter Property=""Margin"" Value=""0 0 5 2""/>
                                        <Setter Property=""VerticalAlignment"" Value=""Bottom""/>
                                    </Style>

                                    <Style x:Key=""settingsItem_"" TargetType=""StackPanel"">
                                        <Setter Property=""Orientation"" Value=""Horizontal""/>
                                        <Setter Property=""FrameworkElement.HorizontalAlignment"" Value=""Left""/>
                                        <Setter Property=""FrameworkElement.Margin"" Value=""0 8 0 0""/>
                                    </Style>
                                </StackPanel.Resources>

                                <StackPanel Style=""{{StaticResource settingsItem_}}"">
                                    <TextBlock Style=""{{StaticResource horizontalCaption_}}"">Line:</TextBlock>
                                    <fp:PenSelector SelectedPen=""{{Binding Pen, Mode = TwoWay}}"" VerticalAlignment=""Bottom""/>
                                </StackPanel>

                            </StackPanel>";

                return xaml;
            }
        }

        public override double GetIndicatorValue(int candle_i)
        {
            return GetStockInfo(candle_i).Average;
        }

        public override void OnRender(DrawingContext drawingContext, IntRange visibleCandlesRange, CandleExtremums visibleCandlesExtremums, double candleWidth, double gapBetweenCandles, double RenderHeight)
        {
            base.OnRender(drawingContext, visibleCandlesRange, visibleCandlesExtremums, candleWidth, gapBetweenCandles, RenderHeight);

            for (int i = 1; i < visibleCandlesRange.Count; i++)
            {
                drawingContext.DrawLine(Pen, GetPoint(i - 1), GetPoint(i));
            }
        }

        protected override void OnLastCandleChanged()
        {
        }

        protected override void OnNewCandleAdded()
        {
        }

        protected override void ReCalcAllIndicatorValues()
        {
        }
    }
}
