using Coffee.DTOs;
using Coffee.Models;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DALs
{
    public class ChatDAL
    {
        private static ChatDAL _ins;
        public static ChatDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new ChatDAL();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// Tạo tin nhắn mới
        /// </summary>
        /// <param name="chat">tin nhắn</param>
        /// <param name="chatID">mã tin nhắn</param>
        /// <param name="userID">mã người dùng</param>
        /// <returns>
        ///     1. Thông báo
        ///     2. True khi tạo thành công
        /// </returns>
        public async Task<(string, bool)> createChat(ChatModel chat, string chatID, string userID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.SetTaskAsync("TinNhan/" + userID + "/" + chatID, chat);

                    return ("Thêm tin nhắn thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }

        /// <summary>
        ///     Lấy mã tin nhắn lớn nhất của người dùng
        /// </summary>
        /// <returns>
        ///     Mã người dùng lớn nhất
        /// </returns>
        public async Task<string> getMaxMaChat(string userID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("TinNhan/" + userID);

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, ChatDTO> data = response.ResultAs<Dictionary<string, ChatDTO>>();

                        string MaxMaChat = data.Keys.Max();

                        return MaxMaChat;
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
        /// Lấy danh sách tin nhắn của người dùng
        /// </summary>
        /// <returns>
        ///     Danh sách tin nhắn
        /// </returns>
        public async Task<(string, List<ChatDTO>)> getListChat(string userID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse chatResponse = await context.Client.GetTaskAsync("TinNhan/" + userID);
                    Dictionary<string, ChatDTO> chatData = chatResponse.ResultAs<Dictionary<string, ChatDTO>>();

                    var result = (from chat in chatData.Values
                                  select new ChatDTO
                                  {
                                      IsReceived = !string.IsNullOrEmpty(chat.MaKH),
                                      MaKH = chat.MaKH,
                                      NoiDung = chat.NoiDung,
                                      ThoiGian = chat.ThoiGian
                                  }).ToList();

                    return ("Lấy danh sách tin nhắn thành công", result);
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
        /// <param name="userID"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public async Task<(string, List<ChatDTO>)> getListChatByTime(string userID, DateTime datetime)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse chatResponse = await context.Client.GetTaskAsync("TinNhan/" + userID);
                    Dictionary<string, ChatDTO> chatData = chatResponse.ResultAs<Dictionary<string, ChatDTO>>();

                    var result = (from chat in chatData.Values
                                  where chat.ThoiGiandt > datetime && !string.IsNullOrEmpty(chat.MaKH)
                                  select new ChatDTO
                                  {
                                      IsReceived = !string.IsNullOrEmpty(chat.MaKH),
                                      MaKH = chat.MaKH,
                                      NoiDung = chat.NoiDung,
                                      ThoiGian = chat.ThoiGian
                                  }).ToList();

                    return ("Lấy danh sách tin nhắn thành công", result);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }
    }
}
