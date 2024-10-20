const db = require('../../config/firebase')

const getOrdersDAO = async () => {
    try {
        const snapshot = await db.ref('DonHang').orderByChild('TrangThai').equalTo('Đã xác nhận').once('value')
        const orders = snapshot.val()

        if (!orders) {
            return { success: true, data: [] }
        }
        const ordersArray = Object.keys(orders).map(key => ({
            id: key,
            ...orders[key]
        }));

        const result = ordersArray.filter(order => order.TrangThai != 'Giao hàng thành công' || order.TrangThai != 'Đã nhận hàng')

        return { success: true, data: result }
    } catch (error) {
        console.log(error)
        throw new Error('Lỗi server!')
    }
}

const getOrdersByShipperDAO = async (shipperId) => {
    try {
        const snapshot = await db.ref('DonHang').orderByChild('MaNhanVien').equalTo(shipperId).once('value')
        const orders = snapshot.val()

        if (!orders) {
            throw new Error('Không tìm thấy mã shipper')
        }

        return { success: true, data: orders }
    } catch (error) {
        throw new Error('Lỗi server!')
    }
}

const getOrdersSuccessByShipperDAO = async (shipperId) => {
    try {
        const snapshot = await db.ref('DonHang').orderByChild('MaNhanVien').equalTo(shipperId).once('value')
        const orders = snapshot.val()

        if (!orders) {
            return { success: false, data: [] }
        }

        const successOrders = Object.values(orders).filter((order) => order.TrangThai === 'Giao hàng thành công' || order.TrangThai === 'Đã nhận hàng')

        return { success: true, data: successOrders }
    } catch (error) {
        throw new Error('Lỗi server!')
    }
}

const takeUpOrderDAO = async (orderId, shipperId) => {
    try {
        const snapshot = await db.ref('DonHang/' + orderId).once('value')
        const order = snapshot.val()

        if (!order) {
            throw new Error('Mã đơn hàng không tồn tại')
        }

        if (order.MaNhanVien) {
            throw new Error('Đơn hàng đã được nhận')
        }

        await db.ref('DonHang/' + orderId).update({
            MaNhanVien: shipperId
        })

        return { success: true, message: 'Nhận đơn hàng thành công' }
    } catch (error) {
        if (error.message === 'Mã đơn hàng không tồn tại' || error.message === 'Đơn hàng đã được nhận') {
            throw new Error(error.message)
        }
        throw new Error('Lỗi server!')
    }
}

const getStatusOrderDAO = async (orderId) => {
    try {
        const snapshot = await db.ref('DonHang/' + orderId).once('value')
        const order = snapshot.val()

        if (!order) {
            throw new Error('Mã đơn hàng không tồn tại')
        }

        if (!order.hasOwnProperty('MaNhanVien') || order.MaNhanVien === '') {
            return { success: true, data: 'Đang chờ' }
        }

        return { success: true, data: 'Đã nhận' }
    } catch (error) {
        throw new Error('Lỗi server!')
    }
}

const cancelOrderDAO = async (orderId, shipperId) => {
    try {
        const snapshot = await db.ref('DonHang/' + orderId).once('value')
        const order = snapshot.val()

        if (!order) {
            throw new Error('Mã đơn hàng không tồn tại')
        }

        if (order.MaNhanVien === shipperId) {
            await db.ref('DonHang/' + orderId).update({
                MaNhanVien: null
            })

            return { success: true, message: 'Hủy đơn hàng thành công' }
        }
    } catch (error) {
        if (error.message === 'Mã đơn hàng không tồn tại') {
            throw new Error(error.message)
        }
        throw new Error('Lỗi server!')
    }
}

module.exports = {
    getOrdersDAO,
    getOrdersByShipperDAO,
    getOrdersSuccessByShipperDAO,
    takeUpOrderDAO,
    getStatusOrderDAO,
    cancelOrderDAO
}
