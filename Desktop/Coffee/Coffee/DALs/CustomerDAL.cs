using Coffee.DTOs;
using Coffee.Models;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DALs
{
    public class CustomerDAL
    {
        private static CustomerDAL _ins;
        public static CustomerDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new CustomerDAL();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// Cập nhật điểm cho khách hàng
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="pointRank"></param>
        /// <returns></returns>
        public async Task<(string, double)> updatePointRankCustomer(string customerID, double pointRank)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.UpdateTaskAsync("KhachHang/" + customerID, new { DiemTichLuy = pointRank });

                    return ("Cập nhật điểm thành công", pointRank);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, -1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public async Task<(string, CustomerDTO)> getCustomer(string customerID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("KhachHang/" + customerID);

                    CustomerDTO customer = response.ResultAs<CustomerDTO>();

                    return ("Lấy khách hàng thành công", customer);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Láy rank của khách hàng
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public async Task<(string, string)> getRankCustomer(string customerID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("ChiTietMucDoThanThiet/" + customerID + "/ChiTiet");

                    Dictionary<string, DetailRankModel>  detailRankData = response.ResultAs<Dictionary<string, DetailRankModel>>();

                    return ("Lấy mức độ thân thiết của khách hàng thành công", detailRankData.Last().Value.MaMucDoThanThiet);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<(string, List<string>)> getCustomerIDByRankMininum(string rankID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("ChiTietMucDoThanThiet/");

                    Dictionary<string, string> customerData = response.ResultAs<Dictionary<string, string>>();

                    List<string> customerIDList = customerData.Keys.ToList();
                    
                    List<string> result = new List<string>();

                    foreach (var customerID in customerIDList)
                    {
                        FirebaseResponse responseDetailRank = await context.Client.GetTaskAsync("ChiTietMucDoThanThiet/" + customerID + "/ChiTiet");

                        Dictionary<string, DetailRankModel> detailRankData = responseDetailRank.ResultAs<Dictionary<string, DetailRankModel>>();

                        int compare = string.Compare(detailRankData.Last().Value.MaMucDoThanThiet, rankID);

                        if (compare >= 0)
                            result.Add(customerID);
                    }

                    return ("Lấy danh sách mã khách hàng thành công", result);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Tạo mức độ thân thiết mới cho khách hàng
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="detailRank"></param>
        /// <returns></returns>
        public async Task<(string, bool)> createRankCustomer(string customerID, DetailRankModel detailRank)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.SetTaskAsync("ChiTietMucDoThanThiet/" + customerID + "/ChiTiet/" + detailRank.MaMucDoThanThiet, detailRank);

                    return ("Tạo mức độ thân thiết của khách hàng thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }

        public async Task<(string, bool)> createRankCustomerBegin(string customerID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.SetTaskAsync("ChiTietMucDoThanThiet/" + customerID, new {MaKhachHang = customerID});

                    return ("Tạo mức độ thân thiết của khách hàng thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }


        /// <summary>
        /// Thêm Khách hàng
        /// INPUT: Customer: Khách hàng
        /// </summary>
        /// <param name="Customer"></param>
        /// <returns>
        ///     1: Lỗi khi thêm dữ liệu
        ///     2: Khách hàng
        /// </returns>
        public async Task<(string, CustomerDTO)> createCustomer(CustomerDTO Customer)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.SetTaskAsync("KhachHang/" + Customer.MaKhachHang, Customer);

                    return ("Thêm Khách hàng thành công", Customer);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Cập nhật Khách hàng
        /// INPUT: Customer: Khách hàng
        /// </summary>
        /// <param name="Customer"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: Khách hàng
        /// </returns>
        public async Task<(string, CustomerDTO)> updateCustomer(CustomerDTO Customer)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.UpdateTaskAsync("KhachHang/" + Customer.MaKhachHang, Customer);

                    return ("Cập nhật Khách hàng thành công", Customer);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Mã Khách hàng lớn nhất
        /// </returns>
        public async Task<string> getMaxMaKhachHang()
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("KhachHang");

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, CustomerDTO> data = response.ResultAs<Dictionary<string, CustomerDTO>>();

                        string MaxMaKhachHang = data.Values.Select(p => p.MaKhachHang).Max();

                        return MaxMaKhachHang;
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Danh sách Khách hàng
        /// </returns>
        public async Task<(string, List<CustomerDTO>)> getListCustomer()
        {
            try
            {
                using (var context = new Firebase())
                {
                    // Lấy dữ liệu từ nút "Customers" trong Firebase
                    FirebaseResponse CustomersResponse = await context.Client.GetTaskAsync("KhachHang");
                    Dictionary<string, CustomerDTO> CustomersData = CustomersResponse.ResultAs<Dictionary<string, CustomerDTO>>();

                    // Lấy dữ liệu từ nút "Users" trong Firebase
                    FirebaseResponse usersResponse = await context.Client.GetTaskAsync("NguoiDung");
                    Dictionary<string, UserDTO> usersData = usersResponse.ResultAs<Dictionary<string, UserDTO>>();

                    var result = (from Customer in CustomersData.Values
                                  join user in usersData.Values
                                  on Customer.MaKhachHang equals user.MaNguoiDung
                                  select new CustomerDTO
                                  {
                                      HoTen = user.HoTen,
                                      CCCD_CMND = user.CCCD_CMND,
                                      DiaChi = user.DiaChi,
                                      Email = user.Email,
                                      GioiTinh = user.GioiTinh,
                                      HinhAnh = user.HinhAnh,
                                      SoDienThoai = user.SoDienThoai,
                                      MaKhachHang = Customer.MaKhachHang,
                                      DiemTichLuy = Customer.DiemTichLuy,
                                      MatKhau = user.MatKhau,
                                      NgaySinh = user.NgaySinh,
                                      TaiKhoan = user.TaiKhoan,
                                      NgayTao = user.NgayTao,
                                  }).ToList();

                    return ("Lấy danh sách Khách hàng thành công", result);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Xoá Khách hàng
        /// INPUT:
        ///     CustomerID: mã Khách hàng
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> DeleteCustomer(string CustomerID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.DeleteTaskAsync("KhachHang/" + CustomerID);
                    return ("Xoá Khách hàng thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }

        /// <summary>
        /// Xoá mức độ thân thiết của khách hàng
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public async Task<(string, bool)> deleteRankCustomer(string customerID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.DeleteTaskAsync("ChiTietMucDoThanThiet/" + customerID);
                    return ("Xoá chi tiết mức độ thân thiết khách hàng thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }

        /// <summary>
        /// Xoá địa chỉ khách hàng
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public async Task<(string, bool)> deleteAddressCustomer(string customerID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.DeleteTaskAsync("Diachi/" + customerID);
                    return ("Xoá địa chỉ khách hàng thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public async Task<(string, List<AddressModel>)> getListAddressCustomer(string customerID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse AddressCustomersResponse = await context.Client.GetTaskAsync("DiaChi/" + customerID);
                    Dictionary<string, AddressModel> AdressCustomersData = AddressCustomersResponse.ResultAs<Dictionary<string, AddressModel>>();

                    return ("Lấy danh sách địa chỉ khách hàng thành công", AdressCustomersData.Values.ToList());
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }
    }
}
