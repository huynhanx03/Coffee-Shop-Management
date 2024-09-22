const { getIngredients, getMaxIngredientId, addIngredient, deleteIngredient, updateIngredient, getUnits, updateQuantityIngredient } = require('../dao/IngredientDAO');
const { nextID } = require('../utils/helper');

const getIngredientsHandle = async (req, res) => {
    try {
        const Ingredients = await getIngredients();

        return res.status(200).json({ success: true, data: Ingredients });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const getUnitsHandler = async (req, res) => {
    try {
        const units = await getUnits();

        return res.status(200).json({ success: true, data: units });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const addIngredientHandler = async (req, res) => {
    const ingredient = req.body;

    try {
        const maxIngredientId = await getMaxIngredientId()
        const newIngredientId = nextID(maxIngredientId, "NL")

        ingredient.MaNguyenLieu = newIngredientId

        await addIngredient(ingredient)

        return res.status(201).json({ success: true, message: 'Ingredient added successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const deleteIngredientHandler = async (req, res) => {
    const { ingredientID } = req.params;

    try {
        await deleteIngredient(ingredientID);

        return res.status(200).json({ success: true, message: 'Ingredient deleted successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const updateIngredientHandler = async (req, res) => {
    const ingredient = req.body;
    const { ingredientID } = req.params;

    ingredient.MaNguyenLieu = ingredientID
    
    try {
        await updateIngredient(ingredient);
        
        return res.status(201).json({ success: true, message: 'Ingredient updated successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const getIngredientHandler = async (req, res) => {
    const { ingredientID } = req.params;
    
    try {
        const ingredient = await getIngredient(ingredientID);
        
        return res.status(200).json({ success: true, message: 'Lấy nguyên liệu thành công', ingredient });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const updateQuantityIngredientHandler = async (req, res) => {
    const { quantity } = req.body;
    const { ingredientID } = req.params;
    
    try {
        await updateQuantityIngredient(ingredientID, quantity);
        
        return res.status(201).json({ success: true, message: 'Ingredient updated successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

module.exports = {
    getIngredientsHandle,
    addIngredientHandler,
    deleteIngredientHandler,
    updateIngredientHandler,
    getUnitsHandler,
    updateQuantityIngredientHandler,
    getIngredientHandler
};
