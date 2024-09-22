const express = require('express');
const router = express.Router();
const evaluateController = require('../controllers/evaluateController');
const authenticateToken = require('../middleware/authMiddleware');

router.get('/evaluates', evaluateController.getEvaluatesHandler);
router.delete('/evaluate/:evaluateID', authenticateToken, evaluateController.deleteEvaluateHandler);

module.exports = router;