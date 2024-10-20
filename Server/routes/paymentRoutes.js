const express = require('express');
const router = express.Router();
const paymentController = require('../controllers/paymentController');
const authenticateToken = require('../middleware/authMiddleware');

router.post('/', authenticateToken, paymentController.payment)
router.post('/status/:orderId', paymentController.getStatusTransaction)

module.exports = router