const express = require('express');
const router = express.Router();
const orderController = require('../controllers/orderController');
const authenticateToken = require('../middleware/authMiddleware');

router.post('/:userId', authenticateToken, orderController.saveOrder)
router.get('/:userId', orderController.getOrders)
router.put('/:orderId', authenticateToken, orderController.setStatusOrder)

module.exports = router