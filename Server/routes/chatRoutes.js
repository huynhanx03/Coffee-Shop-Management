const express = require('express')
const router = express.Router()
const chatController = require('../controllers/chatController')
const authenticateToken = require('../middleware/authMiddleware');

router.get('/userContacts', authenticateToken, chatController.getUserContactsHandler)
router.get('/messages/:userID', authenticateToken, chatController.getMessagesHandler)
router.post('/messages/:userID', authenticateToken, chatController.addMessageHandler)
router.get('/all-user-chat/:shipperId', chatController.getAllUserChat)
router.post('/make-chat', chatController.makeChat)
router.post('/send-message', chatController.sendMessage)
router.get('/all-chat', chatController.getAllChat)
router.put('/seen', chatController.setSeen)

module.exports = router
