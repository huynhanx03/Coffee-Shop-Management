const express = require('express');
const router = express.Router();
const addressController = require('../controllers/addressController');
const authenticateToken = require('../middleware/authMiddleware');

router.post('/:userId', authenticateToken, addressController.addAddress);
router.get('/:userId', authenticateToken, addressController.getAddress);
router.put('/:userId/:MaDC', authenticateToken, addressController.setDefaultAddress);

module.exports = router;