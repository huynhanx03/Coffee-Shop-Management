const db = require('../config/firebase');
const { getDiscountProducts, updateDiscountFromProduct, deleteProduct, getProductSizes, updateQuantityProduct, getMaxProductId, addProduct, updateProduct, handleGetProductById } = require('../dao/productDAO');
const { nextID } = require('../utils/helper');

const getCategories = async (req, res) => {
    try {
        const snapshot = await db.ref('LoaiSanPham/').once('value');
        const categories = snapshot.val();

        if (!categories) return res.status(404).json({ success: false, data: 'Không tìm thấy danh mục' });

        return res.status(200).json({ success: true, data: categories });
    } catch (error) {
        console.log(error);
        return res.status(500).json({ success: false, message: "Lỗi server!" });
    }
}

const getProducts = async (req, res) => {
    try {
        const snapshot = await db.ref('SanPham/').once('value');
        const products = snapshot.val();

        return res.status(200).json({ success: true, data: products });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
}

const getProductSizesHandler = async (req, res) => {
    try {
        const data = await getProductSizes()

        return res.status(200).json({ success: true, data: data });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
}

const getProductById = async (req, res) => {
    try {
        const productId = req.params.productId;
        const product = await handleGetProductById(productId);

        if (!product) return res.status(404).json({ success: false, data: 'Không tìm thấy sản phẩm' });

        return res.status(200).json({ success: true, data: product });
    } catch (error) {
        console.log(error);
        return res.status(500).json({ success: false, message: "Lỗi server!" });
    }
}

const getProductsSale = async (req, res) => {
    try {
        const productsSaleSnapshot = await db.ref('SanPhamGiamGiaHomNay/').once('value');
        const productsSale = productsSaleSnapshot.val();

        return res.status(200).json({ success: true, data: productsSale });
    } catch (error) {
        console.log(error);
        return res.status(500).json({ success: false, message: "Lỗi server!" });
    }
}

const getProductsBestSeller = async (req, res) => {
    try {
        const billsSnapshot = await db.ref('HoaDon/').once('value');
        const bills = billsSnapshot.val();

        const products = Object.values(bills).reduce((acc, bill) => {
            const products = Object.values(bill.ChiTietHoaDon).map(product => {
                return {
                    MaSanPham: product.MaSanPham,
                    SoLuong: product.SoLuong
                }
            });


            return [...acc, ...products];
        }, []);

        const totalSold = {}

        products.map((product) => {
            if (totalSold[product.MaSanPham]) {
                totalSold[product.MaSanPham] += product.SoLuong;
            } else {
                totalSold[product.MaSanPham] = product.SoLuong;
            }
        })

        const sortObj = Object.fromEntries(
            Object.entries(totalSold).sort(([,a],[,b]) => b-a)
        )

        return res.status(200).json({ success: true, data: Object.keys(sortObj) });

    } catch (error) {
        console.log(error);
        return res.status(500).json({ success: false, message: "Lỗi server!" });
    }
}

const getDiscountProductsHandler = async (req, res) => {
    try {
        const products = await getDiscountProducts();

        return res.status(200).json({ success: true, data: products });
    } catch (error) {
        console.log(error);
        return res.status(500).json({ success: false, message: error.message });
    }
}

const updateDiscountFromProductHandler = async (req, res) => {
    try {
        const { productID } = req.params;
        
        await updateDiscountFromProduct(productID, req.body.PhanTramGiam);

        return res.status(200).json({ success: true, data: "Successfully" });
    } catch (error) {
        console.log(error);
        return res.status(500).json({ success: false, message: error.message });
    }
}

const deleteProductHandler = async (req, res) => {
    const { productID } = req.params;

    try {
        await deleteProduct(productID);

        return res.status(200).json({ success: true, message: 'Product deleted successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const updateQuantityProductHandler = async (req, res) => {
    const { quantity } = req.body;
    const { productID } = req.params;

    try {
        await updateQuantityProduct(productID, quantity);

        return res.status(201).json({ success: true, message: 'Product quantity updated successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const addProductHandler = async (req, res) => {
    const product = req.body;

    try {
        const maxProductId = await getMaxProductId();
        const newProductId = nextID(maxProductId, "SP");

        product.MaSanPham = newProductId;

        await addProduct(product);

        return res.status(201).json({ success: true, message: 'Product added successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const updateProductHandler = async (req, res) => {
    const product = req.body;
    const { productID } = req.params;

    try {
        product.MaSanPham = productID;

        await updateProduct(product);

        return res.status(201).json({ success: true, message: 'Product added successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

module.exports = {
    getCategories,
    getProducts,
    getProductById,
    getProductsSale,
    getProductsBestSeller,
    getDiscountProductsHandler,
    updateDiscountFromProductHandler,
    deleteProductHandler,
    getProductSizesHandler,
    updateQuantityProductHandler,
    addProductHandler,
    updateProductHandler
}