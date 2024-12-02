const db = require('../config/firebase')
const { handleGetProductById } = require('../dao/productDAO')
const { getOrdersSuccessByShipperDAO, getOrdersDAO, takeUpOrderDAO, getOrdersByShipperDAO, getStatusOrderDAO, cancelOrderDAO } = require('../dao/shipper/orderDAO')
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
    const { products, total, transFee, addressData, status } = req.body
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
            ThanhToan: status === 'Đã thanh toán' ? true : false,
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
        var orders;

        if (userId) {
            ordersSnapshot = await db.ref('DonHang/').orderByChild('MaNguoiDung').equalTo(userId).once('value');
            orders = ordersSnapshot.val()

        } else {
            const ordersSnapshot = await db.ref('DonHang/').once('value');
            const ordersData = ordersSnapshot.val();
            const ordersArray = Object.values(ordersData);

            const userSnapshot = await db.ref('NguoiDung').once('value');
            const userData = userSnapshot.val();
            const usersArray = Object.values(userData);

            const result = ordersArray.map(order => {
                const user = usersArray.find(user => user.MaNguoiDung === order.MaNguoiDung);
            
                order.TenKhachHang = user ? user.HoTen : 'Unknown Customer';
                
                return order; 
            });

            orders = result;
        }

        return res.status(200).json({ success: true, data: orders || 'Khách hàng chưa tạo đơn hàng' })
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
}

const setStatusOrder = async (req, res) => {
    try {
        const orderId = req.params.orderId
        const { status } = req.body
        await db.ref('DonHang/' + orderId).update({
            TrangThai: status,
            ThanhToan: ThanhToan ? true : status === 'Đã nhận hàng' ? true : false,
        })

        return res.status(200).json({ success: true, message: 'Cập nhật trạng thái đơn hàng thành công' })
    } catch (error) {
        return res.status(500).json({ success: false, message: "Lỗi server!" });
        
    }
}

const setBillIDOrderHandler = async (req, res) => {
    try {
        const orderId = req.params.orderId;
        const { billID } = req.body;

        await db.ref('DonHang/' + orderId).update({
            MaHoaDon: billID,
        });

        return res.status(200).json({ success: true, message: 'Cập nhật đơn hàng thành công' });
    } catch (error) {
        return res.status(500).json({ success: false, message: 'Lỗi server!', error: error.message });
    }
};

//shipper
const getAllOrders = async (req, res) => {
    try {
        const result = await getOrdersDAO()

        return res.status(200).json({ success: true, data: result.data })
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
}

const getOrderByShipperId = async (req, res) => {
    try {
        const { shipperId } = req.params
        const result = await getOrdersByShipperDAO(shipperId)

        return res.status(200).json({ success: true, data: result.data })
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
}

const getOrdersSuccessByShipper = async (req, res) => {
    try {
        const { shipperId } = req.params
        const result = await getOrdersSuccessByShipperDAO(shipperId)

        return res.status(200).json({ success: true, data: result.data })
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
}

const takeUpOrder = async (req, res) => {
    try {
        const { orderId, shipperId } = req.body
        const result = await takeUpOrderDAO(orderId, shipperId)

        return res.status(200).json({ success: true, message: result.message })
    } catch (error) {
        if (error.message === 'Mã đơn hàng không tồn tại' || error.message === 'Đơn hàng đã được nhận') {
            return res.status(400).json({ success: false, message: error.message });
        }
        return res.status(500).json({ success: false, message: error.message });
    }
}

const getStatusOrder = async (req, res) => {
    try {
        const { orderId } = req.params
        const result = await getStatusOrderDAO(orderId)

        return res.status(200).json({ success: true, data: result.data })
    } catch (error) {
        if (error.message === 'Mã đơn hàng không tồn tại') {
            return res.status(400).json({ success: false, message: error.message });
        }
        return res.status(500).json({ success: false, message: error.message });
    }
}

const cancelOrder = async (req, res) => {
    try {
        const { orderId, shipperId } = req.body
        const result = await cancelOrderDAO(orderId, shipperId)

        return res.status(200).json({ success: true, message: result.message })
    } catch (error) {
        if (error.message === 'Mã đơn hàng không tồn tại') {
            return res.status(400).json({ success: false, message: error.message });
        }
        return res.status(500).json({ success: false, message: error.message });
    }
}

module.exports = {
    saveOrder,
    calculateTotal,
    getOrders,
    setStatusOrder,
    getAllOrders,
    getOrdersSuccessByShipper,
    takeUpOrder,
    getOrderByShipperId,
    getStatusOrder,
    cancelOrder,
    setBillIDOrderHandler
}