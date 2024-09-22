const db = require('../config/firebase')
const { handleGetProductById } = require('../dao/productDAO')
const { optionsDateTime } = require('../utils/helper')

const getNewId = async () => {
    try {
        const ordersRef = db.ref(`DonHang/`)
        const snapshot = await ordersRef.once('value')
        const orders = snapshot.val()
        if (orders) {
            const currentId = parseInt(Object.keys(orders)[Object.keys(orders).length - 1].slice(2))
            const newId = 'DH' + String(currentId + 1).padStart(4, '0')
            return newId
        } else {
            return 'DH0001'
        }
    } catch (error) {
        console.log(error)
    }
}

const calculateTotal = async (products) => {
    let total = 0
    try {
        for (const product of products) {
            const getProduct = await handleGetProductById(product.MaSanPham)
            total += getProduct.GiaBan * product.SoLuong
        }

        return total
    } catch (error) {
        return error
    }
}

const saveOrder = async (req, res) => {
    const userId = req.params.userId
    const { products, total, transFee, addressData } = req.body
    const currentDate = new Date()
    const options = optionsDateTime
    const formattedDate = currentDate.toLocaleString('vi-VN', options)
    try {
        const newId = await getNewId()
        let productObj = {}
        for (const product of products) {
            productObj = { ...productObj, [product.MaSanPham]: product }
        }

        await db.ref('DonHang/' + newId).set({
            MaDonHang: newId,
            MaNguoiDung: userId,
            TrangThai: 'Chờ xác nhận',
            SanPham: {
                ...productObj,
            },
            ThanhTien: total,
            PhiVanChuyen: transFee,
            NgayTaoDon: formattedDate,
            DiaChiGiaoHang: addressData,
        })

        return res.status(200).json({ success: true, message: 'Đặt hàng thành công' })
    } catch (error) {
        return res.status(501).json({ success: false, message: error.message });
    }
}

const getOrders = async (req, res) => {
    try {
        const userId = req.params.userId
        const ordersSnapshot = await db.ref('DonHang/').orderByChild('MaNguoiDung').equalTo(userId).once('value')
        const orders = ordersSnapshot.val()

        return res.status(200).json({ success: true, data: orders || 'Khách hàng chưa tạo đơn hàng' })
    } catch (error) {
        return res.status(501).json({ success: false, message: error.message });
    }
}

const setStatusOrder = async (req, res) => {
    try {
        const orderId = req.params.orderId
        await db.ref('DonHang/' + orderId).update({
            TrangThai: 'Đã nhận hàng',
        })

        return res.status(200).json({ success: true, message: 'Cập nhật trạng thái đơn hàng thành công' })
    } catch (error) {
        return res.status(501).json({ success: false, message: "Lỗi server!" });
        
    }
}


module.exports = {
    saveOrder,
    calculateTotal,
    getOrders,
    setStatusOrder
}