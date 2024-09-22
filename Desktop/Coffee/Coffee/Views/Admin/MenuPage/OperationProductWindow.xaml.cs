using Coffee.ViewModel.AdminVM.Menu;
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
using System.Windows.Shapes;

namespace Coffee.Views.Admin.MenuPage
{
    /// <summary>
    /// Interaction logic for OperationProductWindow.xaml
    /// </summary>
    public partial class OperationProductWindow : Window
    {
        public OperationProductWindow()
        {
            InitializeComponent();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MenuViewModel).addIngredientToRecipe();
        }
        
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MenuViewModel).removeRecipe();
        }
    }
}
