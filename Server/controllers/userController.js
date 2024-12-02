const db = require('../config/firebase')
const jwt = require('jsonwebtoken')
const dotenv = require('dotenv')
const crypto = require('crypto')
const bcrypt = require('bcrypt')
const { updateUser, checkIDCard, checkEmail, checkNumberPhone, checkUsername } = require('../dao/userDAO')
const { messaging } = require('firebase-admin')
const { shipperLoginDAO, setStatusShipperDAO, getStatusShipperDAO, getProfitByShipperDAO } = require('../dao/shipper/userDAO')
const { hashPassword, verifyPassword } = require('../utils/helper')

dotenv.config()

const getNewId = async () => {
    try {
        const userRef = db.ref('KhachHang/')
        const snapshot = await userRef.once('value')
        const users = snapshot.val()
        if (users) {
            const currentId = parseInt(Object.keys(users)[Object.keys(users).length - 1].slice(2))
            const newId = 'KH' + String(currentId + 1).padStart(4, '0')
            return newId
        } else {
            return 'KH0001'
        }
    } catch (error) {
        console.log(error)
    }
}

const verifyToken = async (req, res) => {
    return res.status(200).json({ success: true, message: 'Token hợp lệ' })
}

const register = async (req, res) => {
    try {
        const { name, username, phone, email, password } = req.body
        const hasPass = await hashPassword(password)
        const newId = await getNewId()
        const currentTime = new Date()
        const dateCreated = currentTime.toLocaleDateString('vi-VN')
        await db.ref('NguoiDung/' + newId).set({
            TaiKhoan: username,
            Email: email,
            MatKhau: hasPass,
            VaiTro: '2',
            CCCD_CMND: '',
            DiaChi: '',
            GioiTinh: '',
            HoTen: name,
            MaNguoiDung: newId,
            NgayTao: dateCreated,
            SoDienThoai: phone,
            HinhAnh: 'https://user-images.githubusercontent.com/5709133/50445980-88299a80-0912-11e9-962a-6fd92fd18027.png',
            NgaySinh: '',
        })

        await db.ref('KhachHang/' + newId).set({
            DiemTichLuy: 0,
            MaKhachHang: newId,
        })

        await db.ref('ChiTietMucDoThanThiet/' + newId).set({
            MaKhachHang: newId,
            ChiTiet: {
                TT0001: {
                    MaMucDoThanThiet: 'TT0001',
                    NgayDatDuoc: dateCreated,
                },
            },
        })

        return res.status(200).json({ success: true, message: 'Đăng ký thành công!', data: { MaKhachHang: newId, HinhAnh: 'https://user-images.githubusercontent.com/5709133/50445980-88299a80-0912-11e9-962a-6fd92fd18027.png', HoTen: name, SoDienThoai: phone } })
    } catch (error) {
        console.log(error)
        return res.status(500).json({ success: false, message: 'Đăng ký thất bại!' })
    }
}

const login = async (req, res) => {
    try {
        const { username, password } = req.body
        const snapshot = await db.ref('NguoiDung/').orderByChild('TaiKhoan').equalTo(username).once('value')
        const userData = snapshot.val()

        if (!userData || +userData[Object.keys(userData)[0]].VaiTro !== 2) {
            return res.status(404).json({ success: false, message: 'Không tìm thấy người dùng' })
        }

        const verify = await verifyPassword(password, userData[Object.keys(userData)[0]].MatKhau)

        if (!verify) {
            return res.status(401).json({ success: false, message: 'Sai tài khoản hoặc mật khẩu' })
        }

        const token = jwt.sign({ username: username }, process.env.SECRET_KEY, { expiresIn: '5h' })

        return res.status(200).json({ success: true, token, data: userData[Object.keys(userData)[0]] })
    } catch (error) {
        console.log(error)
    }
}

const loginDesktopHandler = async (req, res) => {
    const { username, password } = req.body
    try {
        const snapshot = await db.ref('NguoiDung/').orderByChild('TaiKhoan').equalTo(username).once('value')
        const userData = snapshot.val()

        if (!userData) {
            return res.status(404).json({ success: false, message: 'Không tìm thấy người dùng' })
        }

        const user = userData[Object.keys(userData)[0]]

        if (user.VaiTro !== 1 && user.VaiTro !== 3) {
            return res.status(401).json({ success: false, message: 'Không tìm thấy người dùng' })
        }

        if (password !== user.MatKhau) {
            return res.status(401).json({ success: false, message: 'Tài khoản hoặc mật khẩu không chính xác' })
        }

        const token = jwt.sign({ username: username }, process.env.SECRET_KEY, { expiresIn: '5h' })

        return res.status(200).json({ success: true, token, data: user })
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
}

const getUserById = async (req, res) => {
    try {
        const userId = req.params.userId
        const snapshot = await db.ref('NguoiDung/' + userId).once('value')
        const userData = snapshot.val()

        if (!userData) {
            return res.status(404).json({ success: false, message: 'Không tìm thấy người dùng' })
        }

        return res.status(200).json({ success: true, data: userData })
    } catch (error) {
        console.log(error)
    }
}

const getUserByEmail = async (req, res) => {
    const email = req.params.email;
    try {
        const snapshot = await db.ref('NguoiDung/').orderByChild('Email').equalTo(email).once('value')
        const userData = snapshot.val()

        if (!userData) {
            return res.status(404).json({ success: false, message: 'Không tìm thấy người dùng' })
        }

        return res.status(200).json({ success: true, data: userData })
    } catch (error) {
        return res.status(500).json({ success: false, message: 'Lỗi server' })
    }
}

const getUserByNumberphoneHandler = async (req, res) => {
    const { numberPhone } = req.params;
    try {
        const snapshot = await db.ref('NguoiDung/').orderByChild('SoDienThoai').equalTo(numberPhone).once('value')
        const userData = snapshot.val()

        if (!userData) {
            return res.status(404).json({ success: false, message: 'Không tìm thấy người dùng' })
        }

        return res.status(200).json({ success: true, data: userData })
    } catch (error) {
        return res.status(500).json({ success: false, message: 'Lỗi server' })
    }
}

//Update info
const updateInfo = async (req, res) => {
    const { content, type } = req.body
    const userId = req.params.userId
    
    try {
        await db.ref(`NguoiDung/${userId}`).update({
            [type]: content
        })

        return res.status(200).json({ success: true, message: 'Cập nhật thông tin thành công' })
    } catch (error) {
        return res.status(500).json({ success: false, message: 'Cập nhật thông tin thất bại' })
    }
}

//update password
const updatePassword = async (req, res) => {
    const { oldPassword, newPassword, isForgot } = req.body
    const userId = req.params.userId

    try {
        const snapshot = await db.ref('NguoiDung/' + userId).once('value')
        const userData = snapshot.val()

        if (!userData) {
            return res.status(404).json({ success: false, message: 'Không tìm thấy người dùng' })
        }

        if (oldPassword != userData.MatKhau && !isForgot) {
            return res.status(401).json({ success: false, message: 'Mật khẩu cũ không đúng' })
        }

        await db.ref(`NguoiDung/${userId}`).update({
            MatKhau: newPassword
        })

        return res.status(200).json({ success: true, message: 'Đổi mật khẩu thành công' })
    } catch (error) {
        return res.status(500).json({ success: false, message: 'Lỗi server' })
    }
}

const updateUserHandler = async (req, res) => {
    const user = req.body;
    const { userID } = req.params;

    user.MaNguoiDung = userID
    
    try {
        if (await checkIDCard(user))
            return res.status(422).json({ success: false, message: "CCCD/CMND đã tồn tại" });

        if (await checkEmail(user))
            return res.status(422).json({ success: false, message: "Email đã tồn tại" });

        if (await checkNumberPhone(user))
            return res.status(422).json({ success: false, message: "Số điện thoại đã tồn tại" });

        if (await checkUsername(user))
            return res.status(422).json({ success: false, message: "Tên tài khoản đã tồn tại" });

        await updateUser(user);
        
        return res.status(201).json({ success: true, message: 'User updated successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const checkTokenHandler = async (req, res) => {
    try {
        return res.status(200).json({ success: true, message: 'Token successfully' })
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
}

//Shipper
const shipperLogin = async (req, res) => {
    try {
        const {username, password} = req.body;
        const userData = await shipperLoginDAO(username, password);

        return res.status(200).json({ success: true, token: userData.token, data: userData.data });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
}

const setStatusShipper = async (req, res) => {
    try {
        const { shipperId, status } = req.body;
        const result = await setStatusShipperDAO(shipperId, status);
        
        return res.status(200).json({ success: true, message: result.message });
    } catch (error) {
        if (error.message === 'Không tìm thấy mã shipper') {
            return res.status(404).json({ success: false, message: error.message });
        }
        return res.status(500).json({ success: false, message: error.message });
    }
}

const getStatusShipper = async (req, res) => {
    try {
        const { shipperId } = req.params;
        const result = await getStatusShipperDAO(shipperId);

        return res.status(200).json({ success: true, data: result.data });
    } catch (error) {
        if (error.message === 'Không tìm thấy mã shipper') {
            return res.status(404).json({ success: false, message: error.message });
        }
        return res.status(500).json({ success: false, message: error.message });
    }
}

const getProfitByShipper = async (req, res) => {
    try {
        const { shipperId } = req.params;
        const result = await getProfitByShipperDAO(shipperId)

        return res.status(200).json({ success: true, data: result.data });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message })
    }
}

module.exports = {
    register,
    login,
    getUserById,
    updateInfo,
    updatePassword,
    getUserByEmail,
    updateUserHandler,
    getUserByNumberphoneHandler,
    loginDesktopHandler,
    checkTokenHandler,
    shipperLogin,
    setStatusShipper,
    getStatusShipper,
    getProfitByShipper,
    verifyToken
}
