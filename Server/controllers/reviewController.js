const db = require('../config/firebase');
const { optionsDateTime } = require('../utils/helper')


const getNewId = async () => {
    try {
        const reviewRef = db.ref('DanhGia/')
        const snapshot = await reviewRef.once('value')
        const reviews = snapshot.val()
        if (reviews) {
            const currentId = parseInt(Object.keys(reviews)[Object.keys(reviews).length - 1].slice(2))
            const newId = 'DG' + String(currentId + 1).padStart(4, '0')
            return newId
        } else {
            return 'DG0001'
        }
    } catch (error) {
        console.log(error)
    }
}

const setReview = async (req, res) => {
    try {
        const { userId, productId } = req.params
        const { rating, content } = req.body
        const newId = await getNewId()

        const currentDate = new Date()
        const option = optionsDateTime
        const formattedDate = currentDate.toLocaleString('vi-VN', option)

        await db.ref(`DanhGia/${newId}`).set({
            MaDanhGia: newId,
            MaNguoiDung: userId,
            MaSanPham: productId,
            DiemDanhGia: rating,
            VanBanDanhGia: content,
            ThoiGianDanhGia: formattedDate
        })

        return res.status(200).json({success: true, message: "Đánh giá thành công"})
    } catch (error) {
        console.log(error)
        return res.status(500).json({success: false, message: "Lỗi server"})
    }
}

const getReview = async (req, res) => {
    try {
        const { productId }= req.params
        const snapshot = await db.ref('DanhGia/').orderByChild('MaSanPham').equalTo(productId).once('value')
        const reviews = snapshot.val()

        return res.status(200).json({success: true, data: reviews})
    } catch (error) {
        return res.status(500).json({success: false, message: "Lỗi server"})
    }
}

module.exports = {
    setReview,
    getReview
}