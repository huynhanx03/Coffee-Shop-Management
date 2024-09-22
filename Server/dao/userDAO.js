const db = require('../config/firebase');

const addUser = async (user) => {
    await db.ref(`NguoiDung/${user.MaNguoiDung}`).set(user);
};

const deleteUser = async (userID) => {
    await db.ref(`NguoiDung/${userID}`).remove();
};

const updateUser = async (user) => {
    await db.ref(`NguoiDung/${user.MaNguoiDung}`).update(user);
};

const checkEmail = async (user) => {
    try {
        const response = await db.ref('NguoiDung').once('value');
        if (response.exists()) {
            const data = response.val();
            const users = Object.values(data);
            const existingUser = users.find(u => u.MaNguoiDung !== user.MaNguoiDung && u.Email === user.Email);
            return existingUser !== undefined;
        }

        return false;
    } catch (error) {
        console.error('Error checking email:', error);
        return false;
    }
};

const checkNumberPhone = async (user) => {
    try {
        const response = await db.ref('NguoiDung').once('value');
        if (response.exists()) {
            const data = response.val();
            const users = Object.values(data);
            const existingUser = users.find(u => u.MaNguoiDung !== user.MaNguoiDung && u.SoDienThoai === user.SoDienThoai);
            return existingUser !== undefined;
        }
        return false;
    } catch (error) {
        console.error('Error checking number phone:', error);
        return false;
    }
};

const checkIDCard = async (user) => {
    try {
        const response = await db.ref('NguoiDung').once('value');
        if (response.exists()) {
            const data = response.val();
            const users = Object.values(data);
            const existingUser = users.find(u => u.MaNguoiDung !== user.MaNguoiDung && u.CCCD_CMND === user.CCCD_CMND);
            return existingUser !== undefined;
        }

        return false;
    } catch (error) {
        console.error('Error checking ID card:', error);
        return false;
    }
};

const checkUsername = async (user) => {
    try {
        const response = await db.ref('NguoiDung').once('value');
        if (response.exists()) {
            const data = response.val();
            const users = Object.values(data);
            const existingUser = users.find(u => u.MaNguoiDung !== user.MaNguoiDung && u.TaiKhoan === user.TaiKhoan);
            return existingUser !== undefined;
        }
        return false;
    } catch (error) {
        console.error('Error checking username:', error);
        return false;
    }
};

const getUserByNumberphone = async (userNumberPhone) => {
    try {
        const response = await db.ref('NguoiDung').once('value');
        if (response.exists()) {
            const data = response.val();
            const users = Object.values(data);
            return users.find(u => u.SoDienThoai === userNumberPhone) || null;
        }
        return null;
    } catch (error) {
        console.error('Error getting user by number phone:', error);
        return null;
    }
};


module.exports = {
    addUser,
    deleteUser,
    updateUser,
    checkEmail,
    checkNumberPhone,
    getUserByNumberphone,
    checkIDCard,
    checkUsername
};