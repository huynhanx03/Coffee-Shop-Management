using Coffee.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Table
{
    public partial class MainTableViewModel : BaseViewModel
    {
        #region variable
        private ObservableCollection<ProductRecommendDTO> _ProductRecommendList;
        public ObservableCollection<ProductRecommendDTO> ProductRecommendList
        {
            get { return _ProductRecommendList; }
            set { _ProductRecommendList = value; OnPropertyChanged(); }
        }
        #endregion

        #region ICommend
        public ICommand closeRecommendProductWindowIC { get; set; }

        #endregion
    }
}
