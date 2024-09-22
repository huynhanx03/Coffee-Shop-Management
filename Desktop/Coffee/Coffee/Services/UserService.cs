using Coffee.API;
using Coffee.DALs;
using Coffee.DTOs;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Services
{
    public class UserService
    {
        private static UserService _ins;
        public static UserService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new UserService();
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
            return await UserDAL.Ins.createUser(user);
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
            return await UserAPI.Ins.updateUser(user);
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
            return await UserDAL.Ins.checkEmail(user);
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
            return await UserDAL.Ins.checkNumberPhone(user);
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
            return await UserDAL.Ins.checkIDCard(user);
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
            return await UserDAL.Ins.checkUsername(user);
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
           return await UserDAL.Ins.DeleteUser(UserID);
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
            return await UserAPI.Ins.login(username, password);
            return await UserDAL.Ins.findUser(username, password);
        }

        /// <summary>
        /// Kiểm tra số điện thoại đã tồn tại chưa
        /// </summary>
        /// <param name="userNumberPhone"></param>
        /// <returns>
        /// </returns>
        public async Task<UserDTO> getUserByNumberphone(string userNumberPhone)
        {
            return (await UserAPI.Ins.getUserByNumberPhone(userNumberPhone)).Item2;
        }

        public async Task<(string, bool)> checkToken()
        {
            return await UserAPI.Ins.checkToken();
        }

        public async Task<(string, UserDTO)> GetUser(string userID)
        {
            return await UserAPI.Ins.GetUser(userID);
        }
    }
}
