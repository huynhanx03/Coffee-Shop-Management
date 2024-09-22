using Coffee.API;
using Coffee.DALs;
using Coffee.DTOs;
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
    public class EmployeeService
    {
        private static EmployeeService _ins;
        public static EmployeeService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new EmployeeService();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// Thêm nhân viên
        /// INPUT: employee: Nhân viên
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>
        ///     1: Lỗi khi thêm dữ liệu
        ///     2: Nhân viên
        /// </returns>
        public async Task<(string, EmployeeDTO)> createEmpoloyee(EmployeeDTO employee)
        {
            return await EmployeeAPI.Ins.createEmployee(employee);

            // Tạo mã nhân viên mới nhất
            string MaxMaNhanVien = await this.getMaxMaNhanVien();
            string NewMaNhanVien = Helper.nextID(MaxMaNhanVien, "NV");

            // Tạo employee
            EmployeeDTO _employee = new EmployeeDTO { 
                MaNhanVien = NewMaNhanVien,
                MaChucVu = employee.MaChucVu,
                Luong = employee.Luong,
            };

            // Thêm user
            UserDTO user = new UserDTO {
                HoTen = employee.HoTen,
                CCCD_CMND = employee.CCCD_CMND,
                DiaChi = employee.DiaChi,
                Email = employee.Email,
                GioiTinh = employee.GioiTinh,
                NgaySinh = employee.NgaySinh,
                NgayTao = employee.NgayLam,
                SoDienThoai = employee.SoDienThoai,
                TaiKhoan = employee.TaiKhoan,
                MatKhau = employee.MatKhau,
                VaiTro = 3,
                MaNguoiDung = NewMaNhanVien,
                HinhAnh = employee.HinhAnh
            };

            //Kiểm tra có trùng CCCD/CMND không
            bool IsCheckIDCard = await UserService.Ins.checkIDCard(user);

            if (IsCheckIDCard)
                return ("CCCD/CMND đã tồn tại", null);

            //Kiểm tra có trùng email không
            bool IsCheckEmail = await UserService.Ins.checkEmail(user);

            if (IsCheckEmail)
                return ("Email đã tồn tại", null);

            //Kiểm tra có trùng số điện thoại không
            bool IsCheckNumberPhone = await UserService.Ins.checkNumberPhone(user);

            if (IsCheckNumberPhone)
                return ("Số điện thoại đã tồn tại", null);

            //Kiểm tra có trùng tên đăng nhập không
            bool IsCheckUsername = await UserService.Ins.checkNumberPhone(user);

            if (IsCheckUsername)
                return ("Tên tài khoản đã tồn tại", null);

            (string labelEmployee, EmployeeDTO __employee) = await EmployeeDAL.Ins.createEmpoloyee(_employee);
            (string labelUser, UserDTO _user) = await UserService.Ins.createUser(user);

            employee.MaNhanVien = NewMaNhanVien;

            if (_user != null && __employee != null)
            {
                return ("Thêm nhân viên thành công", employee);
            }
            else
            {
                return ("Thêm nhân viên thất bại", null);
            }
        }

        /// <summary>
        /// Cập nhật nhân viên
        /// INPUT: employee: Nhân viên
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: Nhân viên
        /// </returns>
        public async Task<(string, EmployeeDTO)> updateEmpoloyee(EmployeeDTO employee)
        {
            return await EmployeeAPI.Ins.updateEmployee(employee);

            // Tạo employee
            EmployeeDTO _employee = new EmployeeDTO
            {
                MaNhanVien = employee.MaNhanVien,
                MaChucVu = employee.MaChucVu,
                Luong = employee.Luong,
            };

            // Thêm user
            UserDTO user = new UserDTO
            {
                HoTen = employee.HoTen,
                CCCD_CMND = employee.CCCD_CMND,
                DiaChi = employee.DiaChi,
                Email = employee.Email,
                GioiTinh = employee.GioiTinh,
                NgaySinh = employee.NgaySinh,
                NgayTao = employee.NgayLam,
                SoDienThoai = employee.SoDienThoai,
                TaiKhoan = employee.TaiKhoan,
                MatKhau = employee.MatKhau,
                VaiTro = 3,
                MaNguoiDung = employee.MaNhanVien,
                HinhAnh = employee.HinhAnh
            };

            //Kiểm tra có trùng CCCD/CMND không
            bool IsCheckIDCard = await UserService.Ins.checkIDCard(user);

            if (IsCheckIDCard)
                return ("CCCD/CMND đã tồn tại", null);

            //Kiểm tra có trùng email không
            bool IsCheckEmail = await UserService.Ins.checkEmail(user);

            if (IsCheckEmail)
                return ("Email đã tồn tại", null);

            //Kiểm tra có trùng số điện thoại không
            bool IsCheckNumberPhone = await UserService.Ins.checkNumberPhone(user);

            if (IsCheckNumberPhone)
                return ("Số điện thoại đã tồn tại", null);

            //Kiểm tra có trùng tên đăng nhập không
            bool IsCheckUsername = await UserService.Ins.checkNumberPhone(user);

            if (IsCheckUsername)
                return ("Tên tài khoản đã tồn tại", null);

            (string labelEmployee, EmployeeDTO __employee) = await EmployeeDAL.Ins.updateEmpoloyee(_employee);
            (string labelUser, UserDTO _user) = await UserService.Ins.updateUser(user);

            if (_user != null && __employee != null)
            {
                return ("Cập nhật nhân viên thành công", employee);
            }
            else
            {
                return ("Cập nhật nhân viên thất bại", null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Mã nhân viên lớn nhất
        /// </returns>
        public async Task<string> getMaxMaNhanVien()
        {
            return await EmployeeDAL.Ins.getMaxMaNhanVien();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     lấy thông tin nhân viên
        /// </returns>
        public async Task<EmployeeDTO> getDetailEmployee(string employeeID)
        {
            return await EmployeeDAL.Ins.getDetailEmployee(employeeID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Danh sách nhân viên
        /// </returns>
        public async Task<(string, List<EmployeeDTO>)> getListEmployee()
        {
            return await EmployeeAPI.Ins.getEmployees();
        }

        /// <summary>
        /// Xoá nhân viên
        /// INPUT:
        ///     EmployeeID: mã nhân viên
        /// </summary>
        /// <param name="EmployeeID"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> DeleteEmployee(EmployeeDTO Employee)
        {
            return await EmployeeAPI.Ins.DeleteEmployee(Employee.MaNhanVien);

            (string labelEmployee, bool isDeleteEmployee) = await EmployeeDAL.Ins.DeleteEmployee(Employee.MaNhanVien);
            (string labelUser, bool isDeleteUser) = await UserDAL.Ins.DeleteUser(Employee.MaNhanVien);

            if (isDeleteUser)
            {
                await CloudService.Ins.DeleteImage(Employee.HinhAnh);
            }

            if (isDeleteEmployee && isDeleteUser)
            {
                return (labelEmployee, true);
            }
            else
            {
                if (!isDeleteEmployee)
                    return (labelEmployee, false);

                if (!isDeleteUser) 
                    return (labelUser, false);

                return (labelEmployee, false);
            }
        }
    }
}
