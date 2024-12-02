const db = require('../config/firebase');
const { convertUnit } = require('../utils/helper');

const getUnits = async () => {
    const unitsSnapshot = await db.ref('DonVi/').once('value');

    units = unitsSnapshot.val()

    return units ? Object.values(units) : [];
};

const getIngredients = async () => {
    // Lấy dữ liệu từ nút "NguyenLieu" trong Firebase
    const ingredientResponse = await db.ref('NguyenLieu').once('value');
    const ingredientData = ingredientResponse.val();

    // Lấy dữ liệu từ nút "DonVi" trong Firebase
    const unitResponse = await db.ref('DonVi').once('value');
    const unitData = unitResponse.val();

    if (!ingredientData || !unitData) {
        return { message: "Không tìm thấy dữ liệu", data: [] };
    }

    const ingredientsArray = Object.values(ingredientData);
    const unitsArray = Object.values(unitData);

    const result = ingredientsArray.map(ingredient => {
        const unit = unitsArray.find(unit => convertUnit(ingredient.MaDonVi, ingredient.SoLuong, 1).toString() === unit.MaDonVi);

        if (unit) {
            return {
                MaNguyenLieu: ingredient.MaNguyenLieu,
                TenNguyenLieu: ingredient.TenNguyenLieu,
                MaDonVi: convertUnit(ingredient.MaDonVi, ingredient.SoLuong, 1).toString(),
                SoLuong: convertUnit(ingredient.MaDonVi, ingredient.SoLuong, 2),
                TenDonVi: unit.TenDonVi
            };
        }
    }).filter(Boolean);


    return result;
};

const getMaxIngredientId = async () => {
    try {
        const snapshot = await db.ref('NguyenLieu').once('value');
        const data = snapshot.val();

        if (data) {
            const maxIngredientId = Object.keys(data).reduce((max, current) => current > max ? current : max, "");
            return maxIngredientId;
        }

        return ""
    } catch (error) {
        return ""
    }
};

const checkIngredientName = async (currentIngredient) => {
    const ingredients = await getIngredients();  

    const isDuplicate = ingredients.some(ingredient => ingredient.TenNguyenLieu.toLowerCase() === currentIngredient.TenNguyenLieu.toLowerCase() && ingredient.MaNguyenLieu !== currentIngredient.MaNguyenLieu);

    return isDuplicate;
};

const addIngredient = async (ingredient) => {
    if (await checkIngredientName(ingredient)) 
        throw new Error("Tên nguyên liệu đã tồn tại")  
    else await db.ref(`NguyenLieu/${ingredient.MaNguyenLieu}`).set(ingredient);
};

const deleteIngredient = async (ingredientID) => {
    await db.ref(`NguyenLieu/${ingredientID}`).remove();
};

const updateIngredient = async (ingredient) => {
    if (await checkIngredientName(ingredient)) 
        throw new Error("Tên nguyên liệu đã tồn tại") 
    else await db.ref(`NguyenLieu/${ingredient.MaNguyenLieu}`).update(ingredient);
};

const updateQuantityIngredient = async (ingredientID, newQuantity) => {
    await db.ref(`NguyenLieu/${ingredientID}`).update({ SoLuong: newQuantity });
};

const getIngredient = async (ingredientID) => {
    try {
        const snapshot = await db.ref(`NguyenLieu/${ingredientID}`).once('value');
        
        if (snapshot.exists()) {
            const ingredient = snapshot.val();
            return ingredient;
        } else {
            throw new Error('Nguyên liệu không tồn tại');
        }
    } catch (error) {
        console.error('Error getting ingredient:', error);
        throw error;
    }
};

const findIngredientByName = async (name) => {
    const snapshot = await db.ref('NguyenLieu').orderByChild('TenNguyenLieu').equalTo(name).once('value');
    if (snapshot.exists()) {
        const ingredients = snapshot.val();
        const firstIngredientKey = Object.keys(ingredients)[0];
        return { id: firstIngredientKey, ...ingredients[firstIngredientKey] };
    }
    return null;
};

module.exports = {
    getIngredients,
    addIngredient,
    deleteIngredient,
    getMaxIngredientId,
    updateIngredient,
    getUnits,
    updateQuantityIngredient,
    getIngredient,
    findIngredientByName
};