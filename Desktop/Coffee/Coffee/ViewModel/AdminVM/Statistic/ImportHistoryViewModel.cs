using Coffee.DTOs;
using Coffee.Services;
using Coffee.Views.MessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Statistic
{
    public partial class StatisticViewModel : BaseViewModel
    {
        #region variable
        private ObservableCollection<ImportDTO> _BillImportList = new ObservableCollection<ImportDTO>();
        public ObservableCollection<ImportDTO> BillImportList
        {
            get { return _BillImportList; }
            set { _BillImportList = value; OnPropertyChanged(); }
        }
        private ImportDTO _SelectedBillImport;
        public ImportDTO SelectedBillImport
        {
            get { return _SelectedBillImport; }
            set { _SelectedBillImport = value; OnPropertyChanged(); }
        }
        #endregion

        #region Icommand
        public ICommand loadBillImportListtimeIC { get; set; }
        public ICommand openWindowBillImportIC { get; set; }
        #endregion
        /// <summary>
        /// load danh sách hóa đơn nhập kho
        /// </summary>
        private async Task loadBillImportList(DateTime fromDate = default(DateTime), DateTime toDate = default(DateTime))
        {
            (string label, List<ImportDTO> billimportlist) = await BillImportService.Ins.getListBillImporttime(fromDate, toDate);

            if (billimportlist != null)
            {
                BillImportList = new ObservableCollection<ImportDTO>(billimportlist);
            }
            else
            {
                BillImportList = new ObservableCollection<ImportDTO>();
            }
        }
        /// <summary>
        /// xóa hóa đơn nhập kho
        /// </summary>
        public async void deleteBillImport()
        {
            MessageBoxCF ms = new MessageBoxCF(Application.Current.Resources["ConfirmDeleteWarehouseBill"] as string, MessageType.Waitting, MessageButtons.YesNo);

            if (ms.ShowDialog() == true)
            {
                (string label, bool isDeleteBillImportList) = await BillImportService.Ins.DeleteBillImport(SelectedBillImport.MaPhieuNhapKho);

                if (isDeleteBillImportList)
                {
                    MessageBoxCF msn = new MessageBoxCF(label, MessageType.Accept, MessageButtons.OK);
                    loadData();
                    msn.ShowDialog();
                }
                else
                {
                    MessageBoxCF msn = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                    msn.ShowDialog();
                }
            }
        }
    }
}
