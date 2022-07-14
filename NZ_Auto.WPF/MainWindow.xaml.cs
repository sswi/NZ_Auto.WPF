using NZ_Auto.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NZ_Auto.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel=new MainWindowViewModel();
            //数据绑定
            DataContext = _viewModel;

        }


        //退出时清除插件，避免下次启动异常
        protected override void OnClosed(EventArgs e)
        {
            _viewModel._dm.Dispose();
            base.OnClosed(e);
        }


    }
}
