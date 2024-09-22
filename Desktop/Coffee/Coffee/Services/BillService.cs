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
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.Services
{
    public class BillService
    {
        private static BillService _ins;
        public static BillService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new BillService();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// Tạo hoá đơn mới
        /// </summary>
        /// <param name="bill">hoá đơn</param>
        /// <returns>
        ///     1. Thông báo
        ///     2. True khi tạo thành công
        /// </returns>
        public async Task<(string, bool)> createBill(BillModel bill)
        {
            return await BillAPI.Ins.CreateBillSell(bill);

            // BillModel bill, ObservableCollection<DetailBillDTO> detailBillList
            // Tìm mã hoá đơn
            //string MaHoaDonMax = await this.getMaxMaHoaDon();
            //bill.MaHoaDon = Helper.nextID(MaHoaDonMax, "HD");

            //(string labelCreateBill, bool isCreateBill) = await BillDAL.Ins.createBill(bill);

            //if (isCreateBill)
            //{
            //    // Nếu tạo hoá đơn thành công thì tạo các chi tiết hoá đơn
            //    List<DetailBillModel> listDetailBill = new List<DetailBillModel>();

            //    foreach (DetailBillDTO detail in detailBillList)
            //    {
            //        listDetailBill.Add(new DetailBillModel
            //        {
            //            MaHoaDon = bill.MaHoaDon,
            //            MaSanPham = detail.MaSanPham,
            //            MaKichThuoc = detail.SelectedProductSize.MaKichThuoc,
            //            SoLuong = detail.SoLuong,
            //            ThanhTien = detail.ThanhTien
            //        });
            //    }

            //    (string labelCreateDetailBillImprot, bool isCreateDetailBillImprot) = await this.createDetailBill(bill.MaHoaDon, listDetailBill);

            //    if (isCreateDetailBillImprot)
            //    {
            //        return (labelCreateBill, isCreateBill);
            //    }
            //    else
            //    {
            //        // Tạo các chi tiết thất bại
            //        // Xoá hoá đơn
            //        await this.DeleteBill(bill.MaHoaDon);

            //        return (labelCreateDetailBillImprot, false);
            //    }
            //}
            //else
            //    return (labelCreateBill, isCreateBill);
        }

        /// <summary>
        /// Tạo hoá đơn mới
        /// </summary>
        /// <param name="bill">hoá đơn</param>
        /// <returns>
        ///     1. Thông báo
        ///     2. True khi tạo thành công
        /// </returns>
        public async Task<(string, BillModel)> createBill(BillModel bill, List<DetailBillModel> detailBillList)
        {
            // Tìm mã hoá đơn
            string MaHoaDonMax = await this.getMaxMaHoaDon();
            bill.MaHoaDon = Helper.nextID(MaHoaDonMax, "HD");

            (string labelCreateBill, bool isCreateBill) = await BillDAL.Ins.createBill(bill);

            if (isCreateBill)
            {
                foreach (DetailBillModel detail in detailBillList)
                {
                    detail.MaHoaDon = bill.MaHoaDon;
                }

                (string labelCreateDetailBillImprot, bool isCreateDetailBillImprot) = await this.createDetailBill(bill.MaHoaDon, detailBillList);

                if (isCreateDetailBillImprot)
                {
                    return (labelCreateBill, bill);
                }
                else
                {
                    // Tạo các chi tiết thất bại
                    // Xoá hoá đơn
                    await this.DeleteBill(bill.MaHoaDon);

                    return (labelCreateDetailBillImprot, bill);
                }
            }
            else
                return (labelCreateBill, bill);
        }

        /// <summary>
        ///     Lấy mã hoá đơn lớn nhất
        /// </summary>
        /// <returns>
        ///     Mã hoá đơn lớn nhất
        /// </returns>
        public async Task<string> getMaxMaHoaDon()
        {
            return await BillDAL.Ins.getMaxMaHoaDon();
        }

        /// <summary>
        /// Tạo chi tiết hoá đơn mới
        /// </summary>
        /// <param name="BillID"> Mã của hoá đơn </param>
        /// <param name="detailList"> List chi tiết hoá đơn</param>
        /// <returns>
        ///     1. Thông báo
        ///     2. True khi tạo thành công
        /// </returns>
        ///
        public async Task<(string, bool)> createDetailBill(string BillID, List<DetailBillModel> detailList)
        {
            return await BillDAL.Ins.createDetailBill(BillID, detailList);
        }

        /// <summary>
        /// Xoá hoá đơn
        /// </summary>
        /// <param name="BillID"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> DeleteBill(string BillID)
        {
            return await BillAPI.Ins.DeleteBillSell(BillID);
        }

        /// <summary>
        /// Danh sách hóa đơn
        /// </summary>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, List<BillDTO>)> getListBill()
        {
            return await BillDAL.Ins.getListBill();
        }

        /// <summary>
        /// Danh sách hóa đơn theo thời gian
        /// </summary>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, List<BillDTO>)> getListBilltime(DateTime fromdate,DateTime todate)
        {
            return await BillAPI.Ins.GetBillSells(fromdate, todate);
            //return await BillDAL.Ins.getListBilltime(fromdate,todate);
        }


        /// <summary>
        /// Danh sách chi tiết hóa đơn
        /// </summary>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, List<DetailBillModel>)> getDetailBill(string MaHoaDon)
        {
            return await BillDAL.Ins.getDetailBill(MaHoaDon);
        }

        /// <summary>
        /// Tìm kiếm hoá đơn của bàn đặt chỗ (chưa thanh toán)
        /// </summary>
        /// <param name="tableID">
        /// 
        /// </param>
        /// <returns></returns>
        public async Task<(string, BillDTO)> findBillByTableBooking(string tableID)
        {
            return await BillAPI.Ins.getBillSellUnpaid(tableID, Constants.StatusBill.UNPAID);

            //(string label, BillModel bill) = await BillDAL.Ins.findBillByTableBooking(tableID);

            //if (bill != null)
            //{
            //(string labelDetailBIll, List<DetailBillDTO> detailBillDTOList) = await BillDAL.Ins.getDetailBillById(bill.MaHoaDon);

            //    if (detailBillDTOList != null)
            //        return ("Tìm thành công", bill, detailBillDTOList);
            //    else
            //        return (labelDetailBIll, null, null);
            //}
            //else
            //    return (label, null, null);
        }

        /// <summary>
        /// Cập nhật hoá đơn
        /// </summary>
        /// <param name="bill"> hoá đơn </param>
        /// <param name="detailBillList"> danh sách chi tiết hoá đơn </param>
        /// <returns>
        ///     1. Thông báo
        ///     2. Hoá đơn
        /// </returns>
        public async Task<(string, BillModel)> updateBill(BillModel bill)
        {
            return await BillAPI.Ins.UpdateBillSell(bill);

            //List<DetailBillModel> listDetailBill = new List<DetailBillModel>();

            //foreach (DetailBillDTO detail in detailBillList)
            //{
            //    listDetailBill.Add(new DetailBillModel
            //    {
            //        MaHoaDon = bill.MaHoaDon,
            //        MaSanPham = detail.MaSanPham,
            //        MaKichThuoc = detail.SelectedProductSize.MaKichThuoc,
            //        SoLuong = detail.SoLuong,
            //        ThanhTien = detail.ThanhTien
            //    });
            //}

            //(string label, bool isCreate) = await BillDAL.Ins.updateDetailBill(bill.MaHoaDon, listDetailBill);

            //return await BillDAL.Ins.updateBill(bill);
        }

        /// <summary>
        /// Cập nhật hoá đơn
        /// </summary>
        /// <param name="bill"> hoá đơn </param>
        /// <param name="detailBillList"> danh sách chi tiết hoá đơn </param>
        /// <returns>
        ///     1. Thông báo
        ///     2. Hoá đơn
        /// </returns>
        public async Task<(string, BillModel)> updateBill(BillModel bill, List<DetailBillModel> detailBillList)
        {
            (string label, bool isCreate) = await BillDAL.Ins.updateDetailBill(bill.MaHoaDon, detailBillList);

            return await BillDAL.Ins.updateBill(bill);
        }

        /// <summary>
        /// Cập nhật hoá đơn theo bàn
        /// </summary>
        /// <param name="tableID"></param>
        /// <param name="tableIDNew"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> updateBillByTableID(string tableID, string tableIDNew)
        {
            return await BillAPI.Ins.updateTableBooking(tableID, tableIDNew);

            //(string labelFindBill, BillModel bill) = await BillDAL.Ins.findBillByTableBooking(tableID);

            //if (bill != null)
            //{
            //    (string labelUpdate, bool isUpdate) = await BillDAL.Ins.updateTableIDInBill(bill.MaHoaDon, tableIDNew);
            
            //    if (isUpdate)
            //        return (labelFindBill, true);
            //    else
            //        return (labelUpdate, false);
            //}
            //else
            //    return (labelFindBill, false);
        }

        /// <summary>
        /// Danh sách các món ăn bán chạy
        /// </summary>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, List<ProductDTO>)> GetMostSoldFoods()
        {
            return await BillDAL.Ins.GetMostSoldFoods();
        }

        /// <summary>
        /// Gộp hoá đơn theo bàn
        /// </summary>
        /// <param name="tableID1"></param>
        /// <param name="tableID2"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> mergeBillByTableID(string tableID1, string tableID2)
        {
            return await BillAPI.Ins.mergeBillOfTables(tableID1, tableID2);

            (string labelFindBill1, BillModel bill1) = await BillDAL.Ins.findBillByTableBooking(tableID1);
            (string labelFindBill2, BillModel bill2) = await BillDAL.Ins.findBillByTableBooking(tableID2);

            if (bill1 != null && bill2 != null)
            {
                bill2.TongTien += bill1.TongTien;

                (string labelGet1, List<DetailBillModel> detailBillList1) = await BillDAL.Ins.getDetailBill(bill1.MaHoaDon);
                (string labelGet2, List<DetailBillModel> detailBillList2) = await BillDAL.Ins.getDetailBill(bill2.MaHoaDon);

                List<DetailBillModel> list = new List<DetailBillModel>();

                foreach (DetailBillModel detailBill1 in detailBillList1)
                {
                    bool isFind = false;

                    foreach (DetailBillModel detailBill2 in detailBillList2)
                    {
                        if (detailBill2.MaSanPham == detailBill1.MaSanPham && detailBill2.MaKichThuoc == detailBill1.MaKichThuoc)
                        {
                            detailBill2.SoLuong += detailBill1.SoLuong;
                            detailBill2.ThanhTien += detailBill1.ThanhTien;
                            
                            isFind = true;

                            break;
                        }
                    }

                    if (isFind == false) list.Add(detailBill1);
                }

                detailBillList2.AddRange(list);

                (string labelUpdate, BillModel bill) = await this.updateBill(bill2, detailBillList2);

                if (bill != null)
                {
                    // Xoá bill1
                    (string labelDelete, bool isDelete) = await this.DeleteBill(bill1.MaHoaDon);

                    return ("Gộp bàn thành công", true);
                }
                else
                    return (labelUpdate, false);
            }
            else
                if (bill1 != null)
                    return (labelFindBill2, false);
                else
                    return (labelFindBill1, false);
        }
    }
}
