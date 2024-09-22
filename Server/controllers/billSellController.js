const { getMaxBillSellId, addBillSell, updateBillSell, deleteBillSell, getBillSellByTableAndStatus, updateTableBooking, mergeTables, getBills } = require('../dao/billSellDAO');
const { nextID } = require('../utils/helper');

const calculateTotalPrice = (details) => {
    details = Object.values(details);

    return details.reduce((total, detail) => {
        return total + detail.ThanhTien;
    }, 0);
};

const addBillSellHandler = async (req, res) => {
    const billSell = req.body;

    try {
        const maxBillSellId = await getMaxBillSellId();
        const newBillSellId = nextID(maxBillSellId, "HD");

        billSell.MaHoaDon = newBillSellId;

        // Calculate total price
        const totalPrice = calculateTotalPrice(billSell.ChiTietHoaDon);
                
        // Update the billSell object with the total price
        billSell.TongTien = totalPrice;

        await addBillSell(billSell);

        return res.status(201).json({ success: true, message: 'Bill Sell added successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};


const updateBillSellHandler = async (req, res) => {
    const billSell = req.body;

    try {
        // Calculate total price
        const totalPrice = calculateTotalPrice(billSell.ChiTietHoaDon);
                
        // Update the billSell object with the total price
        billSell.TongTien = totalPrice;

        await updateBillSell(billSell);

        return res.status(200).json({ success: true, message: 'Bill Sell updated successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const deleteBillSellHandler = async (req, res) => {
    const { billSellID } = req.params;

    try {
        await deleteBillSell(billSellID);

        return res.status(200).json({ success: true, message: 'BillSell deleted successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const getBillSellByTableAndStatusHandler = async (req, res) => {
    const { status, tableID } = req.query;

    try {
        const bills = await getBillSellByTableAndStatus(tableID, status);

        if (!bills) {
            return res.status(404).json({ success: false, message: 'No bills found for the given table and status' });
        }

        return res.status(200).json({ success: true, data: bills });
    } catch (error) {
        console.error("Error searching bills:", error);
        return res.status(500).json({ success: false, message: 'Server error' });
    }
};

const updateTableBookingHandler = async (req, res) => {
    const { tableID1, tableID2 } = req.body;

    try {
        await updateTableBooking(tableID1, tableID2)

        return res.status(200).json({ success: true, message: 'Bill Sell updated successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const mergeTablesHandler = async (req, res) => {
    const { tableID1, tableID2 } = req.body;

    try {
        await mergeTables(tableID1, tableID2)

        return res.status(200).json({ success: true, message: 'Bill Sell updated successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const getBillSellsHandler = async (req, res) => {
    const { fromDate, toDate } = req.query;

    try {
        const billsells = await getBills(new Date(fromDate), new Date(toDate));

        return res.status(200).json({ success: true, data: billsells });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

module.exports = {
    addBillSellHandler,
    deleteBillSellHandler,
    updateBillSellHandler,
    getBillSellByTableAndStatusHandler,
    updateTableBookingHandler,
    mergeTablesHandler,
    getBillSellsHandler
};
