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
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Coffee.Utils.Constants;

namespace Coffee.ViewModel.AdminVM.Table
{
    public partial class MainTableViewModel: BaseViewModel, IConstraintViewModel
    {
        #region variable
        private ObservableCollection<DetailBillDTO> _DetailBillList = new ObservableCollection<DetailBillDTO>();
        public ObservableCollection<DetailBillDTO> DetailBillList
        {
            get { return _DetailBillList; }
            set { _DetailBillList = value; OnPropertyChanged(); }
        }

        private DetailBillDTO _SelectedDetailBill;
        public DetailBillDTO SelectedDetailBill
        {
            get { return _SelectedDetailBill; }
            set { _SelectedDetailBill = value; OnPropertyChanged(); }
        }

        private decimal _TotalBill;

        public decimal TotalBill
        {
            get { return _TotalBill; }
            set { _TotalBill = value; OnPropertyChanged(); }
        }

        private string _TableNameSale;

        public string TableNameSale
        {
            get { return _TableNameSale; }
            set { _TableNameSale = value; OnPropertyChanged(); }
        }

        private string _Created;

        public string Created
        {
            get { return _Created; }
            set { _Created = value; OnPropertyChanged(); }
        }

        private string _EmployeeName;

        public string EmployeeName
        {
            get { return _EmployeeName; }
            set { _EmployeeName = value; OnPropertyChanged(); }
        }
        
        private string _CustomeName = "Khách vãng lai";

        public string CustomeName
        {
            get { return _CustomeName; }
            set { _CustomeName = value; OnPropertyChanged(); }
        }
        
        private string _CustomerPhone;

        public string CustomerPhone
        {
            get { return _CustomerPhone; }
            set { _CustomerPhone = value; OnPropertyChanged(); }
        }
        
        private UserDTO _Customer;

        public UserDTO Customer
        {
            get { return _Customer; }
            set { _Customer = value; OnPropertyChanged(); }
        }

        private TableDTO currentTable { get; set; }
        private BillModel billCurrent { get; set; }

        #endregion

        #region ICommend
        public ICommand changeSizeProductIC { get; set; }
        public ICommand subQuantityBillIC { get; set; }
        public ICommand plusQuantityBillIC { get; set; }
        public ICommand removeBillIC { get; set; }
        public ICommand loadDateSalesIC { get; set; }
        public ICommand bookingIC { get; set; }
        public ICommand payIC { get; set; }
        public ICommand loadCustomerIC { get; set; }

        #endregion


        /// <summary>
        /// Thay đổi kích thước
        /// </summary>
        /// <param name="detalBill"> chi tiết hoá đơn </param>
        private void changeSizeProduct(DetailBillDTO detalBill)
        {
            if (SelectedDetailBill == null)
                return;

            try
            {
                SelectedDetailBill.ThanhTien = SelectedDetailBill.SoLuong * SelectedDetailBill.SelectedProductSize.Gia;

                DetailBillDTO find = DetailBillList.FirstOrDefault(x => x != SelectedDetailBill && x.MaSanPham == SelectedDetailBill.MaSanPham && x.SelectedProductSize == SelectedDetailBill.SelectedProductSize);

                if (find != null)
                {
                    SelectedDetailBill.SoLuong += find.SoLuong;

                    SelectedDetailBill.ThanhTien = SelectedDetailBill.SoLuong * SelectedDetailBill.SelectedProductSize.Gia;
                    DetailBillList.Remove(find);
                }

                DetailBillList = new ObservableCollection<DetailBillDTO>(DetailBillList);
                CalculateTotalBill();
            }
            catch (Exception ex) 
            { 
         
            }
        }

        /// <summary>
        /// Giảm số lượng của sản phẩm tại hoá đơn
        /// </summary>
        private void subQuantityBill()
        {
            if (SelectedDetailBill.SoLuong > 1)
            {
                SelectedDetailBill.SoLuong -= 1;
                SelectedDetailBill.ThanhTien = SelectedDetailBill.SelectedProductSize.Gia * SelectedDetailBill.SoLuong;

                DetailBillList = new ObservableCollection<DetailBillDTO>(DetailBillList);
                CalculateTotalBill();
            }
            else
            {
                // Xoá
                removeBill();
            }
        }

        /// <summary>
        /// Tăng số lượng của sản phẩm tại hoá đơn
        /// </summary>
        private void plusQuantityBill()
        {
            ProductDTO product = ProductList.First(x => x.MaSanPham == SelectedDetailBill.MaSanPham);
            List<DetailBillDTO> listFind = DetailBillList.Where(x => x.MaSanPham == SelectedDetailBill.MaSanPham).ToList();

            int totalQuantity = listFind.Sum(x => x.SoLuong);
            
            // Kiếm tra số lượng
            if (totalQuantity + 1 <= product.SoLuong)
            {

                SelectedDetailBill.SoLuong += 1;
                SelectedDetailBill.ThanhTien = SelectedDetailBill.SelectedProductSize.Gia * SelectedDetailBill.SoLuong;
            }
            else
            {
                // 
            }

            DetailBillList = new ObservableCollection<DetailBillDTO>(DetailBillList);
            CalculateTotalBill();
        }

        /// <summary>
        /// Xoá sản phẩm ra khỏi hoá đơn
        /// </summary>
        private void removeBill()
        {
            MaskName.Visibility = Visibility.Visible;
            MessageBoxCF ms = new MessageBoxCF("Xác nhận gỡ sản phẩm này khỏi hoá đơn", MessageType.Waitting, MessageButtons.YesNo);
            if (ms.ShowDialog() == true)
            {
                DetailBillList.Remove(SelectedDetailBill);
                DetailBillList = new ObservableCollection<DetailBillDTO>(DetailBillList);
                CalculateTotalBill();
            }
            MaskName.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Tính tổng tiền của hoá đơn
        /// </summary>
        private void CalculateTotalBill()
        {
            decimal total = 0;

            foreach (var item in DetailBillList)
                total += item.ThanhTien;
        
            TotalBill = total;
        }

        /// <summary>
        /// load dữ liệu của bán hàng
        /// </summary>
        private void loadDateSales()
        {
            Created = DateTime.Now.ToString("dd/MM/yyyy");
            EmployeeName = Memory.user.HoTen;
        }

        /// <summary>
        /// Đặt món: đặt bàn tại thu ngân nhưng chưa tính tiền
        /// </summary>
        private async void booking()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            // Lưu thế thông tin chi tiết hoá đơn lên trên cơ sở dữ liệu
            BillModel bill = new BillModel
            {
                MaBan = currentTable.MaBan,
                MaNhanVien = Memory.user.MaNguoiDung,
                NgayTao = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"),
                //TongTien = TotalBill,
                TrangThai = StatusBill.UNPAID,
                MaKhachHang = "",
                ChiTietHoaDon = DetailBillList.ToDictionary(
                    detail => detail.MaSanPham + "-" + detail.SelectedProductSize.MaKichThuoc,
                    detail => new DetailBillModel
                    {
                        MaSanPham = detail.MaSanPham,
                        MaKichThuoc = detail.SelectedProductSize.MaKichThuoc,
                        SoLuong = detail.SoLuong,
                        ThanhTien = detail.ThanhTien
                    })
            };

            if (Customer != null)
                bill.MaKhachHang = Customer.MaNguoiDung;

            (string label, bool isCreate) = await BillService.Ins.createBill(bill);
        
            if (isCreate)
            {
                // Cộng điểm cho khách hàng
                if (Customer != null)
                {
                    (string labelUpdatePoint, bool isUpdate) = await CustomerService.Ins.updatePointRankCustomer(Customer.MaNguoiDung, (double)bill.TongTien / 10000);

                    //await CustomerService.Ins.checkUpdateRankCustomer(Customer.MaNguoiDung, point);
                }

                // Thành công: Xoá số lượng sản phẩm
                reduceProduct();

                // Chuyển sang bàn có khách
                currentTable.TrangThai = Constants.StatusTable.BOOKED;

                (string labelTable, TableDTO table) = await TableService.Ins.updateTable(currentTable);
                loadTableList();
                TotalBill = 0;

                billCurrent = bill;

                MessageBoxCF ms = new MessageBoxCF("Đặt món thành công", MessageType.Accept, MessageButtons.OK);
                ms.ShowDialog();
            }
            else
            {
                MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        // Thanh toán hoá đơn
        private async void pay()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            // Nếu chưa có bill trước
            if (billCurrent == null)
            {
                //Thanh toán luôn
                //Lưu thế thông tin chi tiết hoá đơn lên trên cơ sở dữ liệu
                BillModel bill = new BillModel
                {
                    MaBan = currentTable == null ? "" : currentTable.MaBan,
                    MaNhanVien = Memory.user.MaNguoiDung,
                    NgayTao = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"),
                    //TongTien = TotalBill,
                    TrangThai = StatusBill.PAID,
                    MaKhachHang = "",
                    ChiTietHoaDon = DetailBillList.ToDictionary(
                    detail => detail.MaSanPham + "-" + detail.SelectedProductSize.MaKichThuoc,
                    detail => new DetailBillModel
                    {
                        MaSanPham = detail.MaSanPham,
                        MaKichThuoc = detail.SelectedProductSize.MaKichThuoc,
                        SoLuong = detail.SoLuong,
                        ThanhTien = detail.ThanhTien
                    })
                };

                if (Customer != null)
                    bill.MaKhachHang = Customer.MaNguoiDung;

                (string label, bool isCreate) = await BillService.Ins.createBill(bill);

                if (isCreate)
                {
                    // Cộng điểm cho khách hàng
                    if (Customer != null)
                    {
                        (string labelUpdatePoint, bool isUpdate) = await CustomerService.Ins.updatePointRankCustomer(Customer.MaNguoiDung, (double)bill.TongTien / 10000);

                        //await CustomerService.Ins.checkUpdateRankCustomer(Customer.MaNguoiDung, point);
                    }

                    // Giảm số lượng sản phẩm
                    reduceProduct();

                    // Thành công:
                    showBill();

                    TotalBill = 0;
                }
                else
                {
                    MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                    ms.ShowDialog();
                }

            }
            else // Thanh toán hoá đơn trước
            {
                BillModel billUpdate = new BillModel
                {
                    MaBan = currentTable == null ? "" : currentTable.MaBan,
                    MaNhanVien = Memory.user.MaNguoiDung,
                    NgayTao = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"),
                    //TongTien = TotalBill,
                    TrangThai = StatusBill.PAID,
                    MaKhachHang = "",
                    ChiTietHoaDon = DetailBillList.ToDictionary(
                    detail => detail.MaSanPham + "-" + detail.SelectedProductSize.MaKichThuoc,
                    detail => new DetailBillModel
                    {
                        MaSanPham = detail.MaSanPham,
                        MaKichThuoc = detail.SelectedProductSize.MaKichThuoc,
                        SoLuong = detail.SoLuong,
                        ThanhTien = detail.ThanhTien
                    })
                };

                (string label, BillModel bill) = await BillService.Ins.updateBill(billUpdate);

                if (bill != null)
                {
                    currentTable.TrangThai = Constants.StatusTable.FREE;

                    (string labelTable, TableDTO table) = await TableService.Ins.updateTable(currentTable);

                    loadTableList();

                    showBill();
                }
                else
                {
                    MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                    ms.ShowDialog();
                }
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        /// <summary>
        /// Giảm số lượng sản phẩm
        /// </summary>
        private async void reduceProduct()
        {
            var groupedData = DetailBillList.GroupBy(item => item.MaSanPham)
                                .Select(group => new
                                {
                                    MaSanPham = group.Key,
                                    SoLuong = group.Sum(item => item.SoLuong)
                                }).ToList();

            foreach (var group in groupedData)
            {
                await ProductService.Ins.reduceQuantityProduct(group.MaSanPham, group.SoLuong);
            }

            loadMenuList();
        }

        /// <summary>
        /// tìm kiếm khách hàng theo số điện thoại
        /// </summary>
        private async void loadCustomer()
        {
            if (CustomerPhone.Length == 10)
            {
                // Kiểm tra số điện thoại
                UserDTO user = await UserService.Ins.getUserByNumberphone(CustomerPhone);

                if (user != null)
                {
                    Customer = user;
                    CustomeName = user.HoTen;
                }
            }
            else
            {
                CustomeName = "Khách vãng lai";
                Customer = null;
            }
        }

        /// <summary>
         /// Kiểm tra chỉ được nhập số
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
        public void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void showBill()
        {
            DateBill = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");

            if (Customer != null)
                CurrentPhone = Customer.SoDienThoai;
            else
                CurrentPhone = "";

            BillWindow w = new BillWindow();
            w.ShowDialog();
        }
    }
}
