using Coffee.DTOs;
using Coffee.Utils;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Coffee.DALs
{
    public class TableDAL
    {
        private static TableDAL _ins;
        public static TableDAL Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new TableDAL();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        /// <summary>
        /// Thêm bàn
        /// </summary>
        /// <param name="table"> Bàn </param>
        /// <returns>
        ///     1: Thông báo
        ///     2: Bàn
        /// </returns>
        public async Task<(string, TableDTO)> createTable(TableDTO table)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.SetTaskAsync("Ban/" + table.MaBan, table);

                    return ("Thêm bàn thành công", table);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Cập nhật bàn
        /// </summary>
        /// <param name="table"> Bàn </param>
        /// <returns>
        ///     1: Thông báo
        ///     2: Bàn
        /// </returns>
        public async Task<(string, TableDTO)> updateTable(TableDTO table)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.UpdateTaskAsync("Ban/" + table.MaBan, table);

                    return ("Cập nhật bàn thành công", table);
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
        /// <param name="tableID"> Mã bàn </param>
        /// <returns>
        ///     1. Thông báo
        ///     2. Bàn
        /// </returns>
        public async Task<(string, TableDTO)> getTable(string tableID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse tableResponse = await context.Client.GetTaskAsync("Ban/" + tableID);

                    TableDTO table = tableResponse.ResultAs<TableDTO>();

                    return ("Lấy bàn thành công", table);
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
        ///     Mã bàn lớn nhất
        /// </returns>
        public async Task<string> getMaxMaBan()
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse response = await context.Client.GetTaskAsync("Ban");

                    if (response.Body != null && response.Body != "null")
                    {
                        Dictionary<string, TableDTO> data = response.ResultAs<Dictionary<string, TableDTO>>();

                        string MaxMaBan = data.Values.Select(p => p.MaBan).Max();

                        return MaxMaBan;
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
        ///     Danh sách bàn
        /// </returns>
        public async Task<(string, List<TableDTO>)> getListTable()
        {
            try
            {
                using (var context = new Firebase())
                {
                    // Lấy dữ liệu từ nút "Table" trong Firebase
                    FirebaseResponse tableResponse = await context.Client.GetTaskAsync("Ban");
                    Dictionary<string, TableDTO> tableData = tableResponse.ResultAs<Dictionary<string, TableDTO>>();

                    // Lấy dữ liệu từ nút "TableType" trong Firebase
                    FirebaseResponse tableTypeResponse = await context.Client.GetTaskAsync("LoaiBan");
                    Dictionary<string, TableTypeDTO> tableTypeData = tableTypeResponse.ResultAs<Dictionary<string, TableTypeDTO>>();

                    var result = (from table in tableData.Values
                                  join tableType in tableTypeData.Values
                                  on table.MaLoaiBan equals tableType.MaLoaiBan
                                  select new TableDTO
                                  {
                                      MaBan = table.MaBan,
                                      TenBan = table.TenBan,
                                      MaLoaiBan = table.MaLoaiBan,
                                      TenLoaiBan = tableType.TenLoaiBan,
                                      Cot = table.Cot,
                                      Hang = table.Hang,
                                      TrangThai = table.TrangThai
                                  }).ToList();

                    return ("Lấy danh sách bàn thành công", result);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        /// <summary>
        /// Xoá bàn
        /// </summary>
        /// <param name="TableID"> Mã bàn </param>
        /// <returns>
        ///     1: Thông báo
        ///     2: True nếu xoá thành công, False xoá thất bại
        /// </returns>
        public async Task<(string, bool)> DeleteTable(string TableID)
        {
            try
            {
                using (var context = new Firebase())
                {
                    await context.Client.DeleteTaskAsync("Ban/" + TableID);
                    return ("Xoá bàn thành công", true);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, false);
            }
        }

        /// <summary>
        /// Tìm bàn theo tên
        /// </summary>
        /// <param name="tableName"> Tên bàn </param>
        /// <returns>
        ///     1. Thông báo
        ///     2. Bàn
        /// </returns>
        public async Task<(string, TableDTO)> findTableByName(string tableName, string tableID = "null")
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse tableResponse = await context.Client.GetTaskAsync("Ban");
                    Dictionary<string, TableDTO> tableData = tableResponse.ResultAs<Dictionary<string, TableDTO>>();
                    TableDTO table = tableData.Values.FirstOrDefault(x => x.TenBan == tableName && x.MaBan != tableID);

                    if (table != null)
                        return ("Tìm thành công", table);
                    else
                        return ("Không tồn tại", null);
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        public async Task<(string, TableDTO)> findTableByPosition(TableDTO table)
        {
            try
            {
                using (var context = new Firebase())
                {
                    FirebaseResponse tableResponse = await context.Client.GetTaskAsync("Ban");
                    Dictionary<string, TableDTO> tableData = tableResponse.ResultAs<Dictionary<string, TableDTO>>();
                    TableDTO tableFind = tableData.Values.FirstOrDefault(x => x.Cot == table.Cot && x.Hang == table.Hang && x.MaBan != table.MaBan);

                    if (tableFind != null)
                        return ("Tìm thành công", table);
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
