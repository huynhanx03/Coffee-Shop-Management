const db = require('../config/firebase');

const getPositionEmployees = async () => {
    const positionEmployeesSnapshot = await db.ref('ChucDanh').once('value');
    const PositionEmployeesData = positionEmployeesSnapshot.val();

    return PositionEmployeesData ? Object.values(PositionEmployeesData) : []
}

module.exports = {
    getPositionEmployees
};