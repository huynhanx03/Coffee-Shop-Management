const db = require('../config/firebase');
const { nextID } = require('../utils/helper');
const { getRank } = require('./rankDAO');

const getCustomers = async () => {
    const customerSnapshot = await db.ref('KhachHang').once('value');
    const customerData = customerSnapshot.val();

    if (!customerData) {
        return [];
    }

    const userSnapshot = await db.ref('NguoiDung').once('value');
    const userData = userSnapshot.val();

    const customersArray = Object.values(customerData);
    const usersArray = Object.values(userData);

    // Kết hợp dữ liệu từ hai mảng
    const result = customersArray.map(customer => {
        const user = usersArray.find(user => user.MaNguoiDung === customer.MaKhachHang);

        if (user) {
            return {
                HoTen: user.HoTen,
                CCCD_CMND: user.CCCD_CMND,
                DiaChi: user.DiaChi,
                Email: user.Email,
                GioiTinh: user.GioiTinh,
                HinhAnh: user.HinhAnh,
                SoDienThoai: user.SoDienThoai,
                MaKhachHang: customer.MaKhachHang,
                DiemTichLuy: customer.DiemTichLuy,
                MatKhau: user.MatKhau,
                NgaySinh: user.NgaySinh,
                TaiKhoan: user.TaiKhoan,
                NgayTao: user.NgayTao
            };
        }
    }).filter(Boolean);

    return result;
};

const getMaxCustomerId = async () => {
    try {
        const snapshot = await db.ref('KhachHang').once('value');
        const data = snapshot.val();

        if (data) {
            const maxCustomerId = Object.keys(data).reduce((max, current) => current > max ? current : max, "");
            return maxCustomerId;
        }

        return ""
    } catch (error) {
        return ""
    }
};

const addCustomer = async (customer) => {
    await db.ref(`KhachHang/${customer.MaKhachHang}`).set(customer);
};

const deleteCustomer = async (customerID) => {
    await db.ref(`KhachHang/${customerID}`).remove();
};

const getRankCustomer = async (customerID) => {
    const response = await db.ref(`ChiTietMucDoThanThiet/${customerID}/ChiTiet`).once('value');
    const detailRankData = response.val();

    if (detailRankData) {
        const detailRankArray = Object.values(detailRankData);
        const lastRank = detailRankArray[detailRankArray.length - 1];

        return lastRank.MaMucDoThanThiet
    }
}

const plusPointRankCustomer = async (customerID, point) => {
    try {
        const customerRef = db.ref('KhachHang/' + customerID);
        
        // Get current customer data
        const snapshot = await customerRef.once('value');
        const customer = snapshot.val();

        if (!customer) {
            throw new Error("Customer not found");
        }

        // Update points
        customer.DiemTichLuy += point;

        await customerRef.update(customer);
    } catch (error) {
        throw error;
    }
};

const getCustomer = async (customerID) => {
    try {
        const snapshot = await db.ref('KhachHang/' + customerID).once('value');
        const customer = snapshot.val();

        if (!customer) {
            throw new Error("Customer not found");
        }

        return customer;
    } catch (error) {
        console.error("Error getting customer:", error);
        throw error;
    }
};

const updateCustomerRank = async (customerID) => {
    try {
        const customer = await getCustomer(customerID)

        const currentRankID = await getRankCustomer(customerID);
        const nextRankID = nextID(currentRankID, "TT");

        const nextRank = await getRank(nextRankID);

        if (nextRank && customer.DiemTichLuy >= nextRank.DiemMucDoThanThiet) {
            const detailRank = {
                MaMucDoThanThiet: nextRank.MaMucDoThanThiet,
                NgayDatDuoc: new Date().toLocaleDateString('vi-VN') // Format: dd/MM/yyyy
            };

            await db.ref(`ChiTietMucDoThanThiet/${customerID}/ChiTiet/${detailRank.MaMucDoThanThiet}`).set(detailRank);

            return true;
        }
        else {
            return false;

            throw new Error('Không đủ điểm để cập nhật hạng');
        }
    } catch (error) {
        throw error
    }
};

module.exports = {
    getCustomers,
    addCustomer,
    deleteCustomer,
    getMaxCustomerId,
    getRankCustomer,
    updateCustomerRank,
    plusPointRankCustomer
};