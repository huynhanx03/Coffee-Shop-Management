const db = require('../config/firebase');
const { convertToDateTime } = require('../utils/helper');

const getMaxBillImportId = async () => {
    try {
        const snapshot = await db.ref('PhieuNhapKho').once('value');
        const data = snapshot.val();

        if (data) {
            const maxBillImportId = Object.keys(data).reduce((max, current) => current > max ? current : max, "");
            return maxBillImportId;
        }

        return "";
    } catch (error) {
        console.error("Error getting max BillImport ID:", error);
        return "";
    }
};

const addBillImport = async (billImport) => {
    await db.ref(`PhieuNhapKho/${billImport.MaPhieuNhapKho}`).set(billImport);
};

const deleteBillImport = async (billImportID) => {
    await db.ref(`PhieuNhapKho/${billImportID}`).remove();
};

const getImportBills = async (fromDate, toDate) => {
    try {
        // Lấy dữ liệu từ nút "PhieuNhapKho" trong Firebase
        const importBillSnapshot = await db.ref('PhieuNhapKho').once('value');
        const importBillData = importBillSnapshot.val();

        // Lấy dữ liệu từ nút "NguoiDung" trong Firebase
        const userSnapshot = await db.ref('NguoiDung').once('value');
        const userData = userSnapshot.val();

        // Chuyển đổi sang dạng mảng
        const importBills = Object.values(importBillData);
        const users = Object.values(userData);

        // Lọc và map các phiếu nhập kho theo thời gian và dữ liệu cần thiết
        const result = importBills
            .filter(importBill => {
                const importDate = convertToDateTime(importBill.NgayTaoPhieu); // Parse the date
                return importDate >= fromDate && importDate <= toDate;
            })
            .map(importBill => {
                const user = users.find(user => user.MaNguoiDung === importBill.MaNhanVien);

                return {
                    MaNhanVien: importBill.MaNhanVien,
                    MaPhieuNhapKho: importBill.MaPhieuNhapKho,
                    NgayTaoPhieu: importBill.NgayTaoPhieu,
                    TongTien: importBill.TongTien,
                    TenNhanVien: user.HoTen,
                    ChiTietPhieuNhapKho: importBill.ChiTietPhieuNhapKho
                };
            });

        return result;

    } catch (error) {
        throw error;
    }
};

module.exports = {
    getMaxBillImportId,
    addBillImport,
    deleteBillImport,
    getImportBills
};