const express = require('express')
const router = express.Router()
const voucherController = require('../controllers/voucherController')
const authenticateToken = require('../middleware/authMiddleware')

router.get('vouchers/:userId', voucherController.getVoucher)
router.post('/update/:voucherId/:userId', authenticateToken, voucherController.updateVoucherUsed)
router.get('/vouchers', authenticateToken, voucherController.getVouchersHandler);
router.post('/voucher', authenticateToken, voucherController.addVoucherHandler);
router.delete('/vouchers/:voucherId', authenticateToken, voucherController.deleteVoucherHandler);

module.exports = router
