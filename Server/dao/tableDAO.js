const { binarySearchTypeTable } = require('../dsa/binarySearch');
const db = require('../config/firebase');

const getTables = async () => {
    try {
        const tableResponse = await db.ref('Ban').once('value');
        const tableData = tableResponse.val();

        const typeTableResponse = await db.ref('LoaiBan').once('value');
        const typeTableData = typeTableResponse.val();

        const tableArray = tableData ? Object.values(tableData) : [];
        const typeTableArray = typeTableData ? Object.values(typeTableData) : [];
        const sizeTypeTableArray = typeTableArray.length

        const result = tableArray.map(table => {
            const index = binarySearchTypeTable(typeTableArray, sizeTypeTableArray, table.MaLoaiBan)

            return {
                ...table,
                TenLoaiBan: typeTableArray[index].TenLoaiBan,
            };
        });

        return result;
    } catch (error) {
        throw new Error('Error getting tables: ' + error.message);
    }
};

const getTypeTables = async () => {
    try {
        const snapshot = await db.ref('LoaiBan').once('value');
        const data = snapshot.val()
        return data ? Object.values(data) : [];
    } catch (error) {
        throw new Error('Error getting tables: ' + error.message);
    }
};

const checkTableName = async (currentTable) => {
    const tables = await getTables();  

    const isDuplicate = tables.some(table => 
        table.TenBan.toLowerCase() === currentTable.TenBan.toLowerCase() && 
        table.MaBan !== currentTable.MaBan
    );

    return isDuplicate;
};

const addTable = async (table) => {
    try {
        if (await checkTableName(table))
            throw new Error("Tên bàn đã tồn tại")
        else
            await db.ref(`Ban/${table.MaBan}`).set(table);
    } catch (error) {
        throw new Error('Error adding table: ' + error.message);
    }
};

const updateTable = async (table) => {
    try {
        if (await checkTableName(table))
            throw new Error("Tên bàn đã tồn tại")
        else
            await db.ref(`Ban/${table.MaBan}`).update(table);
    } catch (error) {
        throw new Error('Error updating table: ' + error.message);
    }
};

const deleteTable = async (tableID) => {
    try {
        await db.ref(`Ban/${tableID}`).remove();
    } catch (error) {
        throw new Error('Error deleting table: ' + error.message);
    }
};

const getMaxTableId = async () => {
    try {
        const snapshot = await db.ref('Ban').once('value');
        const data = snapshot.val();

        if (data) {
            const maxTableId = Object.keys(data).reduce((max, current) => current > max ? current : max, "");
            return maxTableId;
        }

        return ""
    } catch (error) {
        return ""
    }
};

const updateTableStatus = async (tableID, status) => {
    try {
        await db.ref(`Ban/${tableID}`).update({ TrangThai: status });
    } catch (error) {
        console.error("Error updating table status:", error);
        throw error;
    }
};

module.exports = {
    getTables,
    addTable,
    updateTable,
    deleteTable,
    getMaxTableId,
    getTypeTables,
    updateTableStatus
};
