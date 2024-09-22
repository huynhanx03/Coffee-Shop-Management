using Coffee.ViewModel.AdminVM.Statistic;
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

namespace Coffee.Views.Admin.StatisticPage
{
    /// <summary>
    /// Interaction logic for ImportHistoryPage.xaml
    /// </summary>
    public partial class ImportHistoryPage : Page
    {
        public ImportHistoryPage()
        {
            InitializeComponent();
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            //(DataContext as StatisticViewModel).openWindowEditBill();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as StatisticViewModel).deleteBillImport();
        }
    }
}
