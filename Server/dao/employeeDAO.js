const db = require('../config/firebase');

const getEmployees = async () => {
    try {
        // Lấy dữ liệu từ nút "Employees" trong Firebase
        const employeesSnapshot = await db.ref('NhanVien').once('value');
        const employeesData = employeesSnapshot.val();

        // Lấy dữ liệu từ nút "Users" trong Firebase
        const usersSnapshot = await db.ref('NguoiDung').once('value');
        const usersData = usersSnapshot.val();

        // Lấy dữ liệu từ nút "Position" trong Firebase
        const positionSnapshot = await db.ref('ChucDanh').once('value');
        const positionData = positionSnapshot.val();

        if (!employeesData || !usersData || !positionData) {
            throw new Error('Dữ liệu không tồn tại');
        }

        const employeesArray = Object.values(employeesData);
        const usersArray = Object.values(usersData);
        const positionsArray = Object.values(positionData);

        const result = employeesArray.map(employee => {
            const user = usersArray.find(u => u.MaNguoiDung === employee.MaNhanVien);
            const position = positionsArray.find(p => p.MaChucVu === employee.MaChucVu);

            if (user && position) {
                return {
                    HoTen: user.HoTen,
                    CCCD_CMND: user.CCCD_CMND,
                    DiaChi: user.DiaChi,
                    Email: user.Email,
                    GioiTinh: user.GioiTinh,
                    HinhAnh: user.HinhAnh,
                    Luong: employee.Luong,
                    SoDienThoai: user.SoDienThoai,
                    MaNhanVien: employee.MaNhanVien,
                    MatKhau: user.MatKhau,
                    NgaySinh: user.NgaySinh,
                    NgayLam: user.NgayTao,
                    TaiKhoan: user.TaiKhoan,
                    MaChucVu: employee.MaChucVu,
                    TenChucVu: position.TenChucVu
                };
            }
        }).filter(Boolean);

        return result;
    } catch (error) {
        return [];
    }
};

const getMaxEmployeeId = async () => {
    try {
        const snapshot = await db.ref('NhanVien').once('value');
        const data = snapshot.val();

        if (data) {
            const maxEmployeeId = Object.keys(data).reduce((max, current) => current > max ? current : max, "");
            return maxEmployeeId;
        }

        return ""
    } catch (error) {
        return ""
    }
};

const addEmployee = async (employee) => {
    await db.ref(`NhanVien/${employee.MaNhanVien}`).set(employee);
};

const deleteEmployee = async (employeeID) => {
    await db.ref(`NhanVien/${employeeID}`).remove();
};

const updateEmployee = async (employee) => {
    await db.ref(`NhanVien/${employee.MaNhanVien}`).update(employee);
};

module.exports = {
    getEmployees,
    addEmployee,
    deleteEmployee,
    getMaxEmployeeId,
    updateEmployee
};
