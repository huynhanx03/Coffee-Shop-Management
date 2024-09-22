using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Coffee.Views.MessageBox;
using System.Windows.Media.Imaging;

namespace Coffee.ViewModel.AdminVM.Table
{
    public partial class MainTableViewModel : BaseViewModel
    {
        #region variable
        private string _CurrentPhone;

        public string CurrentPhone
        {
            get { return _CurrentPhone; }
            set { _CurrentPhone = value; }
        }

        private string _DateBill;

        public string DateBill
        {
            get { return _DateBill; }
            set { _DateBill = value; }
        }

        #endregion

        #region ICommend
        public ICommand closeBillWindowIC { get; set; }
        public ICommand printBillIC { get; set; }

        #endregion

        #region function
        private void printBill(Window w)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = this.CreateFlowDocument();
                // Thiết lập kích thước trang in
                doc.PageHeight = 1056; // Chiều cao trang A4
                doc.PageWidth = 816; // Chiều rộng trang A4

                doc.ColumnWidth = printDialog.PrintableAreaWidth;

                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Bill");

                w.Close();

                MessageBoxCF ms = new MessageBoxCF("In hoá đơn thành công", MessageType.Accept, MessageButtons.OK);
                ms.ShowDialog();
            }
        }

        private FlowDocument CreateFlowDocument()
        {
            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.FontFamily = new FontFamily("Arial"); // Thiết lập font chữ toàn bộ tài liệu

            // Tạo lưới để chứa tiêu đề và hình ảnh
            Grid headerGrid = new Grid();

            // Hình ảnh
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Coffee;component/Resources/Images/cup-of-coffee-avatar-removebg-preview.png"));
            image.Width = 100; // Điều chỉnh kích thước hình ảnh nếu cần
            image.Margin = new Thickness(0, 0, 10, 0); // Thêm khoảng cách giữa hình ảnh và tiêu đề
            image.HorizontalAlignment = HorizontalAlignment.Left;

            // Thêm hình ảnh vào cột đầu tiên của lưới
            Grid.SetColumn(image, 0);
            headerGrid.Children.Add(image);

            // Tiêu đề
            TextBlock headerText = new TextBlock(new Run("Hóa đơn ESPRO"))
            {
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetColumn(headerText, 1);
            headerGrid.Children.Add(headerText);

            // Thêm lưới vào một BlockUIContainer và sau đó thêm vào FlowDocument
            BlockUIContainer headerContainer = new BlockUIContainer(headerGrid);
            doc.Blocks.Add(headerContainer);

            // Thông tin hóa đơn
            Paragraph info1 = new Paragraph();
            info1.Inlines.Add(new Bold(new Run("Tên nhân viên: ")));
            info1.Inlines.Add(new Run($"{EmployeeName}\n"));
            info1.TextAlignment = TextAlignment.Left;

            doc.Blocks.Add(info1);

            Paragraph info2 = new Paragraph();
            info2.Inlines.Add(new Bold(new Run("Tên khách hàng: ")));
            info2.Inlines.Add(new Run($"{CustomeName}\n"));
            info2.TextAlignment = TextAlignment.Left;
            info2.Padding = new Thickness(0, 0, 0, 0);
            doc.Blocks.Add(info2);

            Paragraph info3 = new Paragraph();
            info3.Inlines.Add(new Bold(new Run("Ngày hóa đơn: ")));
            info3.Inlines.Add(new Run($"{DateBill:dd/MM/yyyy}\n"));
            info3.TextAlignment = TextAlignment.Left;
            info3.Margin = new Thickness(0, 0, 0, 20);
            doc.Blocks.Add(info3);

            // Bảng chi tiết hóa đơn
            System.Windows.Documents.Table table = new System.Windows.Documents.Table();
            table.CellSpacing = 0;
            table.BorderBrush = Brushes.Black;
            table.BorderThickness = new Thickness(1);

            int numberOfColumns = 5;
            double[] columnWidths = { 3, 1.5, 1.5, 2, 2 }; // Tỷ lệ chiều rộng cột

            for (int i = 0; i < numberOfColumns; i++)
            {
                TableColumn column = new TableColumn();
                column.Width = new GridLength(columnWidths[i], GridUnitType.Star);
                table.Columns.Add(column);
            }

            TableRowGroup tableRowGroup = new TableRowGroup();
            table.RowGroups.Add(tableRowGroup);

            // Hàng tiêu đề của bảng
            TableRow headerRow = new TableRow();
            tableRowGroup.Rows.Add(headerRow);
            string[] headers = { "Tên sản phẩm", "Kích thước", "Số lượng", "Giá tiền", "Thành tiền" };

            foreach (string headerTextX in headers)
            {
                TableCell cell = new TableCell(new Paragraph(new Run(headerTextX)))
                {
                    FontWeight = FontWeights.Bold,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Padding = new Thickness(5)
                };
                headerRow.Cells.Add(cell);
            }

            // Hàng chi tiết của bảng
            foreach (var detailBill in DetailBillList)
            {
                TableRow row = new TableRow();
                tableRowGroup.Rows.Add(row);

                TableCell tenSPHoaDonCell = new TableCell(new Paragraph(new Run(detailBill.TenSanPham)))
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Padding = new Thickness(5)
                };

                TableCell tenKTHoaDonCell = new TableCell(new Paragraph(new Run(detailBill.SelectedProductSize.TenKichThuoc)))
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Padding = new Thickness(5)
                };

                TableCell soLuongCell = new TableCell(new Paragraph(new Run(detailBill.SoLuong.ToString())))
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Padding = new Thickness(5)
                };

                TableCell giaTienCell = new TableCell(new Paragraph(new Run(detailBill.SelectedProductSize.Gia.ToString("N0"))))
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Padding = new Thickness(5)
                };

                TableCell thanhTienCell = new TableCell(new Paragraph(new Run(detailBill.ThanhTien.ToString("N0"))))
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Padding = new Thickness(5)
                };

                row.Cells.Add(tenSPHoaDonCell);
                row.Cells.Add(tenKTHoaDonCell);
                row.Cells.Add(soLuongCell);
                row.Cells.Add(giaTienCell);
                row.Cells.Add(thanhTienCell);
            }

            doc.Blocks.Add(table);

            // Tổng tiền
            Paragraph totalParagraph = new Paragraph(new Run($"Tổng tiền: {TotalBill:N0} VND"))
            {
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };
            doc.Blocks.Add(totalParagraph);

            return doc;
        }
        #endregion
    }
}
