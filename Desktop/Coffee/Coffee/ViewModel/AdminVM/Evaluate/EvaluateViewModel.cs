using Coffee.API;
using Coffee.DTOs;
using Coffee.Models;
using Coffee.Services;
using Coffee.Views.Admin.EvaluatePage;
using Coffee.Views.MessageBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coffee.ViewModel.AdminVM.Evaluate
{
    public partial class EvaluateViewModel: BaseViewModel
    {
        #region variable
        public Grid MaskName { get; set; }

        private ObservableCollection<EvaluateDTO> _EvaluateList;

        public ObservableCollection<EvaluateDTO> EvaluateList
        {
            get { return _EvaluateList; }
            set { _EvaluateList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<RankModel> _RankList;

        public ObservableCollection<RankModel> RankList
        {
            get { return _RankList; }
            set { _RankList = value; OnPropertyChanged(); }
        }

        private RankModel _selectedRank;
        public RankModel selectedRank
        {
            get { return _selectedRank; }
            set
            {
                _selectedRank = value;
                OnPropertyChanged();
            }
        }

        private List<EvaluateDTO> __EvaluateList;
        private List<EvaluateDTO> evaluateSearchList;
        private List<EvaluateDTO> evaluateRankList;

        private bool _IsLoading;

        public bool IsLoading
        {
            get { return _IsLoading; }
            set { _IsLoading = value; OnPropertyChanged(); }
        }

        #endregion

        #region ICommand
        public ICommand loadShadowMaskIC { get; set; }
        public ICommand loadEvaluateListIC { get; set; }
        public ICommand searchEvaluateIC { get; set; }
        public ICommand deleteEvaluateIC { get; set; }
        public ICommand selectedEvaluateRankIC { get; set; }
        public ICommand loadRankListIC { get; set; }
        #endregion

        public EvaluateViewModel()
        {
            loadShadowMaskIC = new RelayCommand<Grid>((p) => { return true; }, (p) =>
            {
                MaskName = p;
            });

            loadEvaluateListIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                loadEvaluateList();
            });

            loadRankListIC = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                loadRankList();
            });

            deleteEvaluateIC = new RelayCommand<EvaluateDTO>((p) => { return true; }, (p) =>
            {
                deleteEvaluate(p);
            });

            searchEvaluateIC = new RelayCommand<TextBox>(null, (p) =>
            {
                searchEvaluate(p.Text);
            });

            selectedEvaluateRankIC = new RelayCommand<TextBox>(null, (p) =>
            {
                selectedEvaluateRank(selectedRank);
            });
        }

        #region function
        /// <summary>
        /// Load danh sách đánh giá
        /// </summary>
        private async void loadEvaluateList()
        {
            MaskName.Visibility = Visibility.Visible;
            IsLoading = true;

            (string label, List<EvaluateDTO> Evaluates) = await EvaluateService.Ins.getListEvaluate();

            if (Evaluates != null)
            {
                EvaluateList = new ObservableCollection<EvaluateDTO>(Evaluates);
                __EvaluateList = new List<EvaluateDTO>(Evaluates);
                evaluateSearchList = new List<EvaluateDTO>(Evaluates);
                evaluateRankList = new List<EvaluateDTO>(Evaluates);
            }
            else
            {
                EvaluateList = new ObservableCollection<EvaluateDTO>();
                __EvaluateList = new List<EvaluateDTO>();
                evaluateSearchList = new List<EvaluateDTO>();
                evaluateRankList = new List<EvaluateDTO>();
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        /// <summary>
        /// Xoá đánh giá
        /// </summary>
        public async void deleteEvaluate(EvaluateDTO evaluate)
        {
            MaskName.Visibility = Visibility.Visible;

            MessageBoxCF ms = new MessageBoxCF("Xác nhận xoá đánh giá?", MessageType.Waitting, MessageButtons.YesNo);

            if (ms.ShowDialog() == true)
            {
                IsLoading = true;

                (string label, bool isDeleteEvaluate) = await EvaluateService.Ins.DeleteEvaluate(evaluate);

                if (isDeleteEvaluate)
                {
                    MessageBoxCF msn = new MessageBoxCF(label, MessageType.Accept, MessageButtons.OK);
                    loadEvaluateList();
                    msn.ShowDialog();
                }
                else
                {
                    MessageBoxCF msn = new MessageBoxCF(label, MessageType.Error, MessageButtons.OK);
                    msn.ShowDialog();
                }
            }

            MaskName.Visibility = Visibility.Collapsed;
            IsLoading = false;
        }

        private void searchEvaluate(string text)
        {
            if (text != null)
            {
                if (__EvaluateList != null)
                    evaluateSearchList = new List<EvaluateDTO>(__EvaluateList.FindAll(x => x.TenKhachHang.ToLower().Contains(text.ToLower())));
            }

            EvaluateList = new ObservableCollection<EvaluateDTO>(evaluateSearchList.Intersect(evaluateRankList));
        }

        private void selectedEvaluateRank(RankModel rank)
        {
            if (rank != null)
            {
                if (rank.MaMucDoThanThiet == "TT0000")
                    evaluateRankList = new List<EvaluateDTO>(__EvaluateList);
                else
                    evaluateRankList = new List<EvaluateDTO>(__EvaluateList.FindAll(p => p.MaHang == rank.MaMucDoThanThiet));
            }

            EvaluateList = new ObservableCollection<EvaluateDTO>(evaluateSearchList.Intersect(evaluateRankList));
        }

        private async void loadRankList()
        {
            (string label, List<RankModel> listRank) = await CustomerAPI.Ins.getRankCustomers();

            if (listRank != null)
            {
                RankList = new ObservableCollection<RankModel>(listRank);
            }
            else
                RankList = new ObservableCollection<RankModel>();

            RankList.Insert(0, new RankModel
            {
                MaMucDoThanThiet = "TT0000",
                TenMucDoThanThiet = "Toàn bộ",
            });
        }

        #endregion
    }
}
