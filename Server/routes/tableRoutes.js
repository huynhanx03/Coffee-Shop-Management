const express = require('express');
const router = express.Router();
const tableController = require('../controllers/tableController');
const authenticateToken = require('../middleware/authMiddleware');

router.get('/tables', tableController.getTablesHandler);
router.get('/types', tableController.getTypeTablesHandler);
router.post('/table', authenticateToken, tableController.addTableHandler);
router.put('/table/:tableID', authenticateToken, tableController.updateTableHandler);
router.put('/status-table/:tableID', authenticateToken, tableController.updateTableStatusHandler);
router.delete('/table/:tableID', authenticateToken, tableController.deleteTableHandler);

module.exports = router;
