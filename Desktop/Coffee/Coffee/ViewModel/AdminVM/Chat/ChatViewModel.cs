using Coffee.API;
using Coffee.DTOs;
using Coffee.Models;
using Coffee.Services;
using Coffee.Views.MessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace Coffee.ViewModel.AdminVM.Chat
{
    public partial class ChatViewModel: BaseViewModel
    {
        #region variable
        private ObservableCollection<UserContactDTO> _UserContactList;

        public ObservableCollection<UserContactDTO> UserContactList
        {
            get { return _UserContactList; }
            set { _UserContactList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ChatDTO> _ChatList;

        public ObservableCollection<ChatDTO> ChatList
        {
            get { return _ChatList; }
            set { _ChatList = value; OnPropertyChanged(); }
        }

        private string UserID { get; set; }
        private DateTime timeNow{ get; set; }

        private string _MessageText;
        public string MessageText
        {
            get { return _MessageText; }
            set { _MessageText = value; OnPropertyChanged(); }
        }

        private ScrollViewer scrollChat {  get; set; }

        #endregion

        #region ICommend
        public ICommand loadUserContactListIC { get; set; }
        public ICommand selectUserContactIC { get; set; }
        public ICommand sendMessageIC { get; set; }
        public ICommand loadScrollChatIC { get; set; }
        #endregion

        public ChatViewModel()
        {
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();

            loadUserContactListIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                loadUserContactList();
            });

            selectUserContactIC = new RelayCommand<UserContactDTO>((p) => { return true; }, (p) =>
            {
                UserID = p.MaKhachHang;
                selectUserContact(p.MaKhachHang);
            });

            sendMessageIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                sendMessage();
            });

            loadScrollChatIC = new RelayCommand<ScrollViewer>((p) => { return true; }, (p) =>
            {
                scrollChat = p;
            });
        }

        /// <summary>
        /// Load dữ liệu liên hệ người dùng
        /// </summary>
        private async void loadUserContactList()
        {
            (string label, List<UserContactDTO> userContactList) = await ChatAPI.Ins.getUserContacts();
            
            if (userContactList != null)
            {
                UserContactList = new ObservableCollection<UserContactDTO>(userContactList);
            }
            else
            {
                UserContactList = new ObservableCollection<UserContactDTO>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async void selectUserContact(string userID)
        {
            // Load lại tin nhắn của khách hàng
            (string label, List<ChatDTO> chats) = await ChatAPI.Ins.getMessage(userID, null);

            if (chats != null)
            {
                ChatList = new ObservableCollection<ChatDTO>(chats);
                scrollChat.ScrollToBottom();
                timeNow = DateTime.Now;
            }
        }

        /// <summary>
        /// Nhắn tin nhắn
        /// </summary>
        private async void sendMessage()
        {
            if (string.IsNullOrEmpty(MessageText) || string.IsNullOrEmpty(UserID))
                return;

            ChatModel chat = new ChatModel
            {
                MaKH = "",
                NoiDung = MessageText,
                ThoiGian = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")
            };

            (string label, bool isCreate) = await ChatAPI.Ins.AddMessage(chat, UserID);

            if (isCreate)
            {
                // Thêm vào danh sách
                ChatList.Add(new ChatDTO
                {
                    IsReceived = false,
                    NoiDung = chat.NoiDung,
                    ThoiGian = chat.ThoiGian
                });

                ChatList = new ObservableCollection<ChatDTO>(ChatList);

                MessageText = "";
            }
        }

        private async void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (ChatList == null)
                return;

            // Load tin nhắn mới nhất từ khách hàng
            (string label, List<ChatDTO> listChat) = await ChatAPI.Ins.getMessage(UserID, timeNow);

            if (listChat != null && listChat.Count > 0)
            {
                timeNow = DateTime.Now;

                foreach (ChatDTO chat in listChat)
                    ChatList.Add(chat);

                ChatList = new ObservableCollection<ChatDTO>(ChatList);
                scrollChat.ScrollToBottom();
            }

            // Xử lý danh sách contacts
        }
    }
}
