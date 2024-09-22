const express = require('express');
const router = express.Router();
const ingredientController = require('../controllers/IngredientController');
const authenticateToken = require('../middleware/authMiddleware');

router.get('/ingredients', authenticateToken, ingredientController.getIngredientsHandle)
router.get('/units', ingredientController.getUnitsHandler)
router.post('/ingredient', authenticateToken, ingredientController.addIngredientHandler)
router.put('/ingredient/:ingredientID', authenticateToken, ingredientController.updateIngredientHandler)
router.delete('/ingredient/:ingredientID', authenticateToken, ingredientController.deleteIngredientHandler)
router.put('/quantity-ingredient/:ingredientID', authenticateToken, ingredientController.updateQuantityIngredientHandler)
router.get('/ingredient/:ingredientID', authenticateToken, ingredientController.getIngredientHandler);

module.exports = router