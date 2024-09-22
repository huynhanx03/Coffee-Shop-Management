const express = require('express');
const router = express.Router();
const billImportController = require('../controllers/billImportController');
const authenticateToken = require('../middleware/authMiddleware');

router.post('/bill-import', authenticateToken, billImportController.addBillImportHandler);
router.get('/bill-imports', authenticateToken, billImportController.getImportBillsHandler);
router.delete('/bill-import/:billImportID', authenticateToken, billImportController.deleteBillImportHandler);

module.exports = router;
