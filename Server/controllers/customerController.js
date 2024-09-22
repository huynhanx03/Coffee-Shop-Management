const { deleteAddress } = require('../dao/addressDAO');
const { getCustomers, getMaxCustomerId, addCustomer, deleteCustomer, updateCustomerRank, plusPointRankCustomer } = require('../dao/customerDAO');
const { addCustomerRank, addInitialCustomerRank, deleteDetailRank, getRankCustomers } = require('../dao/rankDAO');
const { addUser, checkEmail, checkNumberPhone, checkIDCard, checkUsername, updateUser, deleteUser } = require('../dao/userDAO')
const { nextID } = require('../utils/helper');

const getCustomersHandle = async (req, res) => {
    try {
        const customers = await getCustomers();

        return res.status(200).json({ success: true, data: customers });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const getRankCustomersHandle = async (req, res) => {
    try {
        const rankCustomers = await getRankCustomers();

        return res.status(200).json({ success: true, data: rankCustomers });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const addCustomerHandler = async (req, res) => {
    const customer = req.body;

    try {
        const maxCustomerId = await getMaxCustomerId()
        const newCustomerId = nextID(maxCustomerId, "KH")

        // Tạo đối tượng Customer
        const _Customer = {
            MaKhachHang: newCustomerId,
            DiemTichLuy: 0
        };

        // Tạo đối tượng User
        const user = {
            HoTen: customer.HoTen,
            CCCD_CMND: customer.CCCD_CMND,
            DiaChi: customer.DiaChi,
            Email: customer.Email,
            GioiTinh: customer.GioiTinh,
            NgaySinh: customer.NgaySinh,
            NgayTao: customer.NgayTao,
            SoDienThoai: customer.SoDienThoai,
            TaiKhoan: customer.TaiKhoan,
            MatKhau: customer.MatKhau,
            VaiTro: 2,
            MaNguoiDung: newCustomerId,
            HinhAnh: customer.HinhAnh
        };

        if (await checkIDCard(user))
            return res.status(422).json({ success: false, message: "CCCD/CMND đã tồn tại" });

        if (await checkEmail(user))
            return res.status(422).json({ success: false, message: "Email đã tồn tại" });

        if (await checkNumberPhone(user))
            return res.status(422).json({ success: false, message: "Số điện thoại đã tồn tại" });

        if (await checkUsername(user))
            return res.status(422).json({ success: false, message: "Tên tài khoản đã tồn tại" });

        await addCustomer(_Customer);
        await addUser(user);

        // Tạo mức độ thân thiết
        const detailRank = {
            MaMucDoThanThiet: "TT0001",
            NgayDatDuoc: new Date().toLocaleDateString('en-GB')
        };

        await addCustomerRank(newCustomerId, detailRank);
        await addInitialCustomerRank(newCustomerId);

        return res.status(201).json({ success: true, message: 'Customer added successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const updateCustomerHandler = async (req, res) => {
    const customer = req.body;
    const { customerID } = req.params;

    try {
        // Tạo đối tượng User
        const user = {
            HoTen: customer.HoTen,
            CCCD_CMND: customer.CCCD_CMND,
            DiaChi: customer.DiaChi,
            Email: customer.Email,
            GioiTinh: customer.GioiTinh,
            NgaySinh: customer.NgaySinh,
            NgayTao: customer.NgayTao,
            SoDienThoai: customer.SoDienThoai,
            TaiKhoan: customer.TaiKhoan,
            MatKhau: customer.MatKhau,
            VaiTro: 2,
            MaNguoiDung: customerID,
            HinhAnh: customer.HinhAnh
        };

        if (await checkIDCard(user))
            return res.status(422).json({ success: false, message: "CCCD/CMND đã tồn tại" });

        if (await checkEmail(user))
            return res.status(422).json({ success: false, message: "Email đã tồn tại" });

        if (await checkNumberPhone(user))
            return res.status(422).json({ success: false, message: "Số điện thoại đã tồn tại" });

        if (await checkUsername(user))
            return res.status(422).json({ success: false, message: "Tên tài khoản đã tồn tại" });

        await updateUser(user);

        return res.status(201).json({ success: true, message: 'Customer updated successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const deleteCustomerHandler = async (req, res) => {
    const { customerID } = req.params;

    try {
        await deleteCustomer(customerID);
        await deleteUser(customerID);
        await deleteDetailRank(customerID);
        await deleteAddress(customerID);

        return res.status(200).json({ success: true, message: 'Customer deleted successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const updateCustomerPointsAndRankHandler = async (req, res) => {
    const { customerID } = req.params;
    const { pointRank } = req.body;

    try {
        await plusPointRankCustomer(customerID, pointRank)

        var isTrue = true;

        while (isTrue) {
            isTrue = await updateCustomerRank(customerID)
        }

        return res.status(200).json({ success: true, message: 'Customer points and rank updated successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

module.exports = {
    getCustomersHandle,
    addCustomerHandler,
    updateCustomerHandler,
    deleteCustomerHandler,
    getRankCustomersHandle,
    updateCustomerPointsAndRankHandler
};
