const tableDAO = require('../dao/tableDAO');
const { nextID } = require('../utils/helper');

const getTablesHandler = async (req, res) => {
    try {
        const tables = await tableDAO.getTables();
        return res.status(200).json({ success: true, data: tables });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const getTypeTablesHandler = async (req, res) => {
    try {
        const tables = await tableDAO.getTypeTables();
        return res.status(200).json({ success: true, data: tables });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const addTableHandler = async (req, res) => {
    const table = req.body;

    try {
        const existingTable = await tableDAO.findTableByName(table.TenBan);
        if (existingTable) {
            return res.status(400).json({ success: false, message: 'Table name already exists' });
        }

        const maxTableId = await tableDAO.getMaxTableId();
        const newTableId = nextID(maxTableId, "BA");
        table.MaBan = newTableId;

        await tableDAO.addTable(table);
        return res.status(201).json({ success: true, message: 'Table added successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const updateTableHandler = async (req, res) => {
    const table = req.body;
    const { tableID } = req.params;

    try {
        const existingTable = await tableDAO.findTableByName(table.TenBan);
        if (existingTable && existingTable.MaBan !== tableID) {
            return res.status(400).json({ success: false, message: 'Table name already exists' });
        }

        table.MaBan = tableID

        await tableDAO.updateTable(table);
        return res.status(200).json({ success: true, message: 'Table updated successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const deleteTableHandler = async (req, res) => {
    const { tableID } = req.params;

    try {
        await tableDAO.deleteTable(tableID);
        return res.status(200).json({ success: true, message: 'Table deleted successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const updateTableStatusHandler = async (req, res) => {
    const { tableID } = req.params;
    const { status } = req.body;

    try {
        await tableDAO.updateTableStatus(tableID, status);
        return res.status(200).json({ success: true, message: 'Table status updated successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

module.exports = {
    getTablesHandler,
    addTableHandler,
    updateTableHandler,
    deleteTableHandler,
    getTypeTablesHandler,
    updateTableStatusHandler
};
