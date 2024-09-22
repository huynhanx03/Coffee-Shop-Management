using Coffee.DALs;
using Coffee.DTOs;
using Coffee.Utils.Helper;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coffee.API;
using System.Windows.Media.Animation;

namespace Coffee.Services
{
    public class IngredientService
    {
        private static IngredientService _ins;
        public static IngredientService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new IngredientService();
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
            return await IngredientAPI.Ins.createIngredient(ingredient);

            // Kiểm tra tên nguyên liệu
            (string label, IngredientDTO ingredientFind) = await IngredientDAL.Ins.findIngredientByName(ingredient.TenNguyenLieu, ingredient.MaNguyenLieu);

            if (ingredientFind != null)
                return ("Tên nguyên liệu đã tồn tại", null);

            // Tạo mã nguyên liệu mới nhất
            string MaxMaNguyenLieu = await this.getMaxMaNguyenLieu();
            string NewMaNguyenLieu = Helper.nextID(MaxMaNguyenLieu, "NL");

            ingredient.MaNguyenLieu = NewMaNguyenLieu;

            return await IngredientDAL.Ins.createIngredient(ingredient);
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
            return await IngredientAPI.Ins.updateIngredient(ingredient);

            // Kiểm tra tên nguyên liệu
            (string label, IngredientDTO ingredientFind) = await IngredientDAL.Ins.findIngredientByName(ingredient.TenNguyenLieu, ingredient.MaNguyenLieu);

            if (ingredientFind != null)
                return ("Tên nguyên liệu đã tồn tại", null);

            return await IngredientDAL.Ins.updateIngredient(ingredient);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Mã nguyên liệu lớn nhất
        /// </returns>
        public async Task<string> getMaxMaNguyenLieu()
        {
            return await IngredientDAL.Ins.getMaxMaNguyenLieu();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        ///     Danh sách nguyên liệu
        /// </returns>
        public async Task<(string, List<IngredientDTO>)> getListIngredient()
        {
            return await IngredientAPI.Ins.getIngredients();
        }

        /// <summary>
        /// Xoá nguyên liệu 
        /// </summary>
        /// <param name="ingredient">Nguyên Liệu</param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> DeleteIngredient(IngredientDTO Ingredient)
        {
            return await IngredientAPI.Ins.DeleteIngredient(Ingredient.MaNguyenLieu);
        }

        /// <summary>
        /// Lấy nguyên liệu theo mã nguyên liệu
        /// </summary>
        /// <param name="IngredientID"></param>
        /// <returns></returns>
        public async Task<(string, IngredientDTO)> GetIngredient(string IngredientID)
        {
            return await IngredientAPI.Ins.GetIngredient(IngredientID);
        }

        /// <summary>
        /// Tăng số lượng nguyên liệu
        /// </summary>
        /// <param name="IngredientID"> Mã nguyên liệu </param>
        /// <param name="Quantity"> Số lượng </param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu update thành công, False nếu update thất bại
        /// </returns>
        public async Task<(string, bool)> updateIngredientQuantity(string IngredientID, double Quantity, string UnitID)
        {
            (string labelGetIngredient, IngredientDTO ingredient) = await this.GetIngredient(IngredientID);

            // Nếu lấy được nguyên liệu thì update số lượng
            if (ingredient != null)
            {
                if (ingredient.MaDonVi == UnitID)
                    ingredient.SoLuong += Quantity;
                else
                {
                    if (ingredient.MaDonVi == "DV0001" || ingredient.MaDonVi == "DV0003")
                        ingredient.SoLuong += (Quantity / 1000);
                    else
                        ingredient.SoLuong += (Quantity * 1000);
                }

                (string labelUpdate, bool isUpdate) = await IngredientAPI.Ins.updateQuantityIngredient(ingredient.MaNguyenLieu, ingredient.SoLuong);

                // Update thành công
                if (isUpdate)
                    return ("Tăng số lượng nguyên liệu thành công", true);
                else
                    return (labelUpdate, false);
            }

            return ("Không tìm được nguyên liệu", false);
        }

        /// <summary>
        /// Tìm giá trị nguyên liệu tối đa để tạo là sản phẩm
        /// </summary>
        /// <param name="listProductRecipe"></param>
        /// <returns></returns>
        public async Task<(string, int)> getMaxIngredientQuantity(List<ProductRecipeDTO> listProductRecipe)
        {
            (string label, List<IngredientDTO> listIngredient) = await this.getListIngredient();

            int maxQuantity = int.MaxValue;

            foreach (ProductRecipeDTO item in listProductRecipe)
            {
                IngredientDTO findIngredient = listIngredient.FirstOrDefault(x => x.MaNguyenLieu == item.MaNguyenLieu);

                if (findIngredient != null)
                {
                    int quantity = -1;

                    // xử lý tính toán đơn vị
                    if (item.MaDonVi == findIngredient.MaDonVi)
                       quantity = (int) (findIngredient.SoLuong / item.SoLuong);
                    else
                    {
                        if (findIngredient.MaDonVi == "DV0001" || findIngredient.MaDonVi == "DV0003")
                            quantity = (int) (findIngredient.SoLuong * 1000 / item.SoLuong);
                        else
                            quantity = (int) (findIngredient.SoLuong / 1000 / item.SoLuong);
                    }

                    maxQuantity = Math.Min(maxQuantity, quantity);
                }
                else
                    return ("Nguyên liệu trong công thức không tồn tại nữa", 0);
            }

            return ("Tìm giá trị thành công", maxQuantity);
        }

        /// <summary>
        /// Giảm số lượng nguyên liệu
        /// </summary>
        /// <param name="listProductRecipe"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<(string, bool)> reduceIngredientQuantity(List<ProductRecipeDTO> listProductRecipe, int quantity)
        {
            (string label, List<IngredientDTO> listIngredient) = await this.getListIngredient();

            foreach (ProductRecipeDTO item in listProductRecipe)
            {
                IngredientDTO findIngredient = listIngredient.FirstOrDefault(x => x.MaNguyenLieu == item.MaNguyenLieu);

                if (findIngredient != null)
                {
                    double need;

                    // xử lý tính toán đơn vị
                    if (item.MaDonVi == findIngredient.MaDonVi)
                         need = quantity * item.SoLuong;
                    else
                    {
                        if (findIngredient.MaDonVi == "DV0001" || findIngredient.MaDonVi == "DV0003") 
                            need = (quantity * item.SoLuong) / 1000;
                        else
                            need = (quantity * item.SoLuong) * 1000;
                    }

                    findIngredient.SoLuong -= need;

                    // Update Nguyên liệu

                    await IngredientAPI.Ins.updateQuantityIngredient(findIngredient.MaNguyenLieu, findIngredient.SoLuong);
                }
            }

            return ("Giảm số lượng nguyên liệu thành công", true);
        }
    }
}
