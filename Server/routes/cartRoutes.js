const express = require('express');
const router = express.Router();
const cartController = require('../controllers/cartController');
const authenticateToken = require('../middleware/authMiddleware');

router.post('/:userId', authenticateToken, cartController.setCart)
router.delete('/:userId/:MaSanPham', authenticateToken, cartController.deleteItemCart)
router.delete('/:userId', authenticateToken, cartController.removeCart)
router.get('/:userId', cartController.getCart)
router.put('/:userId', authenticateToken, cartController.updateCartWithLastPrice)

module.exports = router;