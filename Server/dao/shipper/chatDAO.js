const db = require('../../config/firebase')
const { optionsDateTime } = require('../../utils/helper')

const getAllUserChatDAO = async (employeeId, userId) => {
    try {
        const snapshot = await db.ref('TinNhan').once('value')
        const allUserChat = snapshot.val()

        let allUserChatArray = []
        if (userId) {
            allUserChatArray = Object.keys(allUserChat).filter((key) => allUserChat[key].MaKhachHang === userId)
        } else {
            allUserChatArray = Object.keys(allUserChat).filter((key) => allUserChat[key].MaNhanVien === employeeId)
        }

        const userInfo = allUserChatArray.map((key) => {
            const lastDetail = allUserChat[key].NoiDung[allUserChat[key].NoiDung.length - 1]
            return {
                NoiDung: lastDetail,
                KhachHang: allUserChat[key].KhachHang,
                NhanVien: allUserChat[key].NhanVien,
            }
        })

        return userInfo
    } catch (error) {
        throw new Error('Lỗi server!')
    }
}

const makeChatDAO = async (employee, user) => {
    try {
        const snapshot = await db.ref('TinNhan').once('value')
        const allUserChat = snapshot.val()

        const options = optionsDateTime

        const flag = Object.keys(allUserChat).some((key) => key === employee.MaNhanVien + '-' + user.MaKhachHang)

        if (!allUserChat || !flag) {
            await db.ref('TinNhan/' + employee.MaNhanVien + '-' + user.MaKhachHang).set({
                MaNhanVien: employee.MaNhanVien,
                MaKhachHang: user.MaKhachHang,
                KhachHang: {
                    MaKhachHang: user.MaKhachHang,
                    HinhAnh: user.HinhAnh,
                    HoTen: user.HoTen,
                    SoDienThoai: user.SoDienThoai
                },
                NhanVien: {
                    MaNhanVien: employee.MaNhanVien,
                    HinhAnh: employee.MaNhanVien === 'ND0001' ? 'https://res.cloudinary.com/dev9hnuhw/image/upload/v1715357953/coffee/jidfceqt6pbovf1ztnjc.png' : employee.HinhAnh,
                    HoTen: employee.MaNhanVien === 'ND0001' ? 'Admin' : employee.HoTen,
                    SoDienThoai: employee.SoDienThoai
                },
                NoiDung: {
                    0: {
                        MaNhanVien: employee.MaNhanVien,
                        ChiTiet: employee.MaNhanVien === 'ND0001' ? 'EPSRO xin chào, hân hạnh được phục vụ quý khách 🫶' : 'Xin chào, tôi là tài xế giao hàng cho bạn ✌️🫶',
                        ThoiGian: new Date().toLocaleString('vi-VN', options),
                        DaXem: false,
                    },
                },
            })
        }
    } catch (error) {
        throw new Error('Lỗi server!')
    }
}

const sendMessageDAO = async (employeeId, userId, message, user) => {
    try {
        const id = employeeId + '-' + userId
        const snapshot = await db.ref('TinNhan/' + id).once('value')
        const chat = snapshot.val()

        if (!chat) {
            throw new Error('Không tìm thấy cuộc trò chuyện!')
        }

        const options = optionsDateTime
        const date = new Date().toLocaleString('vi-VN', options)

        const newId = chat.NoiDung ? Object.keys(chat.NoiDung).length : 0

        if (user) {
            await db.ref('TinNhan/' + id + '/NoiDung/' + newId).set({
                MaKhachHang: userId,
                ChiTiet: message,
                ThoiGian: date,
                DaXem: false,
            })    
        } else {
            await db.ref('TinNhan/' + id + '/NoiDung/' + newId).set({
                MaNhanVien: employeeId,
                ChiTiet: message,
                ThoiGian: date,
                DaXem: false,
            })
        }
    } catch (error) {
        if (error.message === 'Không tìm thấy cuộc trò chuyện!') {
            throw new Error(error.message)
        }
        throw new Error('Lỗi server!')
    }
}

const getAllChatDAO = async (employeeId, userId) => {
    try {
        const id = employeeId + '-' + userId
        const snapshot = await db.ref('TinNhan/' + id).once('value')
        const chat = snapshot.val()

        if (!chat) {
            throw new Error('Không tìm thấy cuộc trò chuyện!')
        }

        const chatArray = chat.NoiDung ? Object.keys(chat.NoiDung).map((key) => chat.NoiDung[key]) : []

        return chatArray
    } catch (error) {
        if (error.message === 'Không tìm thấy cuộc trò chuyện!') {
            return []
        }
        throw new Error('Lỗi server!')
    }
}

const setSeenDAO = async (employeeId, userId, user) => {
    try {
        const id = employeeId + '-' + userId
        const snapshot = await db.ref('TinNhan/' + id + '/NoiDung').once('value')
        const chat = snapshot.val()

        if (!chat) {
            throw new Error('Không tìm thấy cuộc trò chuyện!')
        }

        if (user) {
            chat.forEach(async (message, index) => {
                if (!message.DaXem && message.MaNhanVien) {
                    await db.ref(`TinNhan/${id}/NoiDung/${index}`).update({ DaXem: true });
                }
            });
        } else {
            chat.forEach(async (message, index) => {
                if (!message.DaXem && message.MaKhachHang) {
                    await db.ref(`TinNhan/${id}/NoiDung/${index}`).update({ DaXem: true });
                }
            });
        }
        
    } catch (error) {
        if (error.message === 'Không tìm thấy cuộc trò chuyện!') {
            throw new Error(error.message)
        }
        throw new Error('Lỗi server!')
    }
}

module.exports = { getAllUserChatDAO, makeChatDAO, sendMessageDAO, getAllChatDAO, setSeenDAO }
