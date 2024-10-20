const db = require('../../config/firebase')
const bcrypt = require('bcrypt')
const { verifyPassword } = require('../../utils/helper')

const shipperLoginDAO = async (username, password) => {
    try {
        const snapshot = await db.ref('NguoiDung').orderByChild('TaiKhoan').equalTo(username).once('value')
        const userData = snapshot.val()

        if (!userData || userData[Object.keys(userData)[0]].VaiTro !== 3) {
            throw new Error('Không tìm thấy người dùng')
        }

        const verify = await verifyPassword(password, userData[Object.keys(userData)[0]].MatKhau)

        if (!verify) {
            throw new Error('Sai tài khoản hoặc mật khẩu')
        }

        const token = jwt.sign({ username: username }, process.env.SECRET_KEY, { expiresIn: '5h' })

        return { token, data: userData[Object.keys(userData)[0]] }
    } catch (error) {
        if (error.message === 'Không tìm thấy người dùng' || error.message === 'Sai tài khoản hoặc mật khẩu') {
            throw new Error(error.message)
        }
        throw new Error('Lỗi server!')
    }
}

const getProfitByShipperDAO = async (shipperId) => {
    try {
        const snapshot = await db.ref('DonHang').orderByChild('MaNhanVien').equalTo(shipperId).once('value')
        const profits = snapshot.val()

        if (!shipperId) {
            throw new Error('Không tìm thấy mã shipper')
        }

        if (!profits) {
            return { success: true, data: [] }
        }

        const profitAndDay = Object.keys(profits)
            .filter(
                (key) => profits[key].TrangThai === 'Giao hàng thành công' || profits[key].TrangThai === 'Đã nhận hàng'
            )
            .map((key) => {
                const fullDate = profits[key].NgayTaoDon.split(' ')
                const date = fullDate[1]
                return {
                    Ngay: date,
                    DoanhThu: profits[key].PhiVanChuyen,
                }
            })

        return { success: true, data: profitAndDay }
    } catch (error) {
        if (error.message === 'Không tìm thấy mã shipper') {
            throw new Error(error.message)
        }
        throw new Error('Lỗi server!')
    }
}

const setStatusShipperDAO = async (shipperId, status) => {
    try {
        const snapshot = await db.ref('NhanVien').orderByChild('MaNhanVien').equalTo(shipperId).once('value')
        const shipperData = snapshot.val()

        if (!shipperData || shipperData[Object.keys(shipperData)[0]].MaChucVu != 'CD0004') {
            throw new Error('Không tìm thấy mã shipper')
        }

        await db.ref('NhanVien/' + shipperId).update({
            TrangThai: status,
        })

        return { success: true, message: 'Cập nhật trạng thái thành công' }
    } catch (error) {
        if (error.message === 'Không tìm thấy mã shipper') {
            throw new Error(error.message)
        }
        throw new Error('Lỗi server!')
    }
}

const getStatusShipperDAO = async (shipperId) => {
    try {
        const snapshot = await db.ref('NhanVien').orderByChild('MaNhanVien').equalTo(shipperId).once('value')
        const shipperData = snapshot.val()

        if (!shipperData) {
            throw new Error('Không tìm thấy mã shipper')
        }

        return shipperData[Object.keys(shipperData)[0]].TrangThai
    } catch (error) {
        if (error.message === 'Không tìm thấy mã shipper') {
            throw new Error(error.message)
        }
        throw new Error('Lỗi server!')
    }
}

module.exports = {
    shipperLoginDAO,
    getProfitByShipperDAO,
    setStatusShipperDAO,
    getStatusShipperDAO,
}
