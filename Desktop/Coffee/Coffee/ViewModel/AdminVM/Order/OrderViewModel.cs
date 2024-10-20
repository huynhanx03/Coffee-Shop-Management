using CloudinaryDotNet.Actions;
using Coffee.DALs;
using Coffee.DesignPattern.Mediator;
using Coffee.DTOs;
using Coffee.Models;
using Coffee.Services;
using Coffee.Utils;
using Coffee.Views.Admin.OrderPage;
using Coffee.Views.MessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Coffee.Utils.Constants;

namespace Coffee.ViewModel.AdminVM.Order
{
    public partial class OrderViewModel: BaseViewModel
    {
        #region variable
        public Grid MaskName { get; set; }

        private ObservableCollection<OrderDTO> _OrderList;

        public ObservableCollection<OrderDTO> OrderList
        {
            get { return _OrderList; }
            set { _OrderList = value; OnPropertyChanged(); }
        }

        private List<OrderDTO> __OrderList;
        private List<OrderDTO> OrderSearchList;
        private List<OrderDTO> OrderStatusList;

        private ObservableCollection<ProductOrderDTO> _ProductOrderList;

        public ObservableCollection<ProductOrderDTO> ProductOrderList
        {
            get { return _ProductOrderList; }
            set { _ProductOrderList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _StatusList;

        public ObservableCollection<string> StatusList
        {
            get { return _StatusList; }
            set { _StatusList = value; OnPropertyChanged(); }
        }

        private string _selectedStatus;
        public string SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                _selectedStatus = value;
                OnPropertyChanged();
            }
        }

        private bool _IsLoading;

        public bool IsLoading
        {
            get { return _IsLoading; }
            set { _IsLoading = value; OnPropertyChanged(); }
        }

        #endregion

        #region ICommand
        public ICommand loadShadowMaskIC { get; set; }
        public ICommand loadOrderListIC { get; set; }
        public ICommand loadStatusListIC { get; set; }
        public ICommand searchOrderIC { get; set; }
        public ICommand cancelOrderIC { get; set; }
        public ICommand confirmOrderIC { get; set; }
        public ICommand viewDetailOrderIC { get; set; }
        public ICommand closeViewDetailOrderWindowIC { get; set; }
        public ICommand selectedStatusIC { get; set; }
        #endregion

        public OrderViewModel()
        {
            if (ConcreteMediator.Ins.orderViewModel == null) ConcreteMediator.Ins.orderViewModel = this;

            loadShadowMaskIC = new RelayCommand<Grid>((p) => { return true; }, (p) =>
            {
                MaskName = p;
            });

            loadOrderListIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                MaskName.Visibility = Visibility.Visible;
                IsLoading = true;

                loadOrderList();

                MaskName.Visibility = Visibility.Collapsed;
                IsLoading = false;
            });

            loadStatusListIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                loadStatusList();
            });

            cancelOrderIC = new RelayCommand<OrderDTO>((p) => { return true; }, (p) =>
            {
                cancelOrder(p);
            });

            confirmOrderIC = new RelayCommand<OrderDTO>((p) => { return true; }, (p) =>
            {
                confirmOrder(p);
            });

            viewDetailOrderIC = new RelayCommand<OrderDTO>((p) => { return true; }, (p) =>
            {
                viewDetailOrder(p);
            });

            selectedStatusIC = new RelayCommand<TextBox>(null, (p) =>
            {
                selectedStatus(SelectedStatus);
            });

            closeViewDetailOrderWindowIC = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                p.Close();
            });

            searchOrderIC = new RelayCommand<TextBox>(null, (p) =>
            {
                searchOrder(p.Text);
            });
        }

        #region function
        /// <summary>
        /// Load danh sách đơn hàng
        /// </summary>
        public async Task loadOrderList()
        {
            (string label, List<OrderDTO> Orders) = await OrderService.Ins.getListOrder();

            if (Orders != null)
            {
                Orders = Orders.OrderBy(s => DateTime.ParseExact(s.NgayTaoDon, "HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture)).ToList();

                OrderList = new ObservableCollection<OrderDTO>(Orders);
                __OrderList = new List<OrderDTO>(Orders);
                OrderSearchList = new List<OrderDTO>(Orders);
                OrderStatusList = new List<OrderDTO>(Orders);
            }
            else
            {
                OrderList = new ObservableCollection<OrderDTO>();
                __OrderList = new List<OrderDTO>();
                OrderSearchList = new List<OrderDTO>();
                OrderStatusList = new List<OrderDTO>();
            }

            await ConcreteMediator.Ins.Notify(this, "UpdateOrderCount " + __OrderList.Count(order => order.TrangThai == Constants.StatusOrder.WAITTING).ToString());
        }

        /// <summary>
        /// Huỷ đơn hàng
        /// </summary>
        public async void cancelOrder(OrderDTO Order)
        {
            MaskName.Visibility = Visibility.Visible;
            MessageBoxCF ms = new MessageBoxCF("Xác nhận huỷ đơn hàng này?", MessageType.Waitting, MessageButtons.YesNo);

            if (ms.ShowDialog() == true)
            {
                IsLoading = true;
                (string label, bool isCancelOrder) = await OrderService.Ins.updateStatusOrder(Order.MaDonHang, Constants.StatusOrder.CANCEL);
                IsLoading = false;

                if (isCancelOrder)
                {
                    MessageBoxCF msn = new MessageBoxCF("Huỷ đơn hàng thành công", MessageType.Accept, MessageButtons.OK);
                    loadOrderList();
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
        /// Xác nhận đơn hàng
        /// </summary>
        public async void confirmOrder(OrderDTO Order)
        {
            MaskName.Visibility = Visibility.Visible;
            MessageBoxCF ms = new MessageBoxCF("Xác nhận giao đơn hàng này?", MessageType.Waitting, MessageButtons.YesNo);

            if (ms.ShowDialog() == true)
            {
                IsLoading = true;

                BillModel bill = new BillModel
                {
                    MaBan = "",
                    MaNhanVien = Memory.user.MaNguoiDung,
                    NgayTao = Order.NgayTaoDon,
                    TrangThai = StatusBill.PAID,
                    MaKhachHang = Order.MaNguoiDung,
                    ChiTietHoaDon = Order.DanhSachSanPham.ToDictionary(
                    detail => detail.MaSanPham + "-" + detail.MaKichThuoc,
                    detail => new DetailBillModel
                    {
                        MaSanPham = detail.MaSanPham,
                        MaKichThuoc = detail.MaKichThuoc,
                        SoLuong = detail.SoLuong,
                        ThanhTien = detail.Gia * detail.SoLuong
                    })
                };

                (string labelCreateBill, bool isCreate) = await BillService.Ins.createBill(bill);

                if (isCreate)
                {
                    // Thêm mã hoá đơn vào trong đơn hàng
                    (string labelUpdateBillIDOrder, bool isUpdateBillIDOrder) = await OrderService.Ins.updateBillIDOrder(Order.MaDonHang, bill.MaHoaDon);

                    //// Tăng điểm
                    (string labelUpdatePoint, bool isUpdate) = await CustomerService.Ins.updatePointRankCustomer(Order.MaNguoiDung, (double)Order.ThanhTien / 10000);

                    reduceProduct(Order.DanhSachSanPham);

                    (string label, bool isConfirmOrder) = await OrderService.Ins.updateStatusOrder(Order.MaDonHang, Constants.StatusOrder.CONFIRMED);

                    if (isConfirmOrder)
                    {
                        MessageBoxCF msn = new MessageBoxCF("Xác nhận đơn hàng thành công", MessageType.Accept, MessageButtons.OK);
                        loadOrderList();
                        msn.ShowDialog();
                    }
                    else
                    {
                        MessageBoxCF msn = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                        msn.ShowDialog();
                    }
                }
                else
                {
                    MessageBoxCF msc = new MessageBoxCF(labelCreateBill, MessageType.Error, MessageButtons.OK);
                    msc.ShowDialog();
                }
                IsLoading = false;
            }
            MaskName.Visibility = Visibility.Collapsed;
        }

        public async void viewDetailOrder(OrderDTO Order)
        {
            ProductOrderList = new ObservableCollection<ProductOrderDTO>(Order.DanhSachSanPham);

            MaskName.Visibility = Visibility.Visible;

            DetailOrderWindow w = new DetailOrderWindow();
            w.ShowDialog();

            MaskName.Visibility = Visibility.Collapsed;
        }

        private async void loadStatusList()
        {
            StatusList = new ObservableCollection<string>
            {
                "Toàn bộ",
                Constants.StatusOrder.WAITTING,
                Constants.StatusOrder.CONFIRMED,
                Constants.StatusOrder.CANCEL,
            };
        }

        private void selectedStatus(string Status)
        {
            if (Status != null)
            {
                if (Status == "Toàn bộ")
                    OrderStatusList = new List<OrderDTO>(__OrderList);
                else
                    OrderStatusList = new List<OrderDTO>(__OrderList.FindAll(p => p.TrangThai == Status));
            }

            OrderList = new ObservableCollection<OrderDTO>(OrderSearchList.Intersect(OrderStatusList));
        }

        private void searchOrder(string text)
        {
            if (text != null)
            {
                if (__OrderList != null)
                    OrderSearchList = new List<OrderDTO>(__OrderList.FindAll(x => x.TenKhachHang.ToLower().Contains(text.ToLower())));
            }

            OrderList = new ObservableCollection<OrderDTO>(OrderSearchList.Intersect(OrderStatusList));
        }

        private async void reduceProduct(List<ProductOrderDTO> DanhSachSanPham)
        {
            var groupedData = DanhSachSanPham.GroupBy(item => item.MaSanPham)
                                .Select(group => new
                                {
                                    MaSanPham = group.Key,
                                    SoLuong = group.Sum(item => item.SoLuong)
                                }).ToList();

            foreach (var group in groupedData)
            {
                await ProductService.Ins.reduceQuantityProduct(group.MaSanPham, group.SoLuong);
            }

        }

        #endregion
    }
    

}
