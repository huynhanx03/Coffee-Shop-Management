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
    public class UserContactDAL
    {
        private static UserContactDAL _ins;

        public static UserContactDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new UserContactDAL();
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
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("TinNhan");

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, string> data = response.ResultAs<Dictionary<string, string>>();

                        // Lấy dữ liệu từ nút "NguoiDung" trong Firebase
                        FirebaseResponse userResponse = await context.Client.GetTaskAsync("NguoiDung");
                        Dictionary<string, UserDTO> userdata = userResponse.ResultAs<Dictionary<string, UserDTO>>();
                        List<UserDTO> ListUser = userdata.Values.ToList();

                        // Chuyển đổi từ điển thành danh sách
                        List<string> ListCustomerId = data.Keys.ToList();

                        List<UserContactDTO> listUserContact = new List<UserContactDTO>();

                        foreach (string CustomerId in ListCustomerId)
                        {
                            FirebaseResponse responseChat = await context.Client.GetTaskAsync("TinNhan/" + CustomerId);
                            Dictionary<string, ChatDTO> dataChat = responseChat.ResultAs<Dictionary<string, ChatDTO>>();

                            var chatDTO = dataChat.Values;
                            List<ChatDTO> ListChat = dataChat.Values.ToList();

                            var lastChat = ListChat.Last(x => !string.IsNullOrEmpty(x.MaKH));

                            var user = ListUser.First(x => x.MaNguoiDung == CustomerId);

                            UserContactDTO userContact = new UserContactDTO
                            {
                                MaKhachHang = user.MaNguoiDung,
                                HoTen = user.HoTen,
                                HinhAnh = user.HinhAnh,
                                ThoiGianTinNhanCuoiCung = lastChat.ThoiGiandt.ToString("dd/MM"),
                                TinNhanCuoiCung = lastChat.NoiDung
                            };

                            listUserContact.Add(userContact);
                        };

                        return ("Lấy danh sách liên hệ người dùng thành công", listUserContact);
                    }
                    else
                    {
                        return ("Lấy danh sách đơn vị thành công", new List<UserContactDTO>());
                    }
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }
    }
}
