using Coffee.API;
using Coffee.DALs;
using Coffee.DTOs;
using Coffee.Models;
using Coffee.Utils;
using Coffee.Utils.Helper;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Services
{
    public class CustomerService
    {
        private static CustomerService _ins;
        public static CustomerService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new CustomerService();
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
        public async Task<(string, bool)> updatePointRankCustomer(string customerID, double pointRank)
        {
            return await CustomerAPI.Ins.plusPoint(customerID, pointRank);

            //(string label, CustomerDTO customer) = await CustomerDAL.Ins.getCustomer(customerID);

            //return await CustomerDAL.Ins.updatePointRankCustomer(customerID, customer.DiemTichLuy + pointRank);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="pointRank"></param>
        /// <returns></returns>
        public async Task<(string, bool)> checkUpdateRankCustomer(string customerID, double pointRank)
        {
            (string label, string rankID) = await CustomerDAL.Ins.getRankCustomer(customerID);

            string nextRankID = Helper.nextID(rankID, "TT");

            (string labelGetRank, RankModel rank) = await RankService.Ins.getRank(nextRankID);

            if (rank != null)
            {
                if (pointRank >= rank.DiemMucDoThanThiet)
                {
                    DetailRankModel detailRank = new DetailRankModel
                    {
                        MaMucDoThanThiet = rank.MaMucDoThanThiet,
                        NgayDatDuoc = DateTime.Now.ToString("dd/MM/yyyy")
                    };

                    return await CustomerDAL.Ins.createRankCustomer(customerID, detailRank);
                }
            }

            return ("Lỗi", false);
        }

        /// <summary>
        /// Láy rank của khách hàng
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public async Task<(string, string)> getRankCustomer(string customerID)
        {
            return await CustomerDAL.Ins.getRankCustomer(customerID);
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
            return await CustomerAPI.Ins.createCustomer(Customer);

            //// Tạo mã nhân viên mới nhất
            //string MaxMaKhachHang = await CustomerDAL.Ins.getMaxMaKhachHang();
            //string NewMaKhachHang = Helper.nextID(MaxMaKhachHang, "KH");

            //// Tạo Customer
            //CustomerDTO _Customer = new CustomerDTO
            //{
            //    MaKhachHang = NewMaKhachHang,
            //    DiemTichLuy = 0
            //};

            //// Thêm user
            //UserDTO user = new UserDTO
            //{
            //    HoTen = Customer.HoTen,
            //    CCCD_CMND = Customer.CCCD_CMND,
            //    DiaChi = Customer.DiaChi,
            //    Email = Customer.Email,
            //    GioiTinh = Customer.GioiTinh,
            //    NgaySinh = Customer.NgaySinh,
            //    NgayTao = Customer.NgayTao,
            //    SoDienThoai = Customer.SoDienThoai,
            //    TaiKhoan = Customer.TaiKhoan,
            //    MatKhau = Customer.MatKhau,
            //    VaiTro = 2,
            //    MaNguoiDung = NewMaKhachHang,
            //    HinhAnh = Customer.HinhAnh
            //};

            ////Kiểm tra có trùng CCCD/CMND không
            //bool IsCheckIDCard = await UserService.Ins.checkIDCard(user);

            //if (IsCheckIDCard)
            //    return ("CCCD/CMND đã tồn tại", null);

            ////Kiểm tra có trùng email không
            //bool IsCheckEmail = await UserService.Ins.checkEmail(user);

            //if (IsCheckEmail)
            //    return ("Email đã tồn tại", null);

            ////Kiểm tra có trùng số điện thoại không
            //bool IsCheckNumberPhone = await UserService.Ins.checkNumberPhone(user);

            //if (IsCheckNumberPhone)
            //    return ("Số điện thoại đã tồn tại", null);

            ////Kiểm tra có trùng tên đăng nhập không
            //bool IsCheckUsername = await UserService.Ins.checkUsername(user);

            //if (IsCheckUsername)
            //    return ("Tên tài khoản đã tồn tại", null);

            //(string labelCustomer, CustomerDTO __Customer) = await CustomerDAL.Ins.createCustomer(_Customer);
            //(string labelUser, UserDTO _user) = await UserService.Ins.createUser(user);

            //Customer.MaKhachHang = NewMaKhachHang;

            //if (_user != null && __Customer != null)
            //{
            //    // Thêm rank
            //    DetailRankModel detailRankModel = new DetailRankModel
            //    {
            //        MaMucDoThanThiet = "TT0001",
            //        NgayDatDuoc = DateTime.Now.ToString("dd/MM/yyyy")
            //    };

            //    CustomerDAL.Ins.createRankCustomer(NewMaKhachHang, detailRankModel);
            //    CustomerDAL.Ins.createRankCustomerBegin(NewMaKhachHang);
            //    return ("Thêm khách hàng thành công", Customer);
            //}
            //else
            //{
            //    return ("Thêm khách hàng thất bại", null);
            //}
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
            return await CustomerAPI.Ins.updateCustomer(Customer);

            //// Tạo Customer
            //CustomerDTO _Customer = new CustomerDTO
            //{
            //    MaKhachHang = Customer.MaKhachHang,
            //    DiemTichLuy = 0
            //};

            //// Thêm user
            //UserDTO user = new UserDTO
            //{
            //    HoTen = Customer.HoTen,
            //    CCCD_CMND = Customer.CCCD_CMND,
            //    DiaChi = Customer.DiaChi,
            //    Email = Customer.Email,
            //    GioiTinh = Customer.GioiTinh,
            //    NgaySinh = Customer.NgaySinh,
            //    NgayTao = Customer.NgayTao,
            //    SoDienThoai = Customer.SoDienThoai,
            //    TaiKhoan = Customer.TaiKhoan,
            //    MatKhau = Customer.MatKhau,
            //    VaiTro = 2,
            //    MaNguoiDung = Customer.MaKhachHang,
            //    HinhAnh = Customer.HinhAnh
            //};

            ////Kiểm tra có trùng CCCD/CMND không
            //bool IsCheckIDCard = await UserService.Ins.checkIDCard(user);

            //if (IsCheckIDCard)
            //    return ("CCCD/CMND đã tồn tại", null);

            ////Kiểm tra có trùng email không
            //bool IsCheckEmail = await UserService.Ins.checkEmail(user);

            //if (IsCheckEmail)
            //    return ("Email đã tồn tại", null);

            ////Kiểm tra có trùng số điện thoại không
            //bool IsCheckNumberPhone = await UserService.Ins.checkNumberPhone(user);

            //if (IsCheckNumberPhone)
            //    return ("Số điện thoại đã tồn tại", null);

            ////Kiểm tra có trùng tên đăng nhập không
            //bool IsCheckUsername = await UserService.Ins.checkNumberPhone(user);

            //if (IsCheckUsername)
            //    return ("Tên tài khoản đã tồn tại", null);

            //(string labelCustomer, CustomerDTO __Customer) = await CustomerDAL.Ins.updateCustomer(_Customer);
            //(string labelUser, UserDTO _user) = await UserService.Ins.updateUser(user);

            //if (_user != null && __Customer != null)
            //{
            //    return ("Cập nhật khách hàng thành công", Customer);
            //}
            //else
            //{
            //    return ("Cập nhật khách hàng thất bại", null);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Danh sách Khách hàng
        /// </returns>
        public async Task<(string, List<CustomerDTO>)> getListCustomer()
        {
            return await CustomerAPI.Ins.getCustomers();
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
        public async Task<(string, bool)> DeleteCustomer(CustomerDTO customer)
        {
            return await CustomerAPI.Ins.DeleteCustomer(customer.MaKhachHang);

            //(string labelEmployee, bool isDeleteEmployee) = await CustomerDAL.Ins.DeleteCustomer(customer.MaKhachHang);
            //(string labelUser, bool isDeleteUser) = await UserDAL.Ins.DeleteUser(customer.MaKhachHang);
            //(string labelRank, bool isDeleteRank) = await CustomerDAL.Ins.deleteRankCustomer(customer.MaKhachHang);
            //(string labelAddress, bool isDeleteAddress) = await CustomerDAL.Ins.deleteAddressCustomer(customer.MaKhachHang);

            //if (isDeleteUser)
            //{
            //    await CloudService.Ins.DeleteImage(customer.HinhAnh);
            //}

            //if (isDeleteEmployee && isDeleteUser && isDeleteRank && isDeleteAddress)
            //{
            //    return (labelEmployee, true);
            //}
            //else
            //{
            //    if (!isDeleteEmployee)
            //        return (labelEmployee, false);

            //    if (!isDeleteUser)
            //        return (labelUser, false);

            //    if (!isDeleteAddress)
            //        return (labelAddress, false);

            //    return (labelRank, false);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public async Task<(string, List<AddressModel>)> getListAddressCustomer(string customerID)
        {
            return await CustomerAPI.Ins.getAddressCustomer(customerID);
        }
    }
}
