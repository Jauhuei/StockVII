using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace StockVII.Data
{
    /// <summary>
    /// 主窗口的 ViewModel
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private List<CompanyInfo> _companyInfos;

        /// <summary>
        /// 公司信息列表
        /// </summary>
        public List<CompanyInfo> CompanyInfos
        {
            get { return _companyInfos; }
            set
            {
                _companyInfos = value;
                RaisePropertyChanged(nameof(CompanyInfos));
            }
        }

        private CompanyInfo _selectCompanyInfo;

        /// <summary>
        /// 选中的公司信息
        /// </summary>
        public CompanyInfo SelectCompanyInfo
        {
            get { return _selectCompanyInfo; }
            set
            {
                _selectCompanyInfo?.Clear();
                _selectCompanyInfo = value;
                _ = Task.Run(async () =>
                {
                    await value.DownloadAndSaveAsync();
                    RaisePropertyChanged(nameof(SelectCompanyInfo));
                    RaisePropertyChanged(nameof(Message));
                });
                RaisePropertyChanged(nameof(SelectCompanyInfo));
                RaisePropertyChanged(nameof(Message));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 消息字符串，显示选中公司的更新时间
        /// </summary>
        public string Message
        {
            get
            {
                if (SelectCompanyInfo == null) return $"请选择...";
                return $"更新时间：{SelectCompanyInfo.LastUpdate:yyy/MM/dd HH:mm:ss}, {SelectCompanyInfo.StockInfos.Count} 笔数据";
            }
        }

        /// <summary>
        /// 构造函数，初始化数据并开始下载
        /// </summary>
        public MainViewModel()
        {
            _ = DownloadAsync();
        }

        /// <summary>
        /// 异步下载沪深 300 种类的公司信息并保存
        /// </summary>
        private async Task DownloadAsync()
        {
            var companyInfos = await Downloader.GetHS300CompanyInfosAsync();
            while (!companyInfos.Any())
            {
                companyInfos = await Downloader.GetHS300CompanyInfosAsync();
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                CompanyInfos = companyInfos;
                RaisePropertyChanged(nameof(CompanyInfos));
            });
        }
    }
}
