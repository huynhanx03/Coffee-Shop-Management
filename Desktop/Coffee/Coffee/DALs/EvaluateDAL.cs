using Coffee.DTOs;
using Coffee.Models;
using Coffee.Services;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DALs
{
    public class EvaluateDAL
    {
        private static EvaluateDAL _ins;
        public static EvaluateDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new EvaluateDAL();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// Xoá đánh giá
        /// </summary>
        /// <param name="EvaluateID"></param>
        /// <returns></returns>
        public async Task<(string, bool)> DeleteEvaluate(string EvaluateID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.DeleteTaskAsync("DanhGia/" + EvaluateID);
                    return ("Xoá đánh giá thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }

        /// <summary>
        /// Lấy danh sách đánh giá
        /// </summary>
        /// <returns>
        /// </returns>
        public async Task<(string, List<EvaluateDTO>)> getListEvaluate()
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse evaluateResponse = await context.Client.GetTaskAsync("DanhGia");
                    Dictionary<string, EvaluateDTO> evaluateData = evaluateResponse.ResultAs<Dictionary<string, EvaluateDTO>>();

                    FirebaseResponse userResponse = await context.Client.GetTaskAsync("NguoiDung");
                    Dictionary<string, UserDTO> userData = userResponse.ResultAs<Dictionary<string, UserDTO>>();

                    FirebaseResponse productResponse = await context.Client.GetTaskAsync("SanPham");
                    Dictionary<string, ProductDTO> productData = productResponse.ResultAs<Dictionary<string, ProductDTO>>();

                    var result = (from evaluate in evaluateData.Values
                                  join customer in userData.Values on evaluate.MaNguoiDung equals customer.MaNguoiDung
                                  join product in productData.Values on evaluate.MaSanPham equals product.MaSanPham
                                  select new EvaluateDTO
                                  {
                                      MaDanhGia = evaluate.MaDanhGia,
                                      MaNguoiDung = evaluate.MaNguoiDung,
                                      TenKhachHang = customer.HoTen,
                                      TenSanPham = product.TenSanPham,
                                      MaSanPham = evaluate.MaSanPham,
                                      ThoiGianDanhGia = evaluate.ThoiGianDanhGia,
                                      VanBanDanhGia = evaluate.VanBanDanhGia,
                                      DiemDanhGia = evaluate.DiemDanhGia,
                                  }).ToList();

                    foreach (var item in result)
                    {
                        (string _, string RankID) = await CustomerService.Ins.getRankCustomer(item.MaNguoiDung);

                        (string _, RankModel rank) = await RankService.Ins.getRank(RankID);
                        if (rank != null)
                        {
                            item.MaHang = rank.MaMucDoThanThiet;
                            item.TenHang = rank.TenMucDoThanThiet;
                        }
                    }

                    return ("Lấy danh sách đánh giá thành công", result);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }
    }
}
