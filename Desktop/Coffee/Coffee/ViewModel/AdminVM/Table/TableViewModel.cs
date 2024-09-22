using Coffee.DTOs;
using Coffee.Models;
using Coffee.Services;
using Coffee.Utils;
using Coffee.Views.Admin.TablePage;
using Coffee.Views.MessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Table
{
    public partial class MainTableViewModel: BaseViewModel
    {
        #region variable
        private ObservableCollection<TableDTO> _TableList;
        public ObservableCollection<TableDTO> TableList
        {
            get { return _TableList; }
            set { _TableList = value; OnPropertyChanged(); }
        }

        private TableDTO _SelectedTable;
        public TableDTO SelectedTable
        {
            get { return _SelectedTable; }
            set { _SelectedTable = value; OnPropertyChanged(); }
        }

        private TableDTO _TableMoveSelected;
        public TableDTO TableMoveSelected
        {
            get { return _TableMoveSelected; }
            set { _TableMoveSelected = value; OnPropertyChanged(); }
        }

        private TableDTO _TableTransferSelected;
        public TableDTO TableTransferSelected
        {
            get { return _TableTransferSelected; }
            set { _TableTransferSelected = value; OnPropertyChanged(); }
        }

        private TableDTO _TableNumber1Selected;
        public TableDTO TableNumber1Selected
        {
            get { return _TableNumber1Selected; }
            set { _TableNumber1Selected = value; OnPropertyChanged(); }
        }

        private TableDTO _TableNumber2Selected;
        public TableDTO TableNumber2Selected
        {
            get { return _TableNumber2Selected; }
            set { _TableNumber2Selected = value; OnPropertyChanged(); }
        }

        private string tableID { get; set; }

        #endregion

        #region ICommend
        public ICommand loadTableListIC { get; set; }
        public ICommand openEditTableIC { get; set; }
        public ICommand openDeleteTableIC { get; set; }
        public ICommand clickTableIC { get; set; }
        public ICommand openWindowChangeTableIC { get; set; }
        public ICommand confirmChangeTableIC { get; set; }
        public ICommand closeChangeTableWindowIC { get; set; }
        public ICommand openWindowMergeTableIC { get; set; }
        public ICommand confirmMergeTableIC { get; set; }
        public ICommand closeMergeTableWindowIC { get; set; }

        #endregion

        /// <summary>
        /// load danh sách bàn
        /// </summary>
        private async void loadTableList()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            (string label, List<TableDTO> tableList) = await TableService.Ins.getListTable();

            if (tableList != null)
            {
                TableList = new ObservableCollection<TableDTO>(tableList);
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        /// <summary>
        /// Xoá bàn
        /// </summary>
        private async void deleteTable(TableDTO table)
        {
            MaskName.Visibility = Visibility.Visible;

            if (table.TrangThai == Constants.StatusTable.BOOKED)
            {
                MessageBoxCF msTable = new MessageBoxCF("Bàn này đang có khách không thể xoá", MessageType.Error, MessageButtons.OK);
                msTable.ShowDialog();
                MaskName.Visibility = Visibility.Collapsed;
                return;
            }

            MessageBoxCF ms = new MessageBoxCF("Xác nhận xoá bàn?", MessageType.Waitting, MessageButtons.YesNo);

            if (ms.ShowDialog() == true)
            {
                IsLoading = true;
                (string label, bool isDeleteTable) = await TableService.Ins.DeleteTable(table.MaBan);

                if (isDeleteTable)
                {
                    MessageBoxCF msn = new MessageBoxCF(label, MessageType.Accept, MessageButtons.OK);
                    loadTableList();

                    IsLoading = false;

                    msn.ShowDialog();
                }
                else
                {
                    MessageBoxCF msn = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                    msn.ShowDialog();
                }
            }

            MaskName.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// click bàn
        /// </summary>
        private async void clickTable()
        {
            // Load thông tin bàn
            btnMenu.IsChecked = true;
            TableNameSale = SelectedTable.TenBan;
            currentTable = SelectedTable;

            // Nếu bàn đó chưa thanh toán thì load thực đơn để thanh toán
            if (currentTable.TrangThai == Constants.StatusTable.BOOKED)
            {
                MaskName.Visibility = Visibility.Visible;
                IsLoading = true;

                (string label, BillDTO bill) = await BillService.Ins.findBillByTableBooking(currentTable.MaBan);

                if (bill != null)
                {
                    DetailBillList = new ObservableCollection<DetailBillDTO>(bill.ChiTietHoaDon.Values);

                    TotalBill = bill.TongTien;
                }

                billCurrent = new BillModel(bill);

                MaskName.Visibility = Visibility.Collapsed;
                IsLoading = false;
            }
            else
            {
                billCurrent = null;
                DetailBillList.Clear();
            }
        }

        /// <summary>
        /// Mở cửa sổ chuyển bàn
        /// </summary>
        private void openWindowChangeTable()
        {
            ChangeTableWindow w = new ChangeTableWindow();

            MaskName.Visibility = Visibility.Visible;

            w.ShowDialog();

            MaskName.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Xác nhận chuyển bàn
        /// </summary>
        private async void confirmChangeTable(Window w)
        {
            MaskName.Visibility = Visibility.Visible;

            if (TableMoveSelected == TableTransferSelected)
            {
                // Nếu 2 bàn giống nhau thì không chuyển được
                MessageBoxCF ms = new MessageBoxCF("Không thể chuyển từ bàn này sang bàn chính nó", MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
                MaskName.Visibility = Visibility.Collapsed;

                return;
            }

            if (TableMoveSelected.TrangThai == Constants.StatusTable.FREE)
            {
                // Nếu bàn muốn chuyển đang trạng thái "trống" thì không chuyển được
                MessageBoxCF ms = new MessageBoxCF("Bàn này không có hoá đơn để chuyển", MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
                MaskName.Visibility = Visibility.Collapsed;

                return;
            }

            if (TableTransferSelected.TrangThai == Constants.StatusTable.BOOKED)
            {
                // Nếu bàn muốn chuyển đến đang trạng thái "đã đặt" thì không chuyển được
                MessageBoxCF ms = new MessageBoxCF("Bàn này muốn chuyển đến đã có người. Xin hãy gộp bàn nếu muốn chuyển đến", MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
                MaskName.Visibility = Visibility.Collapsed;

                return;
            }

            IsLoading = true;

            // Chuyển MaBan tại hoá đơn
            (string labelUpdateBill, bool isUpdate) = await BillService.Ins.updateBillByTableID(TableMoveSelected.MaBan, TableTransferSelected.MaBan);

            if (isUpdate)
            {
                // Chuyển đổi trạng thái bàn
                //TableMoveSelected.TrangThai = Constants.StatusTable.FREE;
                //TableTransferSelected.TrangThai = Constants.StatusTable.BOOKED;

                //(string labelUpdateTableMove, TableDTO tableMove) = await TableService.Ins.updateTable(TableMoveSelected);
                //(string labelUpdateTableTransfer, TableDTO tableTransfer) = await TableService.Ins.updateTable(TableTransferSelected);

                loadTableList();

                MessageBoxCF ms = new MessageBoxCF("Chuyển bàn thành công", MessageType.Accept, MessageButtons.OK);
                ms.ShowDialog();

                w.Close();
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        /// <summary>
        /// Mở cửa sổ gộp bàn
        /// </summary>
        private void openWindowMergeTable()
        {
            MergeTableWindow w = new MergeTableWindow();

            MaskName.Visibility = Visibility.Visible;

            w.ShowDialog();

            MaskName.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Xác nhận gộp bàn
        /// </summary>
        private async void confirmMergeTable(Window w)
        {
            MaskName.Visibility = Visibility.Visible;

            if (TableNumber1Selected == TableNumber2Selected)
            {
                // Nếu 2 bàn giống nhau thì không chuyển được
                MessageBoxCF ms = new MessageBoxCF("Không thể gộp từ bàn này sang bàn chính nó", MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
                MaskName.Visibility = Visibility.Collapsed;

                return;
            }

            if (TableNumber1Selected.TrangThai == Constants.StatusTable.FREE)
            {
                // Không thể gộp với bàn "trống"
                MessageBoxCF ms = new MessageBoxCF("Bàn này chưa được đặt để gộp", MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
                MaskName.Visibility = Visibility.Collapsed;

                return;
            }

            if (TableNumber2Selected.TrangThai == Constants.StatusTable.FREE)
            {
                // Không thể gộp với bàn "trống"
                MessageBoxCF ms = new MessageBoxCF("Bàn này chưa được đặt để gộp", MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
                MaskName.Visibility = Visibility.Collapsed;

                return;
            }

            IsLoading = true;

            // Gộp hoá đơn
            (string labelUpdateBill, bool isUpdate) = await BillService.Ins.mergeBillByTableID(TableNumber1Selected.MaBan, TableNumber2Selected.MaBan);

            if (isUpdate)
            {
                // Chuyển đổi trạng thái bàn
                TableNumber1Selected.TrangThai = Constants.StatusTable.FREE;

                (string labelUpdate, TableDTO table) = await TableService.Ins.updateTable(TableNumber1Selected);

                loadTableList();

                MessageBoxCF ms = new MessageBoxCF("Gộp bàn thành công", MessageType.Accept, MessageButtons.OK);
                ms.ShowDialog();

                w.Close();
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }
    }
}
