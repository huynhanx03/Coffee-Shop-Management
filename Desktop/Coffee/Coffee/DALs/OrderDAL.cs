using Coffee.DTOs;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DALs
{
    public class OrderDAL
    {
        private static OrderDAL _ins;
        public static OrderDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new OrderDAL();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Danh sách đơn hàng
        /// </returns>
        public async Task<(string, List<OrderDTO>)> getListOrder()
        {
            try
            {
                using (var context = new Firebase())
                {
                    // Lấy dữ liệu từ nút "Orders" trong Firebase
                    FirebaseResponse orderResponse = await context.Client.GetTaskAsync("DonHang");
                    Dictionary<string, OrderDTO> orderData = orderResponse.ResultAs<Dictionary<string, OrderDTO>>();

                    // Lấy dữ liệu từ nút "User" trong Firebase
                    FirebaseResponse userResponse = await context.Client.GetTaskAsync("NguoiDung");
                    Dictionary<string, UserDTO> userData = userResponse.ResultAs<Dictionary<string, UserDTO>>();

                    var result = (from order in orderData.Values
                                  join user in userData.Values
                                  on order.MaNguoiDung equals user.MaNguoiDung
                                  select new OrderDTO
                                  {
                                      MaDonHang = order.MaDonHang,
                                      MaNguoiDung = order.MaNguoiDung,
                                      NgayTaoDon = order.NgayTaoDon,
                                      PhiVanChuyen = order.PhiVanChuyen,
                                      TenKhachHang = user.HoTen,
                                      SanPham = order.SanPham,
                                      ThanhTien = order.ThanhTien,
                                      TrangThai = order.TrangThai
                                  }).ToList();

                    return ("Lấy danh sách đơn hàng thành công", result);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<(string, bool)> updateStatusOrder(string orderId, string status)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.UpdateTaskAsync("DonHang/" + orderId, new { TrangThai = status });

                    return ("Cập nhật đơn hàng thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }

        public async Task<(string, List<ProductOrderDTO>)> getDetailOrder(string orderID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    // Lấy dữ liệu từ nút "Orders" trong Firebase
                    FirebaseResponse detailOrderResponse = await context.Client.GetTaskAsync("DonHang/" + orderID + "/SanPham");
                    Dictionary<string, ProductOrderDTO> detailOrderData = detailOrderResponse.ResultAs<Dictionary<string, ProductOrderDTO>>();

                    return ("Lấy chi tiết đơn hàng thành công", detailOrderData.Values.ToList());
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        public async Task<(string, bool)> updateBillIDOrder(string orderId, string billID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.UpdateTaskAsync("DonHang/" + orderId, new { MaHoaDon = billID });

                    return ("Cập nhật đơn hàng thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }
    }
}
