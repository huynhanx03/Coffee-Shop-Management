using Coffee.DTOs;
using Coffee.Models;
using Coffee.Services;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DALs
{
    public class BillImportDAL
    {
        private static BillImportDAL _ins;
        public static BillImportDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new BillImportDAL();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// Tạo phiếu nhập kho mới
        /// </summary>
        /// <param name="import">Phiếu nhập kho</param>
        /// <returns>
        ///     1. Thông báo
        ///     2. True khi tạo thành công
        /// </returns>
        public async Task<(string, bool)> createBillImport(ImportDTO import)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.SetTaskAsync("PhieuNhapKho/" + import.MaPhieuNhapKho, import);

                    return ("Thêm phiếu nhập kho thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }

        /// <summary>
        ///     Lấy mã phiêu nhập kho lớn nhất
        /// </summary>
        /// <returns>
        ///     Mã phiếu nhập kho lớn nhất
        /// </returns>
        public async Task<string> getMaxMaPhieuNhapKho()
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("PhieuNhapKho");

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, ImportDTO> data = response.ResultAs<Dictionary<string, ImportDTO>>();

                        string MaxMaPhieuNhapKho = data.Values.Select(i => i.MaPhieuNhapKho).Max();

                        return MaxMaPhieuNhapKho;
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
        /// Tạo chi tiết phiếu nhập kho mới
        /// </summary>
        /// <param name="importID"> Mã của phiếu nhập kho </param>
        /// <param name="detailImportList"> List chi tiết phiếu nhập kho</param>
        /// <returns>
        ///     1. Thông báo
        ///     2. True khi tạo thành công
        /// </returns>
        ///
        public async Task<(string, bool)> createDetailBillImport(string importID, ObservableCollection<DetailImportDTO> detailImportList)
        {
            try
            {
                using (var context = new Firebase())
                {
                    foreach (var detailImport in detailImportList)
                    {
                        await context.Client.SetTaskAsync("PhieuNhapKho/" + importID + "/ChiTietPhieuNhapKho/" + detailImport.MaNguyenLieu, detailImport);
                    }

                    return ("Thêm phiếu nhập kho thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Danh sách hóa đơn nhập kho
        /// </returns>
        public async Task<(string, List<ImportDTO>)> getListBillImport()
        {
            try
            {
                using (var context = new Firebase())
                {
                    // Lấy dữ liệu từ nút "Imports" trong Firebase
                    FirebaseResponse billimportResponse = await context.Client.GetTaskAsync("PhieuNhapKho");
                    Dictionary<string, ImportDTO> billimportData = billimportResponse.ResultAs<Dictionary<string, ImportDTO>>();

                    // Lấy dữ liệu từ nút "Employees" trong Firebase
                    FirebaseResponse employeeResponse = await context.Client.GetTaskAsync("NhanVien");
                    Dictionary<string, EmployeeDTO> employeeData = employeeResponse.ResultAs<Dictionary<string, EmployeeDTO>>();


                    var result = (from billimport in billimportData.Values
                                  join employee in employeeData.Values on billimport.MaNhanVien equals employee.MaNhanVien
                                  select new ImportDTO
                                  {
                                      MaNhanVien = billimport.MaNhanVien,
                                      MaPhieuNhapKho = billimport.MaPhieuNhapKho,
                                      NgayTaoPhieu = billimport.NgayTaoPhieu,
                                      TongTien = billimport.TongTien,
                                  }).ToList();

                    return ("Lấy danh sách hóa đơn nhập kho thành công", result);
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
        /// <returns>
        ///     Danh sách hóa đơn nhập kho theo thời gian
        /// </returns>
        public async Task<(string, List<ImportDTO>)> getListBillImporttime(DateTime fromdate, DateTime todate)
        {
            try
            {
                using (var context = new Firebase())
                {
                    // Lấy dữ liệu từ nút "PhieuNhapKho" trong Firebase
                    FirebaseResponse billimportResponse = await context.Client.GetTaskAsync("PhieuNhapKho");
                    Dictionary<string, ImportDTO> billimportData = billimportResponse.ResultAs<Dictionary<string, ImportDTO>>();

                    // Lấy dữ liệu từ nút "NguoiDung" trong Firebase
                    FirebaseResponse userResponse = await context.Client.GetTaskAsync("NguoiDung");
                    Dictionary<string, UserDTO> userData = userResponse.ResultAs<Dictionary<string, UserDTO>>();

                    var result = new List<ImportDTO>();

                    foreach (var billimport in billimportData.Values)
                    {
                        if (DateTime.ParseExact(billimport.NgayTaoPhieu, "HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture).Date >= fromdate &&
                            DateTime.ParseExact(billimport.NgayTaoPhieu, "HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture).Date <= todate)
                        {
                            var user = userData.Values.FirstOrDefault(u => u.MaNguoiDung == billimport.MaNhanVien);
                            var importDto = new ImportDTO
                            {
                                MaNhanVien = billimport.MaNhanVien,
                                MaPhieuNhapKho = billimport.MaPhieuNhapKho,
                                NgayTaoPhieu = billimport.NgayTaoPhieu,
                                TongTien = billimport.TongTien,
                                TenNhanVien = user != null ? user.HoTen : null
                            };
                            result.Add(importDto);
                        }
                    }

                    return ("Lấy danh sách hóa đơn nhập kho thành công", result);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Xoá phiếu nhập kho
        /// </summary>
        /// <param name="importID"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> DeleteBillImport(string importID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.DeleteTaskAsync("PhieuNhapKho/" + importID);
                    return ("Xoá phiếu nhập kho thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }

        /// <summary>
        /// Lấy danh sách chi tiết hoá đơn nhập kho
        /// </summary>
        /// <param name="billID"></param>
        /// <returns></returns>
        public async Task<(string, List<DetailImportDTO>)> getDetailBillImport(string billimportID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse billResponse = await context.Client.GetTaskAsync("PhieuNhapKho/" + billimportID + "/ChiTietPhieuNhapKho");
                    Dictionary<string, DetailImportDTO> billimportData = billResponse.ResultAs<Dictionary<string, DetailImportDTO>>();
                    List<DetailImportDTO> detailBillImportList = billimportData.Values.ToList();

                    if (detailBillImportList != null)
                    {
                        return ("Lấy danh sách chi tiết thành công", detailBillImportList);
                    }
                    else
                    {
                        return ("Lấy danh sách chi tiết thất bại", null);
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
