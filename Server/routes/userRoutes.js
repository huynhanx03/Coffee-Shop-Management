const express = require('express');
const router = express.Router();
const userController = require('../controllers/userController');
const { registerValidationRules, validate } = require('../middleware/validators');
const authenticateToken = require('../middleware/authMiddleware');

router.post('/register', registerValidationRules(), validate, userController.register);
router.post('/login', userController.login);
router.post('/login-desktop', userController.loginDesktopHandler);
router.get('/user/:userId', authenticateToken, userController.getUserById);
router.put('/update/:userId', authenticateToken, userController.updateInfo);
router.put('/update/password/:userId', authenticateToken, userController.updatePassword);
router.get('/forgot/:email', userController.getUserByEmail);
router.put('/user/:userID', authenticateToken, userController.updateUserHandler);
router.get('/number-phone/:numberPhone', authenticateToken, userController.getUserByNumberphoneHandler);
router.get('/token/check-token', authenticateToken, userController.checkTokenHandler);
router.put('/shipper/status', userController.setStatusShipper);
router.get('/profit/:shipperId', userController.getProfitByShipper);
router.post('shipper/login', userController.shipperLogin);

module.exports = router;