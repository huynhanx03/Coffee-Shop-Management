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
    public class UserContactService
    {
        private static UserContactService _ins;

        public static UserContactService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new UserContactService();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        ///     Trả về danh sách liên lạc người dùng
        /// </returns>
        public async Task<(string, List<UserContactDTO>)> getAllUserContact()
        {
            return await UserContactDAL.Ins.getAllUserContact();
        }
    }
}
