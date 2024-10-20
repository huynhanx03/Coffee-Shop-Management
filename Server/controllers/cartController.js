const db = require('../config/firebase')

/**
 * @notice Check if an item is present in cart
 * @param item product to be checked
 * @return true if item is present, false otherwise
 */
const itemPresent = async (MaSanPham, userId) => {
    try {
        const cartsSnapshot = await db
            .ref(`GioHang/${userId}`)
            .orderByChild('MaSanPham')
            .equalTo(MaSanPham)
            .once('value')
        const carts = cartsSnapshot.val()

        if (carts) {
            return [true, item.SoLuong]
        }

        return false
    } catch (error) {
        console.log(error)
    }
}

/**
 * @notice Add a new item to cart
 * @dev If item is already presented in cart, increase the quantity of the item
 * @param item the item to be added to cart
 */
const setCart = async (req, res) => {
    try {
        const userId = req.params.userId
        const item = req.body
        const present = await itemPresent(item.MaSanPham, userId)

        if (present[0]) {
            await db.ref(`GioHang/${userId}/${item.MaSanPham}`).update({
                SoLuong: present[1] + item.SoLuong,
            })
        } else {
            await db.ref(`GioHang/${userId}/${item.MaSanPham}`).set({
                TenSanPham: item.TenSanPham,
                Gia: item.Gia,
                HinhAnh: item.HinhAnh,
                KichThuoc: item.KichThuoc,
                MaSanPham: item.MaSanPham,
                SoLuong: item.SoLuong,
                PhanTramGiam: item.PhanTramGiam,
                GiaGoc: item.GiaGoc,
            })
        }

        return res.status(200).json({ success: true, message: 'Cập nhật giỏ hàng thành công!' })
    } catch (error) {
        return res.status(500).json({ success: false, message: 'Lỗi server!' })
    }
}

/**
 * @notice Delete an item from cart
 * @param item the item to be deleted from cart
 * @returns
 */
const deleteItemCart = async (req, res) => {
    try {
        const { userId, MaSanPham } = req.params

        await db.ref(`GioHang/${userId}/${MaSanPham}`).remove()

        return res.status(200).json({ success: true, message: 'Xóa sản phẩm khỏi giỏ hàng thành công!' })
    } catch (error) {
        return res.status(500).json({ success: false, message: 'Lỗi server!' })
    }
}

/**
 * @notice remove all items from cart
 */
const removeCart = async (req, res) => {
    try {
        const userId = req.params.userId
        await db.ref(`GioHang/${userId}`).remove()

        return res.status(200).json({ success: true, message: 'Xóa giỏ hàng thành công!' })
    } catch (error) {
        return res.status(500).json({ success: false, message: 'Lỗi server!' })
    }
}

/**
 * @notice Get all items in cart from database
 */
const getCartHandler = async (userId) => {
    try {
        const cartsSnapshot = await db.ref(`GioHang/${userId}`).once('value')
        const carts = cartsSnapshot.val()

        return carts
    } catch (error) {
        console.log(error)
    }
}

/**
 * @notice Get all items in cart from database
 */
const getCart = async (req, res) => {
    try {
        const userId = req.params.userId
        const carts = getCartHandler(userId)

        if (!carts) {
            return res.status(200).json({ success: true, message: 'Giỏ hàng trống!' })
        }

        return res.status(200).json({ success: true, data: carts })
    } catch (error) {
        return res.status(500).json({ success: false, message: 'Lỗi server!' })
    }
}

const getProductById = async (productId) => {
    try {
        const snapshot = await db.ref('SanPham/' + productId).once('value');
        const product = snapshot.val();

        return product
    } catch (error) {
        return error.response.data
    }
}

/**
 * @notice Update the cart with the latest price
 * @returns The cart that is updated the latest price
 */
const updateCartWithLastPrice = async (req, res) => {
    try {
        const userId = req.params.userId
        const carts = await getCartHandler(userId)
        
        for (const key in carts[userId]) {
            const product = carts[userId][key]
            const detailProduct = await getProductById(product.MaSanPham)
            if (detailProduct.data.PhanTramGiam !== product.PhanTramGiam) {
                product.Gia = product.GiaGoc * (1 - detailProduct.data.PhanTramGiam / 100)
                product.PhanTramGiam = detailProduct.PhanTramGiam
            }
        }

        if (carts) {
            return res.status(200).json({ success: true, data: carts })
        } else {
            return res.status(404).json({ success: true, message: 'Giỏ hàng trống!' })
        }
    } catch (error) {
        return res.status(500).json({ success: false, message: 'Lỗi server!' })
    }
}

module.exports = {
    setCart,
    deleteItemCart,
    removeCart,
    getCart,
    updateCartWithLastPrice,
}
