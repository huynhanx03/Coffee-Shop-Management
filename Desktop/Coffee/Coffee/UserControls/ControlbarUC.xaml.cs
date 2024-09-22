using Coffee.ViewModel.UserControlVM;
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

namespace Coffee.UserControls
{
    /// <summary>
    /// Interaction logic for ControlbarUC.xaml
    /// </summary>
    public partial class ControlbarUC : UserControl
    {
        public ControlbarViewModel Viewmodel{ get; set; }

        public ControlbarUC()
        {
            InitializeComponent();

            this.DataContext = Viewmodel = new ControlbarViewModel();
        }
    }
}
