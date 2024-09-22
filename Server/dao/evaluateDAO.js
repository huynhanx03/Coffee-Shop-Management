const db = require('../config/firebase');
const { binarySearchUser, binarySearchProduct } = require('../dsa/binarySearch');
const { getRankCustomer } = require('./customerDAO');
const { getRank } = require('./rankDAO');

const getEvaluates = async () => {
    const evaluateResponse = await db.ref('DanhGia').once('value');
    const evaluateData = evaluateResponse.val();

    const userResponse = await db.ref('NguoiDung').once('value');
    const userData = userResponse.val();

    const productResponse = await db.ref('SanPham').once('value');
    const productData = productResponse.val();

    const evaluateArray = evaluateData ? Object.values(evaluateData) : [];
    const userArray = userData ? Object.values(userData) : [];
    const productArray = productData ? Object.values(productData) : [];

    const sizeUserArray = userArray.length
    const sizeProductArray = productArray.length

    const result = evaluateArray.map(evaluate => {
        const indexCustomer = binarySearchUser(userArray, sizeUserArray, evaluate.MaNguoiDung)
        const indexProduct = binarySearchProduct(productArray, sizeProductArray, evaluate.MaSanPham)
        
        const customer = indexCustomer !== -1 ? userArray[indexCustomer] : null;
        const product = indexProduct !== -1 ? productArray[indexProduct] : null;

        if (customer && product) {
            return {
                MaDanhGia: evaluate.MaDanhGia,
                MaNguoiDung: evaluate.MaNguoiDung,
                TenKhachHang: customer.HoTen,
                TenSanPham: product.TenSanPham,
                MaSanPham: evaluate.MaSanPham,
                ThoiGianDanhGia: evaluate.ThoiGianDanhGia,
                VanBanDanhGia: evaluate.VanBanDanhGia,
                DiemDanhGia: evaluate.DiemDanhGia,
            };
        }
        
        return null;
    }).filter(Boolean);

    for (const item of result) {
        const RankID = await getRankCustomer(item.MaNguoiDung);
        const rank = await getRank(RankID);

        if (rank) {
            item.MaHang = rank.MaMucDoThanThiet;
            item.TenHang = rank.TenMucDoThanThiet;
        }
    }

    return result;
};

const deleteEvaluate = async (evaluateID) => {
    await db.ref(`DanhGia/${evaluateID}`).remove();
};

module.exports = {
    getEvaluates,
    deleteEvaluate,
};
