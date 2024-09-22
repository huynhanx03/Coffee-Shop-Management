using Coffee.DALs;
using Coffee.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DSA
{
    public class BinarySearch
    {
        private static BinarySearch _ins;
        public static BinarySearch Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new BinarySearch();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// Sử dụng tìm kiếm nhị phân tìm mã loại sản phẩm theo mã sản phẩm
        /// </summary>
        /// <param name="productTypes"></param>
        /// <param name="sizeProductTypes"></param>
        /// <param name="productTypeID"></param>
        /// <returns></returns>
        public int BinarySearchProductType(List<ProductTypeDTO> productTypes, int sizeProductTypes, string productTypeID)
        {
            int left = 0;
            int right = sizeProductTypes - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                int comparison = string.Compare(productTypes[mid].MaLoaiSanPham, productTypeID);

                // Nếu phần tử ở giữa là target, trả về chỉ số
                if (comparison == 0)
                    return mid;

                // Nếu phần tử ở giữa nhỏ hơn target, bỏ qua nửa bên trái
                if (comparison < 0)
                    left = mid + 1;
                // Nếu phần tử ở giữa lớn hơn target, bỏ qua nửa bên phải
                else
                    right = mid - 1;
            }

            // Nếu không tìm thấy phần tử
            return -1;
        }
    }
}
