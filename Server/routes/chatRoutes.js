const express = require('express')
const router = express.Router()
const chatController = require('../controllers/chatController')
const authenticateToken = require('../middleware/authMiddleware');

router.get('/userContacts', authenticateToken, chatController.getUserContactsHandler)
router.get('/messages/:userID', authenticateToken, chatController.getMessagesHandler)
router.post('/messages/:userID', authenticateToken, chatController.addMessageHandler)

module.exports = router
