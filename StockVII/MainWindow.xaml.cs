using StockVII.Data;
using System.IO;
using System.Windows;

namespace StockVII
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Directory.CreateDirectory("./Stock");
            Directory.CreateDirectory("./Indicators");

            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
