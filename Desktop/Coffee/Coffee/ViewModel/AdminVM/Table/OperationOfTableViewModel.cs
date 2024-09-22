using Coffee.DTOs;
using Coffee.Services;
using Coffee.Utils;
using Coffee.Views.MessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace Coffee.ViewModel.AdminVM.Table
{
    public partial class MainTableViewModel: BaseViewModel, IConstraintViewModel
    {
        #region variable
        private int TypeOperation { get; set; }

        private string _TableName;

        public string TableName
        {
            get { return _TableName; }
            set { _TableName = value; OnPropertyChanged(); }
        }

        private int _Row;

        public int Row
        {
            get { return _Row; }
            set { _Row = value; OnPropertyChanged(); }
        }

        private int _Coloumn;

        public int Coloumn
        {
            get { return _Coloumn; }
            set { _Coloumn = value; OnPropertyChanged(); }
        }

        private ObservableCollection<TableTypeDTO> _TableTypeList;
        public ObservableCollection<TableTypeDTO> TableTypeList
        {
            get { return _TableTypeList; }
            set { _TableTypeList = value; OnPropertyChanged(); }
        }

        private string _SelectedTableTypeName;

        public string SelectedTableTypeName
        {
            get { return _SelectedTableTypeName; }
            set { _SelectedTableTypeName = value; OnPropertyChanged(); }
        }

        #endregion

        #region ICommand
        public ICommand closeOperationTableWindowIC { get; set; }
        public ICommand confirmOperationTableIC { get; set; }
        #endregion


        /// <summary>
        /// Load danh sách loại bàn
        /// </summary>
        private async void loadTableType()
        {
            (string label, List<TableTypeDTO> listTableType) = await TableTypeService.Ins.getAllTableType();

            if (listTableType != null)
                TableTypeList = new ObservableCollection<TableTypeDTO>(listTableType);
        }

        /// <summary>
        /// Xác nhận thao tác của bàn
        /// </summary>
        /// <param name="w">Cửa sổ thao tác</param>
        private async void confirmOperationTable(Window w)
        {
            TableTypeDTO tableType = TableTypeList.First(p => p.TenLoaiBan == SelectedTableTypeName);

            TableDTO table = new TableDTO
            {
                TenBan = TableName.Trim(),
                MaLoaiBan = tableType.MaLoaiBan,
                Cot = Coloumn,
                Hang = Row,
                TrangThai = Constants.StatusTable.FREE
            };

            switch (TypeOperation)
            {
                case 1:
                    // Add
                    (string lableCreate, TableDTO tableCreate) = await TableService.Ins.createTable(table); 
                    
                    // Thành công
                    if (tableCreate != null)
                    {
                        MessageBoxCF msCreate = new MessageBoxCF(lableCreate, MessageType.Accept, MessageButtons.OK);
                        msCreate.ShowDialog();
                        loadTableList();
                        w.Close();
                        resetTable();
                    }
                    else // Thất bại
                    {
                        MessageBoxCF msCreate = new MessageBoxCF(lableCreate, MessageType.Error, MessageButtons.OK);
                        msCreate.ShowDialog();
                    }
                   
                    break;

                case 2:
                    table.MaBan = tableID;

                    // Edit
                    (string lableEdit, TableDTO tableEdit) = await TableService.Ins.updateTable(table);

                    // Thành công
                    if (tableEdit != null)
                    {
                        MessageBoxCF msCreate = new MessageBoxCF(lableEdit, MessageType.Accept, MessageButtons.OK);
                        msCreate.ShowDialog();
                        loadTableList();
                        w.Close();
                    }
                    else // Thất bại
                    {
                        MessageBoxCF msCreate = new MessageBoxCF(lableEdit, MessageType.Error, MessageButtons.OK);
                        msCreate.ShowDialog();
                    }
                    break;
            }
        }

        /// <summary>
        /// Reset dữ liệu của bàn
        /// </summary>
        private void resetTable()
        {
            TableName = "";
            Row = 0;
            Coloumn = 0;
        }
    } 
}
