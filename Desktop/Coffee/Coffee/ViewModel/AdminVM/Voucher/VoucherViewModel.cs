using Coffee.DTOs;
using Coffee.Models;
using Coffee.Services;
using Coffee.Views.MessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Voucher
{
    public partial class VoucherViewModel: BaseViewModel, IConstraintViewModel
    {
        #region variable

        public Grid MaskName { get; set; }

        private ObservableCollection<VoucherDTO> _VoucherList;

        public ObservableCollection<VoucherDTO> VoucherList
        {
            get { return _VoucherList; }
            set { _VoucherList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<RankModel> _RankList;

        public ObservableCollection<RankModel> RankList
        {
            get { return _RankList; }
            set { _RankList = value; OnPropertyChanged(); }
        }

        private List<VoucherDTO> __VoucherList;

        private string _Content;

        public string Content
        {
            get { return _Content; }
            set { _Content = value; OnPropertyChanged(); }
        }

        private int _PercentDiscount = 1;

        public int PercentDiscount
        {
            get { return _PercentDiscount; }
            set 
            {
                if (value >= 100) 
                    value = 99;

                if (value <= 0)
                    value = 1;

                _PercentDiscount = value; 
                OnPropertyChanged(); 
            }
        }

        private RankModel _Rank;
        public RankModel Rank
        {
            get { return _Rank; }
            set { _Rank = value; OnPropertyChanged(); }
        }

        private DateTime _ExpirationDate;
        public DateTime ExpirationDate
        {
            get { return _ExpirationDate; }
            set 
            {
                if (value < DateTime.Now)
                    value = DateTime.Now.AddDays(1);

                _ExpirationDate = value; 
                OnPropertyChanged(); 
            }
        }

        private bool _IsLoading;

        public bool IsLoading
        {
            get { return _IsLoading; }
            set { _IsLoading = value; OnPropertyChanged(); }
        }

        #endregion

        #region ICommand
        public ICommand loadShadowMaskIC { get; set; }
        public ICommand loadVoucherListIC { get; set; }
        public ICommand searchVoucherIC { get; set; }
        public ICommand deleteVoucherIC { get; set; }
        public ICommand confirmCreateVoucherIC { get; set; }
        public ICommand loadRankIC { get; set; }
        #endregion

        public VoucherViewModel()
        {
            ExpirationDate = DateTime.Now.AddDays(1);

            loadShadowMaskIC = new RelayCommand<Grid>((p) => { return true; }, (p) =>
            {
                MaskName = p;
            });

            searchVoucherIC = new RelayCommand<TextBox>(null, (p) =>
            {
                if (p.Text != null)
                {
                    if (__VoucherList != null)
                        VoucherList = new ObservableCollection<VoucherDTO>(__VoucherList.FindAll(x => x.NoiDung.ToLower().Contains(p.Text.ToLower())));
                }
            });

            loadVoucherListIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                loadVoucherList();
            });

            loadRankIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                loadRank();
            });

            confirmCreateVoucherIC = new RelayCommand<object>((p) =>
            {
                return !(string.IsNullOrEmpty(Content)); 
            }, (p) =>
            {
                confirmCreateVoucher();
            });

            deleteVoucherIC = new RelayCommand<VoucherDTO>((p) => { return true; }, (p) =>
            {
                deleteVoucher(p);
            });
        }

        #region function
        /// <summary>
        /// load danh sách phiếu giảm giá
        /// </summary>
        private async void loadVoucherList()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            (string label, List<VoucherDTO> vouchers) = await VoucherService.Ins.getAllVoucher();

            if (vouchers != null)
            {
                VoucherList = new ObservableCollection<VoucherDTO>(vouchers);
                __VoucherList = new List<VoucherDTO>(vouchers);
            }
            else
            {
                VoucherList = new ObservableCollection<VoucherDTO>();
                __VoucherList = new List<VoucherDTO>();
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        /// <summary>
        /// load danh sách mức độ thân thiết
        /// </summary>
        private async void loadRank()
        {
            (string label, List<RankModel> ranks) = await RankService.Ins.getAllRank();

            if (ranks != null)
            {
                RankList = new ObservableCollection<RankModel>(ranks);
            }
        }

        /// <summary>
        ///  Xác nhận tạo phiếu giảm giá
        /// </summary>
        private async void confirmCreateVoucher()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            VoucherDTO voucher = new VoucherDTO
            {
                HangToiThieu = Rank.MaMucDoThanThiet,
                NgayHetHan = ExpirationDate.ToString("dd/MM/yyyy"),
                NgayPhatHanh = DateTime.Now.ToString("dd/MM/yyyy"),
                NoiDung = Content,
                PhanTramGiam = PercentDiscount
            };

            (string label, VoucherDTO voucherCreate) = await VoucherService.Ins.createVoucher(voucher);
            
            IsLoading = false;

            if (voucherCreate != null)
            {
                // Tạo các phiếu cho khách hàng

                MessageBoxCF ms = new MessageBoxCF("Phát hành phiếu giảm giá thành công", MessageType.Accept, MessageButtons.OK);
                ms.ShowDialog();
                loadVoucherList();
            }
            else
            {
                MessageBoxCF ms = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                ms.ShowDialog();
            }

            MaskName.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Kiểm tra chỉ được nhập số
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void deleteVoucher(VoucherDTO voucher)
        {
            MaskName.Visibility = Visibility.Visible;

            MessageBoxCF ms = new MessageBoxCF("Xác nhận xoá phiếu giảm giá", MessageType.Waitting, MessageButtons.YesNo);

            if (ms.ShowDialog() == true)
            {
                IsLoading = true;

                (string label, bool isDelete) = await VoucherService.Ins.DeleteVoucher(voucher.MaPhieuGiamGia);
                
                IsLoading = false;

                if (isDelete)
                {
                    loadVoucherList();
                    MessageBoxCF msDelete = new MessageBoxCF(label, MessageType.Accept, MessageButtons.OK);
                    msDelete.ShowDialog();
                }
                else
                {
                    MessageBoxCF msDelete = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                    msDelete.ShowDialog();
                }
            }

            MaskName.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}
