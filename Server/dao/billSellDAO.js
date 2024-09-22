const db = require('../config/firebase');
const { binarySearchProduct } = require('../dsa/binarySearch');
const { convertToDateTime } = require('../utils/helper');
const Constants = require('../utils/constants');

const calculateTotalPrice = (details) => {
    details = Object.values(details);

    return details.reduce((total, detail) => {
        return total + detail.ThanhTien;
    }, 0);
};

const getMaxBillSellId = async () => {
    try {
        const snapshot = await db.ref('HoaDon').once('value');
        const data = snapshot.val();

        if (data) {
            const maxBillSellId = Object.keys(data).reduce((max, current) => current > max ? current : max, "");
            return maxBillSellId;
        }

        return "";
    } catch (error) {
        console.error("Error getting max BillSell ID:", error);
        return "";
    }
};

const addBillSell = async (billSell) => {
    await db.ref(`HoaDon/${billSell.MaHoaDon}`).set(billSell);
};

const updateBillSell = async (billSell) => {
    await db.ref(`HoaDon/${billSell.MaHoaDon}`).update(billSell);
};

const deleteBillSell = async (billSellID) => {
    await db.ref(`HoaDon/${billSellID}`).remove();
};

const getBills = async (fromDate, toDate) => {
    try {
        // Lấy dữ liệu từ nút "HoaDon" trong Firebase
        const billSnapshot = await db.ref('HoaDon').once('value');
        const billData = billSnapshot.val();

        // Lấy dữ liệu từ nút "NguoiDung" trong Firebase
        const userSnapshot = await db.ref('NguoiDung').once('value');
        const userData = userSnapshot.val();

        // Lấy dữ liệu từ nút "Ban" trong Firebase
        const tableSnapshot = await db.ref('Ban').once('value');
        const tableData = tableSnapshot.val();

        // Chuyển đổi sang dạng mảng
        const bills = Object.values(billData);
        const users = Object.values(userData);
        const tables = Object.values(tableData);

        // Retrieve all products
        const productSnapshot = await db.ref('SanPham').once('value');
        const productsData = productSnapshot.val();
        const productsArray = Object.values(productsData);
        const productsSize = productsArray.length;

        // Lọc và map các hoá đơn theo thời gian và dữ liệu cần thiết
        const result = bills
            .filter(bill => {
                const billDate = convertToDateTime(bill.NgayTao); // Parse only the date part
                return billDate >= fromDate && billDate <= toDate;
            })
            .map(bill => {
                const employee = users.find(user => user.MaNguoiDung === bill.MaNhanVien);
                const customer = users.find(user => user.MaNguoiDung === bill.MaKhachHang) || { HoTen: 'Khách vãng lai' };
                const table = tables.find(table => table.MaBan === bill.MaBan) || { TenBan: 'Mang về' };

                // Map detail bill information with product details
                const listDetailBill = Object.values(bill.ChiTietHoaDon);

                listDetailBill.forEach(detailBill => {
                    const index = binarySearchProduct(productsArray, productsSize, detailBill.MaSanPham);
                    
                    if (index !== -1) {
                        const product = productsArray[index]

                        detailBill.TenSanPham = product.TenSanPham;
                        detailBill.DanhSachChiTietKichThuocSanPham = Object.values(product.ChiTietKichThuocSanPham);
                        detailBill.SelectedProductSize = detailBill.DanhSachChiTietKichThuocSanPham.find(x => x.MaKichThuoc === detailBill.MaKichThuoc);
                    }
                });

                return {
                    MaBan: bill.MaBan,
                    MaHoaDon: bill.MaHoaDon,
                    MaNhanVien: bill.MaNhanVien,
                    NgayTao: bill.NgayTao,
                    TongTien: bill.TongTien,
                    TrangThai: bill.TrangThai,
                    MaKhachHang: bill.MaKhachHang,
                    TenNhanVien: employee.HoTen,
                    TenKhachHang: customer.HoTen,
                    TenBan: table.TenBan,
                    DanhSachChiTietHoaDon: listDetailBill
                };
            });

        return result

    } catch (error) {
        throw error;
    }
};

const getBillSellByTableAndStatus = async (tableID, status) => {
    try {
        // Retrieve the bill by ID
        const billSnapshot = await db.ref('HoaDon').once('value');
        const billData = billSnapshot.val();

        if (!billData) throw new Error('Bill not found');

        var bill = Object.values(billData).find(b => b.TrangThai === status && b.MaBan === tableID)

        if (!bill)
            return null

        // Retrieve all products
        const productSnapshot = await db.ref('SanPham').once('value');
        const productsData = productSnapshot.val();
        const productsArray = Object.values(productsData);
        const productsSize = productsArray.length;

        // Map detail bill information with product details
        const listDetailBill = Object.values(bill.ChiTietHoaDon);

        listDetailBill.forEach(detailBill => {
            const index = binarySearchProduct(productsArray, productsSize, detailBill.MaSanPham);
            
            if (index !== -1) {
                const product = productsArray[index]

                detailBill.TenSanPham = product.TenSanPham;
                detailBill.DanhSachChiTietKichThuocSanPham = Object.values(product.ChiTietKichThuocSanPham);
                detailBill.SelectedProductSize = detailBill.DanhSachChiTietKichThuocSanPham.find(x => x.MaKichThuoc === detailBill.MaKichThuoc);
            }
        });

        bill.DanhSachChiTietHoaDon = listDetailBill;

        return bill;

    } catch (error) {
        console.error("Error searching bills:", error);
        throw error;
    }
};

const updateTableBooking = async (tableID, tableIDNew) => {
    try {
        const bill = await getBillSellByTableAndStatus(tableID, Constants.StatusBill.UNPAID);

        if (bill) {
            await db.ref(`HoaDon/${bill.MaHoaDon}`).update({ MaBan: tableIDNew });
            await db.ref(`Ban/${tableID}`).update({ TrangThai: Constants.StatusTable.FREE });
            await db.ref(`Ban/${tableIDNew}`).update({ TrangThai: Constants.StatusTable.BOOKED });
        }
        else {
            throw new Error("Không có hoá đơn")
        }
    } catch (error) {
        throw error
    }
};

const mergeTables = async (tableID, tableIDNew) => {
    try {
        const bill1 = await getBillSellByTableAndStatus(tableID, Constants.StatusBill.UNPAID);
        const bill2 = await getBillSellByTableAndStatus(tableIDNew, Constants.StatusBill.UNPAID);

        if (bill1 && bill2) {
            // Create a map to store merged details
            const mergedMap = new Map();

            // Helper function to add items to the map
            const addToMap = (list) => {
                list.forEach(detail => {
                    const key = `${detail.MaSanPham}-${detail.MaKichThuoc}`;
                    if (mergedMap.has(key)) {
                        // If the item already exists, update the quantity and total price
                        const existingDetail = mergedMap.get(key);
                        existingDetail.SoLuong += detail.SoLuong;
                        existingDetail.ThanhTien += detail.ThanhTien;
                    } else {
                        // If it doesn't exist, add it to the map
                        mergedMap.set(key, { ...detail });
                    }
                });
            };

            addToMap(bill1.DanhSachChiTietHoaDon)
            addToMap(bill2.DanhSachChiTietHoaDon)

            const newbill2 = {
                MaHoaDon: bill2.MaHoaDon,
                MaBan: bill2.MaBan,
                MaNhanVien: bill2.MaNhanVien,
                NgayTao: bill2.NgayTao,
                TrangThai: bill2.TrangThai,
                MaKhachHang: bill2.MaKhachHang,
                ChiTietHoaDon: Array.from(mergedMap.values()).reduce((acc, detail) => {
                    const key = `${detail.MaSanPham}-${detail.MaKichThuoc}`;
                    acc[key] = {
                        MaSanPham: detail.MaSanPham,
                        MaKichThuoc: detail.MaKichThuoc,
                        SoLuong: detail.SoLuong,
                        ThanhTien: detail.ThanhTien
                    };
                    return acc;
                }, {})
            };

            // Calculate total price
            const totalPrice = calculateTotalPrice(newbill2.ChiTietHoaDon);
                
            // Update the billSell object with the total price
            newbill2.TongTien = totalPrice;

            await deleteBillSell(bill1.MaHoaDon);
            await deleteBillSell(bill2.MaHoaDon);
            await addBillSell(newbill2)
        } else {
            return null
        }
    } catch (error) {
        return null
    }
};

module.exports = {
    getMaxBillSellId,
    addBillSell,
    deleteBillSell,
    updateBillSell,
    getBillSellByTableAndStatus,
    updateTableBooking,
    mergeTables,
    getBills
};
