using Coffee.DALs;
using Coffee.DTOs;
using Coffee.Models;
using Coffee.Utils.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Services
{
    public class ChatService
    {
        private static ChatService _ins;
        public static ChatService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new ChatService();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// Lấy danh sách tin nhắn của người dùng
        /// </summary>
        /// <returns>
        ///     Danh sách tin nhắn
        /// </returns>
        public async Task<(string, List<ChatDTO>)> getListChat(string userID)
        {
            return await ChatDAL.Ins.getListChat(userID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<(string, bool)> createChat(ChatModel chat, string userID)
        {
            string maxMaChat = await ChatDAL.Ins.getMaxMaChat(userID);

            string newMaChat = Helper.nextID(maxMaChat, "TN");

            return await ChatDAL.Ins.createChat(chat, newMaChat, userID);
        }

        // <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public async Task<(string, List<ChatDTO>)> getListChatByTime(string userID, DateTime datetime)
        {
            return await ChatDAL.Ins.getListChatByTime(userID, datetime);
        }
    }
}
