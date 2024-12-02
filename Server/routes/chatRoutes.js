const express = require('express')
const router = express.Router()
const chatController = require('../controllers/chatController')
const authenticateToken = require('../middleware/authMiddleware');

router.get('/userContacts', authenticateToken, chatController.getUserContactsHandler)
router.get('/messages/:userID', authenticateToken, chatController.getMessagesHandler)
router.get('/all-user-chat', chatController.getAllUserChat)
router.get('/all-chat', chatController.getAllChat)

router.post('/messages/:userID', authenticateToken, chatController.addMessageHandler)
router.post('/make-chat', chatController.makeChat)
router.post('/send-message/:user?', chatController.sendMessage)

router.put('/seen/:user?', chatController.setSeen)

module.exports = router
