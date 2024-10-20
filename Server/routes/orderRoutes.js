const express = require('express');
const router = express.Router();
const orderController = require('../controllers/orderController');
const authenticateToken = require('../middleware/authMiddleware');

//shipper
router.put('/take-up-order', orderController.takeUpOrder)
router.get('/shipper/:shipperId', orderController.getOrderByShipperId)
router.get('/shipper/order-success/:shipperId', orderController.getOrdersSuccessByShipper)
router.get('/all-orders', orderController.getAllOrders)
router.get('/status-order/:orderId', orderController.getStatusOrder)
router.put('/cancel-order', orderController.cancelOrder)

router.post('/save-order/:userId', authenticateToken, orderController.saveOrder)
router.get('/orders/:userId?', authenticateToken, orderController.getOrders)
router.put('/set-status-order/:orderId', authenticateToken, orderController.setStatusOrder)
router.put('/bill/:orderId', authenticateToken, orderController.setBillIDOrderHandler)

module.exports = router