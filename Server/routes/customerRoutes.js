const express = require('express')
const router = express.Router()
const customerController = require('../controllers/customerController')
const authenticateToken = require('../middleware/authMiddleware');

router.get('/customers', authenticateToken, customerController.getCustomersHandle)
router.get('/rankCustomers', authenticateToken, customerController.getRankCustomersHandle)
router.post('/customer', authenticateToken, customerController.addCustomerHandler)
router.put('/customer/:customerID', authenticateToken, customerController.updateCustomerHandler)
router.delete('/customer/:customerID', authenticateToken, customerController.deleteCustomerHandler)
router.put('/point/:customerID', authenticateToken, customerController.updateCustomerPointsAndRankHandler)

module.exports = router
