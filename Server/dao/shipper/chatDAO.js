const db = require('../../config/firebase')
const { optionsDateTime } = require('../../utils/helper')

const getAllUserChatDAO = async (shipperId) => {
    try {
        const snapshot = await db.ref('TinNhan').once('value')
        const allUserChat = snapshot.val()

        const allUserChatArray = Object.keys(allUserChat).filter((key) => allUserChat[key].MaNhanVien === shipperId)

        const userInfo = allUserChatArray.map((key) => {
            const lastDetail = allUserChat[key].NoiDung[allUserChat[key].NoiDung.length - 1]
            return {
                MaKhachHang: allUserChat[key].MaKhachHang,
                HinhAnh: allUserChat[key].KhachHang.HinhAnh,
                HoTen: allUserChat[key].KhachHang.HoTen,
                NoiDung: lastDetail,
                DaXem: lastDetail.DaXem,
            }
        })

        return userInfo
    } catch (error) {
        throw new Error('Lỗi server!')
    }
}

const makeChatDAO = async (shipperId, userId) => {
    try {
        const snapshot = await db.ref('TinNhan').once('value')
        const allUserChat = snapshot.val()

        const options = optionsDateTime

        const flag = Object.keys(allUserChat).some((key) => key === shipperId + '-' + userId)

        if (!allUserChat || !flag) {
            await db.ref('TinNhan/' + shipperId + '-' + userId).set({
                MaNhanVien: shipperId,
                MaKhachHang: userId,
                KhachHang: {
                    MaKhachHang: userId,
                    HinhAnh: '',
                    HoTen: '',
                },
                Shipper: {
                    MaNhanVien: shipperId,
                    HinhAnh: '',
                    HoTen: '',
                },
                NoiDung: {
                    0: {
                        MaNhanVien: shipperId,
                        ChiTiet: 'Xin chào, tôi là tài xế giao hàng cho bạn ✌️🫶',
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

const sendMessageDAO = async (shipperId, userId, message) => {
    try {
        const id = shipperId + '-' + userId
        const snapshot = await db.ref('TinNhan/' + id).once('value')
        const chat = snapshot.val()

        if (!chat) {
            throw new Error('Không tìm thấy cuộc trò chuyện!')
        }

        const options = optionsDateTime
        const date = new Date().toLocaleString('vi-VN', options)

        const newId = chat.NoiDung ? Object.keys(chat.NoiDung).length : 0

        await db.ref('TinNhan/' + id + '/NoiDung/' + newId).set({
            MaNhanVien: shipperId,
            ChiTiet: message,
            ThoiGian: date,
            DaXem: false,
        })
    } catch (error) {
        if (error.message === 'Không tìm thấy cuộc trò chuyện!') {
            throw new Error(error.message)
        }
        throw new Error('Lỗi server!')
    }
}

const getAllChatDAO = async (shipperId, userId) => {
    try {
        const id = shipperId + '-' + userId
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

const setSeenDAO = async (shipperId, userId) => {
    try {
        const id = shipperId + '-' + userId
        const snapshot = await db.ref('TinNhan/' + id + '/NoiDung').once('value')
        const chat = snapshot.val()

        if (!chat) {
            throw new Error('Không tìm thấy cuộc trò chuyện!')
        }

        chat.forEach(async (message, index) => {
            if (!message.DaXem && message.MaKhachHang) {
                await db.ref(`TinNhan/${id}/NoiDung/${index}`).update({ DaXem: true });
            }
        });
        
    } catch (error) {
        if (error.message === 'Không tìm thấy cuộc trò chuyện!') {
            throw new Error(error.message)
        }
        throw new Error('Lỗi server!')
    }
}

module.exports = { getAllUserChatDAO, makeChatDAO, sendMessageDAO, getAllChatDAO, setSeenDAO }
