const db = require('../config/firebase');

const deleteAddress = async (userID) => {
    await db.ref(`DiaChi/${userID}`).remove();
};

module.exports = {
    deleteAddress
};