using Coffee.DTOs;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Coffee.DALs
{
    public class EmployeeDAL
    {
        private static EmployeeDAL _ins;
        public static EmployeeDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new EmployeeDAL();
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
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.SetTaskAsync("NhanVien/" + employee.MaNhanVien, employee);

                    return ("Thêm nhân viên thành công",  employee);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
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
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.UpdateTaskAsync("NhanVien/" + employee.MaNhanVien, employee);

                    return ("Cập nhật nhân viên thành công", employee);
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
        ///     Mã nhân viên lớn nhất
        /// </returns>
        public async Task<string> getMaxMaNhanVien()
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("NhanVien");

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, EmployeeDTO> data = response.ResultAs<Dictionary<string, EmployeeDTO>>();

                        string MaxMaNhanVien = data.Values.Select(p => p.MaNhanVien).Max();

                        return MaxMaNhanVien;
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
        ///     Lấy thông tin nhân viên
        /// </returns>
        public async Task<EmployeeDTO> getDetailEmployee(string EmployeeID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    // Lấy dữ liệu từ nút "NhanVien" trong Firebase
                    FirebaseResponse employeesResponse = await context.Client.GetTaskAsync("NhanVien");
                    // Lấy dữ liệu từ nút "NguoiDung" trong Firebase
                    FirebaseResponse userResponse = await context.Client.GetTaskAsync("NguoiDung");

                    if (employeesResponse.Body != null && employeesResponse.Body != "null" && userResponse.Body != null && userResponse.Body != "null")
                    {
                        Dictionary<string, EmployeeDTO> employdata = employeesResponse.ResultAs<Dictionary<string, EmployeeDTO>>();
                        Dictionary<string, UserDTO> userdata = userResponse.ResultAs<Dictionary<string, UserDTO>>();

                        EmployeeDTO Employee = (from employee in employdata.Values join user in userdata.Values
                                                on employee.MaNhanVien equals user.MaNguoiDung where employee.MaNhanVien == EmployeeID select new EmployeeDTO
                                                {
                                                    HoTen = user.HoTen,
                                                }).FirstOrDefault();
                        return Employee;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                throw new Exception("Có lỗi xảy ra: " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Danh sách nhân viên
        /// </returns>
        public async Task<(string, List<EmployeeDTO>)> getListEmployee()
        {
            try
            {
                using (var context = new Firebase())
                {
                    // Lấy dữ liệu từ nút "Employees" trong Firebase
                    FirebaseResponse employeesResponse = await context.Client.GetTaskAsync("NhanVien");
                    Dictionary<string, EmployeeDTO> employeesData = employeesResponse.ResultAs<Dictionary<string, EmployeeDTO>>();

                    // Lấy dữ liệu từ nút "Users" trong Firebase
                    FirebaseResponse usersResponse = await context.Client.GetTaskAsync("NguoiDung");
                    Dictionary<string, UserDTO> usersData = usersResponse.ResultAs<Dictionary<string, UserDTO>>();

                    // Lấy dữ liệu từ nút "Position" trong Firebase
                    FirebaseResponse positionResponse = await context.Client.GetTaskAsync("ChucDanh");
                    Dictionary<string, PositionDTO> positionData = positionResponse.ResultAs<Dictionary<string, PositionDTO>>();


                    var result = (from employee in employeesData.Values
                                  join user in usersData.Values 
                                  on employee.MaNhanVien equals user.MaNguoiDung
                                  join position in positionData.Values
                                  on employee.MaChucVu equals position.MaChucVu
                                  select new EmployeeDTO
                                  {
                                      HoTen = user.HoTen,
                                      CCCD_CMND = user.CCCD_CMND,
                                      DiaChi = user.DiaChi,
                                      Email = user.Email,
                                      GioiTinh = user.GioiTinh,
                                      HinhAnh = user.HinhAnh,
                                      Luong = employee.Luong,
                                      SoDienThoai = user.SoDienThoai,
                                      MaNhanVien = employee.MaNhanVien,
                                      MatKhau = user.MatKhau,
                                      NgaySinh = user.NgaySinh,
                                      NgayLam = user.NgayTao,
                                      TaiKhoan = user.TaiKhoan,
                                      MaChucVu = employee.MaChucVu,
                                      TenChucVu = position.TenChucVu
                                  }).ToList();

                    return ("Lấy danh sách nhân viên thành công", result);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
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
        public async Task<(string, bool)> DeleteEmployee(string EmployeeID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.DeleteTaskAsync("NhanVien/" + EmployeeID);
                    return ("Xoá nhân viên thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }
    }
}
