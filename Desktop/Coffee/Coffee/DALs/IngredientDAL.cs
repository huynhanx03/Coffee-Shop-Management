using Coffee.DTOs;
using Coffee.Models;
using Coffee.Utils;
using Coffee.Utils.Helper;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DALs
{
    public class IngredientDAL
    {
        private static IngredientDAL _ins;
        public static IngredientDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new IngredientDAL();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// Thêm nguyên liệu
        /// </summary>
        /// <param name="ingredient"> Nguyên liệu </param>
        /// <returns>
        ///     1: Thông báo
        ///     2: Nguyên liệu
        /// </returns>
        public async Task<(string, IngredientDTO)> createIngredient(IngredientDTO ingredient)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.SetTaskAsync("NguyenLieu/" + ingredient.MaNguyenLieu, ingredient);

                    return ("Thêm nguyên liệu thành công", ingredient);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Cập nhật nguyên liệu
        /// </summary>
        /// <param name="ingredient"> Nguyên liệu </param>
        /// <returns>
        ///     1: Thông báo
        ///     2: Nguyên liệu
        /// </returns>
        public async Task<(string, IngredientDTO)> updateIngredient(IngredientDTO ingredient)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.UpdateTaskAsync("NguyenLieu/" + ingredient.MaNguyenLieu, ingredient);

                    return ("Cập nhật nguyên liệu thành công", ingredient);
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
        ///     Mã nguyên liệu lớn nhất
        /// </returns>
        public async Task<string> getMaxMaNguyenLieu()
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("NguyenLieu");

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, IngredientDTO> data = response.ResultAs<Dictionary<string, IngredientDTO>>();

                        string MaxMaNguyenLieu = data.Values.Select(p => p.MaNguyenLieu).Max();

                        return MaxMaNguyenLieu;
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
        /// 
        /// </summary>
        /// <returns>
        ///     Danh sách nguyên liệu
        /// </returns>
        public async Task<(string, List<IngredientDTO>)> getListIngredient()
        {
            try
            {
                using (var context = new Firebase())
                {
                    // Lấy dữ liệu từ nút "NguyenLieu" trong Firebase
                    FirebaseResponse ingredientResponse = await context.Client.GetTaskAsync("NguyenLieu");
                    Dictionary<string, IngredientDTO> ingredientData = ingredientResponse.ResultAs<Dictionary<string, IngredientDTO>>();

                    // Lấy dữ liệu từ nút "DonVi" trong Firebase
                    FirebaseResponse unitResponse = await context.Client.GetTaskAsync("DonVi");
                    Dictionary<string, UnitDTO> unitData = unitResponse.ResultAs<Dictionary<string, UnitDTO>>();

                    var result = (from ingredient in ingredientData.Values
                                  join unit in unitData.Values
                                  on Helper.converntUnit(ingredient.MaDonVi, ingredient.SoLuong, 1).ToString() equals unit.MaDonVi
                                  select new IngredientDTO
                                  {
                                      MaNguyenLieu = ingredient.MaNguyenLieu,
                                      TenNguyenLieu = ingredient.TenNguyenLieu,
                                      MaDonVi = Helper.converntUnit(ingredient.MaDonVi, ingredient.SoLuong, 1).ToString(),
                                      SoLuong = (double) Helper.converntUnit(ingredient.MaDonVi, ingredient.SoLuong, 2),
                                      TenDonVi = unit.TenDonVi
                                  }).ToList();

                    return ("Lấy danh sách nguyên liệu thành công", result);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Xoá nguyên liệu 
        /// </summary>
        /// <param name="IngredientID"></param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> DeleteIngredient(string IngredientID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.DeleteTaskAsync("NguyenLieu/" + IngredientID);
                    return ("Xoá nguyên liệu thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }

        

        /// <summary>
        /// Tìm nguyên liệu theo tên nguyên liệu
        /// </summary>
        /// <param name="ingredientName"> Tên nguyên liệu </param>
        /// <returns>
        ///     1. Thông báo
        ///     2. Nguyên liệu
        /// </returns>
        public async Task<(string, IngredientDTO)> findIngredientByName(string ingredientName, string ingredientID = "null")
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse ingredientResponse = await context.Client.GetTaskAsync("NguyenLieu");
                    Dictionary<string, IngredientDTO> ingredientData = ingredientResponse.ResultAs<Dictionary<string, IngredientDTO>>();
                    IngredientDTO ingredient = ingredientData.Values.FirstOrDefault(x => x.TenNguyenLieu == ingredientName && x.MaNguyenLieu != ingredientID);

                    if (ingredient != null)
                        return ("Tìm thành công", ingredient);
                    else
                        return ("Không tồn tại", null);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }
    }
}
