const express = require('express');
const router = express.Router();
const reviewController = require('../controllers/reviewController');
const authenticateToken = require('../middleware/authMiddleware');

router.post('/:userId/:productId', authenticateToken, reviewController.setReview)
router.get('/:productId', reviewController.getReview)

module.exports = router;