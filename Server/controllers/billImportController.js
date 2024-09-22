const { getMaxBillImportId, addBillImport, deleteBillImport, getImportBills } = require('../dao/billImportDAO');
const { nextID } = require('../utils/helper');

const calculateTotalPrice = (details) => {
    details = Object.values(details)

    return details.reduce((total, detail) => {
        return total + detail.Gia;
    }, 0);
};

const addBillImportHandler = async (req, res) => {
    const billImport = req.body;

    try {
        const maxBillImportId = await getMaxBillImportId();
        const newBillImportId = nextID(maxBillImportId, "NK");

        billImport.MaPhieuNhapKho = newBillImportId;

        // Calculate total price
        const totalPrice = calculateTotalPrice(billImport.ChiTietPhieuNhapKho);
                
        // Update the billImport object with the total price
        billImport.TongTien = totalPrice;

        await addBillImport(billImport);

        return res.status(201).json({ success: true, message: 'BillImport added successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const deleteBillImportHandler = async (req, res) => {
    const { billImportID } = req.params;

    try {
        await deleteBillImport(billImportID);

        return res.status(200).json({ success: true, message: 'BillImport deleted successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const getImportBillsHandler = async (req, res) => {
    const { fromDate, toDate } = req.query;

    try {
        const importBills = await getImportBills(new Date(fromDate), new Date(toDate));

        return res.status(200).json({ success: true, data: importBills });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

module.exports = {
    addBillImportHandler,
    deleteBillImportHandler,
    getImportBillsHandler
};
