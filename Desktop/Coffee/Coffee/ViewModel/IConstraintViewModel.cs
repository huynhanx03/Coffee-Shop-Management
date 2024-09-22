using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Coffee.ViewModel
{
    public interface IConstraintViewModel
    {
        /// <summary>
        /// Chỉ nhận số
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void NumberValidationTextBox(object sender, TextCompositionEventArgs e);
    }
}
