const express = require('express');
const router = express.Router();
const cartController = require('../controllers/cartController');
const authenticateToken = require('../middleware/authMiddleware');

router.post('/set-cart/:userId', authenticateToken, cartController.setCart)
router.delete('/delete-item-cart/:userId/:MaSanPham', authenticateToken, cartController.deleteItemCart)
router.delete('/remove-cart/:userId', authenticateToken, cartController.removeCart)
router.get('/get-cart/:userId', cartController.getCart)
router.put('/update-cart/:userId', authenticateToken, cartController.updateCartWithLastPrice)

module.exports = router;