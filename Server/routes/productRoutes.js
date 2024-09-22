const express = require('express');
const router = express.Router();
const productController = require('../controllers/productController');
const authenticateToken = require('../middleware/authMiddleware');

router.get('/categories', productController.getCategories);
router.get('/products', productController.getProducts);
router.get('/products/:productId', productController.getProductById);
router.get('/sales', productController.getProductsSale);
router.get('/bestseller', productController.getProductsBestSeller);
router.get('/discounts', productController.getDiscountProductsHandler);
router.put('/discounts/:productID', authenticateToken, productController.updateDiscountFromProductHandler);
router.delete('/product/:productID', authenticateToken, productController.deleteProductHandler);
router.get('/sizes', productController.getProductSizesHandler);
router.put('/quantity-product/:productID', authenticateToken, productController.updateQuantityProductHandler);
router.post('/product', authenticateToken, productController.addProductHandler);
router.put('/product/:productID', authenticateToken, productController.updateProductHandler);

module.exports = router;