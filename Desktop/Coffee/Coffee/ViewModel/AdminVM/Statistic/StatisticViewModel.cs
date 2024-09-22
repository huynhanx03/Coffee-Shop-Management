using ChartKit.Defaults;
using ChartKit.SkiaSharpView;
using ChartKit;
using Coffee.Views.Admin.StatisticPage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using Coffee.DTOs;
using Coffee.Views.Admin.EmployeePage;
using System.Windows;
using MaterialDesignThemes.Wpf;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Net;
using ChartKit.SkiaSharpView.WPF;
using Coffee.Services;
using Coffee.Models;
using Coffee.Views.MessageBox;
using OfficeOpenXml;
using System.IO;

namespace Coffee.ViewModel.AdminVM.Statistic
{
    public partial class StatisticViewModel : BaseViewModel
    {
        #region variable
        public Grid MaskName { get; set; }
        private ObservableCollection<ProductDTO> _ListTopMenu;
        public ObservableCollection<ProductDTO> ListTopMenu
        {
            get { return _ListTopMenu; }
            set { _ListTopMenu = value; OnPropertyChanged(); }
        }

        private DateTime _FromDate;
        public DateTime FromDate
        {
            get { return _FromDate; }
            set
            {
                if (value > ToDate)
                    _FromDate = ToDate;
                else
                    _FromDate = value;

                OnPropertyChanged();
            }
        }

        private DateTime _ToDate;
        public DateTime ToDate
        {
            get { return _ToDate; }
            set
            {
                _ToDate = value;
                OnPropertyChanged();
            }
        }
        private bool _IsLoading;

        public bool IsLoading
        {
            get { return _IsLoading; }
            set { _IsLoading = value; }
        }
        private int typeExport { get; set; }
        #endregion

        #region Icommand
        public ICommand loadShadowMaskIC { get; set; }
        public ICommand loadSaleHistoryIC { get; set; }
        public ICommand loadImportHistoryIC { get; set; }
        public ICommand loadStatisticIC { get; set; }
        public ICommand loadTopMenuIC { get; set; }
        public ICommand loadDataIC { get; set; }
        public ICommand exportExcelIC { get; set; }
        #endregion

        public StatisticViewModel()
        {
            ToDate = DateTime.Now;
            FromDate = ToDate;
            
            #region 
            
            loadShadowMaskIC = new RelayCommand<Grid>((p) => { return true; }, (p) =>
            {
                MaskName = p;
            });
            loadSaleHistoryIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                p.Content = new SaleHistoryPage();
                typeExport = 1;
            });
            loadImportHistoryIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                p.Content = new ImportHistoryPage();
                typeExport = 2;
            });

            loadStatisticIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                p.Content = new StatisticPage();
                typeExport = 3;
            });

            exportExcelIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                exportExcel();
            });

            loadBillListTimeIC = new RelayCommand<object>(p => true, p => LoadBillList(FromDate,ToDate));
            loadBillImportListtimeIC = new RelayCommand<object>(p => true, p => loadBillImportList(FromDate, ToDate));
            loadDataIC = new RelayCommand<object>(p => true, p => loadData());

            loadTopMenuIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                LoadTopMenu();
            });
            openWindowBillIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                openWindowBill();
            });
            openWindowBillImportIC = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                openWindowBillImport();
            });
            closeBillViewWindowIC = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
            });
            closeBillImportViewWindowIC = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
            });
            #endregion
        }

        #region operation
        //mở cửa sổ chi tiết hóa đơn
        public async void openWindowBill()
        {
            BillView b = new BillView();
            await loadBill(SelectedBill);
            b.ShowDialog();
        }

        //load dữ liệu chi tiết hóa đơn
        private async Task loadBill(BillDTO bill)
        {
            MaskName.Visibility = Visibility.Visible;
            NgayTao = bill.NgayTao;
            TongTien = bill.TongTien;
            TenNhanVien = bill.TenNhanVien;
            DetailBillList = new ObservableCollection<DetailBillDTO>(bill.DanhSachChiTietHoaDon);
            MaskName.Visibility = Visibility.Collapsed;
        } 

        //mở cửa sổ chi tiết hóa đơn nhập kho
        public async void openWindowBillImport()
        {
            BillimportView b = new BillimportView();
            await loadBillImport(SelectedBillImport);
            b.ShowDialog();
        }

        //load dữ liệu chi tiết hóa đơn nhập kho
        private async Task loadBillImport(ImportDTO import)
        {
            MaskName.Visibility = Visibility.Visible;
            NgayTaoPhieu = import.NgayTaoPhieu;
            TongTienNhap = Convert.ToInt32(import.TongTien);
            TenNhanVienNhapKho = import.TenNhanVien;
            DetailImportBillList = new ObservableCollection<DetailImportDTO>(import.DanhSachChiTietPhieuNhapKho);
            MaskName.Visibility = Visibility.Collapsed;
        }
        //load dữ liệu các sản phẩm bán chạy
        public async void LoadTopMenu()
        {
            (string label, List<ProductDTO> productlist) = await BillService.Ins.GetMostSoldFoods();

            if (productlist != null)
            {
                ListTopMenu = new ObservableCollection<ProductDTO>(productlist);
            }
        }
        #endregion

        public async void loadData()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            await LoadBillList(FromDate, ToDate);
            await loadBillImportList(FromDate, ToDate);
            await loadCartesianChar();

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        /// <summary>
        /// In dữ liệu ra excel
        /// </summary>
        private void exportExcel()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            System.Windows.Forms.SaveFileDialog sf = new System.Windows.Forms.SaveFileDialog
            {
                FileName = "LichSu",
                Filter = "Excel |*.xlsx",
                ValidateNames = true
            };

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Tạo một đối tượng ExcelPackage
            ExcelPackage package = new ExcelPackage();

            // Tạo một đối tượng ExcelWorksheet
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("one");

            switch (typeExport)
            {
                case 1:
                    if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        // Tiêu đề cột
                        worksheet.Cells[1, 1].Value = "Mã hoá đơn";
                        worksheet.Cells[1, 2].Value = "Mã nhân viên";
                        worksheet.Cells[1, 3].Value = "Tên nhân viên";
                        worksheet.Cells[1, 4].Value = "Mã khách hàng";
                        worksheet.Cells[1, 5].Value = "Tên khách hàng";
                        worksheet.Cells[1, 6].Value = "Tên bàn";
                        worksheet.Cells[1, 7].Value = "Ngày tạo";
                        worksheet.Cells[1, 8].Value = "Tổng tiền";
                        worksheet.Cells[1, 9].Value = "Trạng thái hoá đơn";

                        // Dữ liệu
                        int count = 2;
                        foreach (var item in BillList)
                        {
                            worksheet.Cells[count, 1].Value = item.MaHoaDon;
                            worksheet.Cells[count, 2].Value = item.MaNhanVien;
                            worksheet.Cells[count, 3].Value = item.TenNhanVien;
                            worksheet.Cells[count, 4].Value = item.MaKhachHang;
                            worksheet.Cells[count, 5].Value = item.TenKhachHang;
                            worksheet.Cells[count, 6].Value = item.TenBan;
                            worksheet.Cells[count, 7].Value = item.NgayTao;
                            worksheet.Cells[count, 8].Value = item.TongTien;
                            worksheet.Cells[count, 9].Value = item.TrangThai;

                            count++;
                        }

                        // Lưu file Excel
                        FileInfo fileInfo = new FileInfo(sf.FileName);
                        package.SaveAs(fileInfo);

                        MessageBoxCF mb = new MessageBoxCF("Xuất file thành công", MessageType.Accept, MessageButtons.OK);
                        mb.ShowDialog();
                    }
                    break;

                case 2:
                    if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        // Tiêu đề cột
                        worksheet.Cells[1, 1].Value = "Mã phiếu nhập kho";
                        worksheet.Cells[1, 2].Value = "Mã nhân viên";
                        worksheet.Cells[1, 3].Value = "Tên nhân viên";
                        worksheet.Cells[1, 4].Value = "Ngày tạo";
                        worksheet.Cells[1, 5].Value = "Tổng tiền";

                        // Dữ liệu
                        int count = 2;
                        foreach (var item in BillImportList)
                        {
                            worksheet.Cells[count, 1].Value = item.MaPhieuNhapKho;
                            worksheet.Cells[count, 2].Value = item.MaNhanVien;
                            worksheet.Cells[count, 3].Value = item.TenNhanVien;
                            worksheet.Cells[count, 4].Value = item.NgayTaoPhieu;
                            worksheet.Cells[count, 5].Value = item.TongTien;

                            count++;
                        }

                        // Lưu file Excel
                        FileInfo fileInfo = new FileInfo(sf.FileName);
                        package.SaveAs(fileInfo);

                        MessageBoxCF mb = new MessageBoxCF("Xuất file thành công", MessageType.Accept, MessageButtons.OK);
                        mb.ShowDialog();
                    }
                    break;

                case 3:
                    MessageBoxCF ms = new MessageBoxCF("Không có gì để xuất hết", MessageType.Error, MessageButtons.OK);
                    ms.ShowDialog();
                    break;
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }
    }
}
