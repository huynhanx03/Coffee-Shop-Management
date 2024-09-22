const express = require('express');
const router = express.Router();
const billSellController = require('../controllers/billSellController');
const authenticateToken = require('../middleware/authMiddleware');


router.post('/bill-sell', authenticateToken, billSellController.addBillSellHandler);
router.get('/bill-sells', authenticateToken, billSellController.getBillSellsHandler);
router.put('/bill-sell/:billSellID', authenticateToken, billSellController.updateBillSellHandler);
router.delete('/bill-sell/:billSellID', authenticateToken, billSellController.deleteBillSellHandler);
router.get('/bill-sell-unpaid', authenticateToken, billSellController.getBillSellByTableAndStatusHandler);
router.put('/bill-sell-table-booking', authenticateToken, billSellController.updateTableBookingHandler);
router.put('/merge-bill', authenticateToken, billSellController.mergeTablesHandler);

module.exports = router;
