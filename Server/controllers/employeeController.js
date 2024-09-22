const { deleteAddress } = require('../dao/addressDAO');
const { getEmployees, getMaxEmployeeId, addEmployee, deleteEmployee, updateEmployee } = require('../dao/employeeDAO');
const { addEmployeeRank, addInitialEmployeeRank, deleteDetailRank } = require('../dao/rankDAO');
const { addUser, checkEmail, checkNumberPhone, checkIDCard, checkUsername, updateUser, deleteUser } = require('../dao/userDAO')
const { nextID } = require('../utils/helper');

const getEmployeesHandle = async (req, res) => {
    try {
        const employees = await getEmployees();

        return res.status(200).json({ success: true, data: employees });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const addEmployeeHandler = async (req, res) => {
    const employee = req.body;

    try {
        const maxEmployeeId = await getMaxEmployeeId();
        const newEmployeeId = nextID(maxEmployeeId, "NV");

        // Tạo đối tượng Employee
        const _Employee = {
            MaNhanVien: newEmployeeId,
            MaChucVu: employee.MaChucVu,
            Luong: employee.Luong
        };

        // Tạo đối tượng User
        const user = {
            HoTen : employee.HoTen,
            CCCD_CMND : employee.CCCD_CMND,
            DiaChi : employee.DiaChi,
            Email : employee.Email,
            GioiTinh : employee.GioiTinh,
            NgaySinh : employee.NgaySinh,
            NgayTao : employee.NgayLam,
            SoDienThoai : employee.SoDienThoai,
            TaiKhoan : employee.TaiKhoan,
            MatKhau : employee.MatKhau,
            VaiTro : 3,
            MaNguoiDung : newEmployeeId,
            HinhAnh : employee.HinhAnh
        };

        if (await checkIDCard(user))
            return res.status(422).json({ success: false, message: "CCCD/CMND đã tồn tại" });

        if (await checkEmail(user))
            return res.status(422).json({ success: false, message: "Email đã tồn tại" });

        if (await checkNumberPhone(user))
            return res.status(422).json({ success: false, message: "Số điện thoại đã tồn tại" });

        if (await checkUsername(user))
            return res.status(422).json({ success: false, message: "Tên tài khoản đã tồn tại" });

        await addEmployee(_Employee);
        await addUser(user);

        return res.status(201).json({ success: true, message: 'Employee added successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const updateEmployeeHandler = async (req, res) => {
    const employee = req.body;
    const { employeeID } = req.params;

    try {
        // Tạo đối tượng User
        const user = {
            HoTen: employee.HoTen,
            CCCD_CMND: employee.CCCD_CMND,
            DiaChi: employee.DiaChi,
            Email: employee.Email,
            GioiTinh: employee.GioiTinh,
            NgaySinh: employee.NgaySinh,
            NgayTao: employee.NgayLam,
            SoDienThoai: employee.SoDienThoai,
            TaiKhoan: employee.TaiKhoan,
            MatKhau: employee.MatKhau,
            VaiTro: 2,
            MaNguoiDung: employeeID,
            HinhAnh: employee.HinhAnh
        };

        const _Employee = {
            MaNhanVien: employeeID,
            MaChucVu: employee.MaChucVu,
            Luong: employee.Luong
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
        await updateEmployee(_Employee);
        
        return res.status(201).json({ success: true, message: 'Employee updated successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const deleteEmployeeHandler = async (req, res) => {
    const { employeeID } = req.params;

    try {
        await deleteEmployee(employeeID);
        await deleteUser(employeeID);

        return res.status(200).json({ success: true, message: 'Employee deleted successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

module.exports = {
    getEmployeesHandle,
    addEmployeeHandler,
    updateEmployeeHandler,
    deleteEmployeeHandler
};
