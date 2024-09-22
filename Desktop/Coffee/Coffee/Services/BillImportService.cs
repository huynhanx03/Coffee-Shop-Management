using Coffee.API;
using Coffee.DALs;
using Coffee.DTOs;
using Coffee.Models;
using Coffee.Utils;
using Coffee.Utils.Helper;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Services
{
    public class BillImportService
    {
        private static BillImportService _ins;
        public static BillImportService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new BillImportService();
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
        public async Task<(string, bool)> createBillImport(ImportModel import)
        {
            return await ImportAPI.Ins.createBillImport(import);

            // ImportDTO import, ObservableCollection<DetailImportDTO> detailImportList

            //// Tìm mã phiếu nhập kho
            //string MaPhieuNhapKhoMax = await this.getMaxMaPhieuNhapKho();

            //import.MaPhieuNhapKho = Helper.nextID(MaPhieuNhapKhoMax, "NK");

            //(string labelCreateBillImport, bool isCreateBillImport) = await BillImportDAL.Ins.createBillImport(import);

            //if (isCreateBillImport)
            //{
            //    // Nếu tạo phiếu nhập kho thành công thì tạo các chi tiết nhập kho
            //    foreach (DetailImportDTO detail in detailImportList)
            //        detail.MaPhieuNhapKho = import.MaPhieuNhapKho;

            //    (string labelCreateDetailBillImprot, bool isCreateDetailBillImprot) = await BillImportDAL.Ins.createDetailBillImport(import.MaPhieuNhapKho, detailImportList);

            //    if (isCreateDetailBillImprot)
            //    {
            //        // Thêm số lượng vào nguyên liệu
            //        foreach (DetailImportDTO detailImport in detailImportList)
            //        {
            //            await IngredientService.Ins.updateIngredientQuantity(detailImport.MaNguyenLieu, detailImport.SoLuong, detailImport.MaDonVi);
            //        }

            //        return (labelCreateBillImport, isCreateBillImport);
            //    }
            //    else
            //    {
            //        // Tạo các chi tiết thất bại
            //        // Xoá phiếu nhập kho
            //        await this.DeleteBillImport(import.MaPhieuNhapKho);

            //        return (labelCreateDetailBillImprot, false);
            //    }
            //}
            //else
            //    return (labelCreateBillImport, isCreateBillImport);
        }

        /// <summary>
        ///     Lấy mã phiêu nhập kho lớn nhất
        /// </summary>
        /// <returns>
        ///     Mã phiếu nhập kho lớn nhất
        /// </returns>
        public async Task<string> getMaxMaPhieuNhapKho()
        {
            return await BillImportDAL.Ins.getMaxMaPhieuNhapKho();
        }

        /// <summary>
        /// Danh sách hóa đơn kho
        /// </summary>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, List<ImportDTO>)> getListBillImport()
        {
            return await BillImportDAL.Ins.getListBillImport();
        }

        /// <summary>
        /// Danh sách hóa đơn kho theo thời gian
        /// </summary>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, List<ImportDTO>)> getListBillImporttime(DateTime FromDate,DateTime ToDate)
        {
            return await ImportAPI.Ins.GetImports(FromDate, ToDate);
            //return await BillImportDAL.Ins.getListBillImporttime(FromDate, ToDate);
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
            return await ImportAPI.Ins.deleteBillImport(importID);
            //return await BillImportDAL.Ins.DeleteBillImport(importID);
        }

        /// <summary>
        /// Lấy chi tiết của phiếu nhập kho
        /// </summary>
        /// <param name="importID"></param>
        /// <returns>
        ///     1: Thông báo
        /// </returns>
        public async Task<(string, List<DetailImportDTO>)> getDetailBillImport(string importID)
        {
            return await BillImportDAL.Ins.getDetailBillImport(importID);
        }
    }
}
