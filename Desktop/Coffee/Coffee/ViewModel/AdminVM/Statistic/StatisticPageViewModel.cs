using ChartKit.SkiaSharpView;
using ChartKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChartKit.SkiaSharpView.Painting.Effects;
using ChartKit.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using ChartKit.Defaults;
using System.Globalization;
using Coffee.DTOs;
using System.Windows.Input;
using Coffee.Services;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using System.Windows.Markup;
using ChartKit.SkiaSharpView.WPF;
using Coffee.Views.Admin.StatisticPage;
using LiveCharts.Wpf;
using LiveCharts;

namespace Coffee.ViewModel.AdminVM.Statistic
{


    public partial class StatisticViewModel : BaseViewModel
    {
        #region variable
        private SeriesCollection _profitSeries;
        public SeriesCollection ProfitSeries
        {
            get { return _profitSeries; }
            set
            {
                _profitSeries = value;
                OnPropertyChanged();
            }
        }
        private string[] _labels;
        public string[] Labels
        {
            get { return _labels; }
            set
            {
                _labels = value;
                OnPropertyChanged();
            }
        }

        private Func<int, string> _yFormatter;
        public Func<int, string> YFormatter
        {
            get { return _yFormatter; }
            set
            {
                _yFormatter = value;
                OnPropertyChanged();
            }
        }

        private decimal _sumMoneyIn;

        public decimal SumMoneyIn
        {
            get { return _sumMoneyIn; }
            set { _sumMoneyIn = value; OnPropertyChanged(); }
        }

        private decimal _sumMoneyOut;

        public decimal SumMoneyOut
        {
            get { return _sumMoneyOut; }
            set { _sumMoneyOut = value; OnPropertyChanged(); }
        }

        private decimal _sumProfit;

        public decimal SumProfit
        {
            get { return _sumProfit; }
            set { _sumProfit = value; OnPropertyChanged(); }
        }

        #endregion

        public async Task loadCartesianChar()
        {
            DateTime currentDate = FromDate;
            DateTime UpDate = ToDate.AddDays(1);

            List<DateTime> dates = new List<DateTime>();

            int indexBill = 0;
            int indexImport = 0;

            int sizeBill = BillList.Count();
            int sizeImport = BillImportList.Count();

            List<decimal> profitValues = new List<decimal>();

            decimal sumMoneyIn = 0;
            decimal sumMoneyOut = 0;
            decimal sumProfit = 0;

            while (currentDate <= UpDate)
            {
                decimal profit = 0;

                DateTime billDate = DateTime.Now; 
                    if (indexBill < sizeBill)
                        DateTime.ParseExact(BillList[indexBill].NgayTao, "HH:mm:ss dd/MM/yyyy", null);

                while (indexBill < sizeBill && billDate.Day == currentDate.Day && billDate.Month == currentDate.Month && billDate.Year == currentDate.Year)
                {
                    profit += BillList[indexBill].TongTien;
                    sumMoneyIn += BillList[indexBill].TongTien;

                    indexBill += 1;
                }

                DateTime importDate = DateTime.Now;
                if (indexImport < sizeImport) 
                    importDate = DateTime.ParseExact(BillImportList[indexImport].NgayTaoPhieu, "HH:mm:ss dd/MM/yyyy", null);

                while (indexImport < sizeImport && importDate.Day == currentDate.Day && importDate.Month == currentDate.Month && importDate.Year == currentDate.Year)
                {
                    profit -= BillImportList[indexImport].TongTien;
                    sumMoneyOut += BillImportList[indexImport].TongTien;

                    indexImport += 1;
                }

                profitValues.Add(profit);
                dates.Add(currentDate);
                sumProfit += profit;

                currentDate = currentDate.AddDays(1);
            }

            string[] dateStrings = dates.Select(date => date.ToString("dd/MM/yyyy")).ToArray();
            ProfitSeries = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Lợi nhuận",
                        Values = new ChartValues<decimal>(profitValues),
                    }
                };

            Labels = dateStrings;

            YFormatter = value =>
            {
                return value.ToString("N");

            };

            SumMoneyIn = sumMoneyIn;
            SumMoneyOut = sumMoneyOut;
            SumProfit = sumProfit;
        }
    }
}


