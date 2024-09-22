const db = require('../config/firebase');

const getDiscountProducts = async () => {
    const productSnapshot = await db.ref('SanPham').once('value');
    const productData = productSnapshot.val();

    const result = Object.values(productData)
        .filter(product => product.PhanTramGiam > 0)
        .map(product => ({
            MaSanPham: product.MaSanPham,
            TenSanPham: product.TenSanPham,
            PhanTramGiam: product.PhanTramGiam
        }));

    return result;
};

const updateDiscountFromProduct = async (productID, value) => {
    await db.ref('SanPham/' + productID).update({ PhanTramGiam: value });
};

const deleteProduct = async (productID) => {
    await db.ref(`SanPham/${productID}`).remove();
};

const getProductSizes = async () => {
    const response = await db.ref('KichThuocSanPham').once('value');
    data = response.val()

    return data ? Object.values(data) : [];
};

const updateQuantityProduct = async (productID, newQuantity) => {
    await db.ref(`SanPham/${productID}`).update({ SoLuong: newQuantity });
};

const addProduct = async (product) => {
    await db.ref(`SanPham/${product.MaSanPham}`).set(product);
};

const updateProduct = async (product) => {
    await db.ref(`SanPham/${product.MaSanPham}`).update(product);
};

const getMaxProductId = async () => {
    try {
        const snapshot = await db.ref('SanPham').once('value');
        const data = snapshot.val();

        if (data) {
            const maxProductId = Object.keys(data).reduce((max, current) => current > max ? current : max, "");
            return maxProductId;
        }

        return "";
    } catch (error) {
        return "";
    }
};

const handleGetProductById = async (productId) => {
    const snapshot = await db.ref('SanPham/' + productId).once('value');
    return snapshot.val();
}

module.exports = {
    getDiscountProducts,
    updateDiscountFromProduct,
    deleteProduct,
    getProductSizes,
    updateQuantityProduct,
    addProduct,
    getMaxProductId,
    updateProduct,
    handleGetProductById
};
