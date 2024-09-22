using Coffee.DTOs;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DALs
{
    public class UserDAL
    {
        private static UserDAL _ins;
        public static UserDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new UserDAL();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// Thêm người dùng
        /// INPUT: user: Người dùng
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        ///     1: Lỗi khi thêm dữ liệu
        ///     2: Người dùng
        /// </returns>
        public async Task<(string, UserDTO)> createUser(UserDTO user)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.SetTaskAsync("NguoiDung/" + user.MaNguoiDung, user);

                    return ("Thêm người dùng thành công", user);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Cập nhật người dùng
        /// INPUT: user: Người dùng
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        ///     1: Lỗi khi thêm dữ liệu
        ///     2: Người dùng
        /// </returns>
        public async Task<(string, UserDTO)> updateUser(UserDTO user)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.UpdateTaskAsync("NguoiDung/" + user.MaNguoiDung, user);

                    return ("Cập nhật người dùng thành công", user);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Kiểm tra email đã tồn tại chưa
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        ///     True: Đã tồn tại
        ///     False: Chưa tồn tại
        /// </returns>

        public async Task<bool> checkEmail(UserDTO user)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("NguoiDung");
                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, UserDTO> data = response.ResultAs<Dictionary<string, UserDTO>>();

                        UserDTO _user = data.Values.FirstOrDefault(u => u.MaNguoiDung != user.MaNguoiDung && u.Email == user.Email);

                        if (_user != null)
                            return true;
                        else
                            return false;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra số điện thoại đã tồn tại chưa
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        ///     True: Đã tồn tại
        ///     False: Chưa tồn tại
        /// </returns>
        public async Task<bool> checkNumberPhone(UserDTO user)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("NguoiDung");
                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, UserDTO> data = response.ResultAs<Dictionary<string, UserDTO>>();

                        UserDTO _user = data.Values.FirstOrDefault(u => u.MaNguoiDung != user.MaNguoiDung && u.SoDienThoai == user.SoDienThoai);

                        if (_user != null)
                            return true;
                        else
                            return false;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra số điện thoại đã tồn tại chưa
        /// </summary>
        /// <param name="userNumberPhone"></param>
        /// <returns>
        /// </returns>
        public async Task<UserDTO> getUserByNumberphone(string userNumberPhone)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("NguoiDung");
                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, UserDTO> data = response.ResultAs<Dictionary<string, UserDTO>>();

                        UserDTO user = data.Values.FirstOrDefault(u => u.SoDienThoai == userNumberPhone);

                        return user;
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra CCCD/CMND đã tồn tại chưa
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        ///     True: Đã tồn tại
        ///     False: Chưa tồn tại
        /// </returns>
        public async Task<bool> checkIDCard(UserDTO user)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("NguoiDung");
                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, UserDTO> data = response.ResultAs<Dictionary<string, UserDTO>>();

                        UserDTO _user = data.Values.FirstOrDefault(u => u.MaNguoiDung != user.MaNguoiDung && u.CCCD_CMND == user.CCCD_CMND);

                        if (_user != null)
                            return true;
                        else
                            return false;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra tên tài khoản đã tồn tại chưa
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        ///     True: Đã tồn tại
        ///     False: Chưa tồn tại
        /// </returns>
        public async Task<bool> checkUsername(UserDTO user)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("NguoiDung");
                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, UserDTO> data = response.ResultAs<Dictionary<string, UserDTO>>();

                        UserDTO _user = data.Values.FirstOrDefault(u => u.MaNguoiDung != user.MaNguoiDung && u.TaiKhoan == user.TaiKhoan);

                        if (_user != null)
                            return true;
                        else
                            return false;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Xoá người dùng
        /// INPUT:
        ///     UserID: mã người dùng
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> DeleteUser(string UserID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.DeleteTaskAsync("NguoiDung/" + UserID);
                    return ("Xoá người dùng thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }

        /// <summary>
        /// Tìm kiếm người dùng
        /// </summary>
        /// <param name="username">Tài khoản</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>
        ///     1: Thông báo
        ///     2: Người dùng
        /// </returns>
        public async Task<(string, UserDTO)> findUser(string username, string password)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("NguoiDung");
                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, UserDTO> data = response.ResultAs<Dictionary<string, UserDTO>>();

                        UserDTO _user = data.Values.FirstOrDefault(u => u.TaiKhoan == username && u.MatKhau == password);

                        return ("Tìm thấy tài khoản thành công", _user);
                    }

                    return ("Tìm tài khoản thất bại", null);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }
    }
}
