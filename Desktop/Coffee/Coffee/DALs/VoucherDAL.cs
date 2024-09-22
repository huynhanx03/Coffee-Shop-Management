using Coffee.DTOs;
using Coffee.Models;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DALs
{
    public class VoucherDAL
    {
        private static VoucherDAL _ins;
        public static VoucherDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new VoucherDAL();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        ///     Trả về danh sách phiếu giảm giá
        /// </returns>
        public async Task<(string, List<VoucherDTO>)> getAllVoucher()
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse vouhcerResponse = await context.Client.GetTaskAsync("PhieuGiamGia");
                    Dictionary<string, VoucherDTO> voucherData = vouhcerResponse.ResultAs<Dictionary<string, VoucherDTO>>();

                    FirebaseResponse rankResponse = await context.Client.GetTaskAsync("MucDoThanThiet");
                    Dictionary<string, RankModel> rankData = rankResponse.ResultAs<Dictionary<string, RankModel>>();

                    var result = (from voucher in voucherData.Values
                                  join rank in rankData.Values
                                  on voucher.HangToiThieu equals rank.MaMucDoThanThiet
                                  select new VoucherDTO
                                  {
                                      HangToiThieu = voucher.HangToiThieu,
                                      MaPhieuGiamGia = voucher.MaPhieuGiamGia,
                                      NgayHetHan = voucher.NgayHetHan,
                                      NgayPhatHanh = voucher.NgayPhatHanh,
                                      NoiDung = voucher.NoiDung,
                                      PhanTramGiam = voucher.PhanTramGiam,
                                      TenHangToiThieu = rank.TenMucDoThanThiet
                                  }).ToList();

                    return ("Lấy danh sách phiếu giảm giá thành công thành công", result);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Thêm phiếu giảm giá
        /// </summary>
        /// <param name="voucher"></param>
        /// <returns></returns>
        public async Task<(string, VoucherDTO)> createVoucher(VoucherDTO voucher)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.SetTaskAsync("PhieuGiamGia/" + voucher.MaPhieuGiamGia, voucher);

                    return ("Thêm phiếu giảm giá thành công", voucher);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        public async Task<(string, bool)> createDetailVoucher(string voucherID, List<string> customerIDList)
        {
            try
            {
                using (var context = new Firebase())
                {
                    DetailVoucherModel detailVoucher = new DetailVoucherModel
                    {
                        MaPhieuGiamGia = voucherID,
                        TrangThai = "Chưa sử dụng"
                    };

                    foreach (var customerID in customerIDList)
                    {
                        detailVoucher.MaKhachHang = customerID;
                        await context.Client.SetTaskAsync("PhieuGiamGia/" + voucherID + "/ChiTiet/" + customerID, detailVoucher);
                    }

                    return ("Thêm phiếu giảm giá thành công", true);
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
        ///     Mã phiếu giảm giá lớn nhất
        /// </returns>
        public async Task<string> getMaxVoucherID()
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("PhieuGiamGia");

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, VoucherDTO> data = response.ResultAs<Dictionary<string, VoucherDTO>>();

                        string MaxVoucherID = data.Values.Select(p => p.MaPhieuGiamGia).Max();

                        return MaxVoucherID;
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
        /// Xoá phiếu giảm giá
        /// </summary>
        /// <param name="VoucherID"> mã phiếu giảm giá </param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> DeleteVoucher(string VoucherID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.DeleteTaskAsync("PhieuGiamGia/" + VoucherID);
                    return ("Xoá phiếu giảm giá thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }
    }
}
