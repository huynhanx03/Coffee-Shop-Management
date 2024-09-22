const { getRankCustomers, getCustomerIDByRankMinimum } = require('../dao/rankDAO');
const { binarySearchRankCustomer } = require('../dsa/binarySearch');
const { nextID } = require('../utils/helper');

const db = require('../config/firebase');

const getVouchers = async () => {
    const vouchersSnapshot = await db.ref('PhieuGiamGia/').once('value');

    vouchers = vouchersSnapshot.val()

    if (vouchers) {
        rankCustomers = await getRankCustomers()
        sizeRankCustomers = rankCustomers.length

        for (const voucherKey in vouchers) {
            const voucher = vouchers[voucherKey];

            const findIndexRankCustomer = binarySearchRankCustomer(rankCustomers, sizeRankCustomers, voucher.HangToiThieu);

            if (findIndexRankCustomer === -1) {
                voucher.TenHangToiThieu = "Không có";
            } else {
                voucher.TenHangToiThieu = rankCustomers[findIndexRankCustomer].TenMucDoThanThiet;
            }
        }
    }
    else
        return []

    return vouchers ? Object.values(vouchers) : [];
};

const checkVoucherContext = async (currentVoucher) => {
    const vouchers = await getVouchers();  

    const isDuplicate = vouchers.some(voucher => 
        voucher.NoiDung.toLowerCase() === currentVoucher.NoiDung.toLowerCase() && 
        voucher.MaPhieuGiamGia !== currentVoucher.MaPhieuGiamGia
    );

    return isDuplicate;
};

const addVoucher = async (voucher) => {
    if (await checkVoucherContext(table))
        throw new Error("Nội dung phiếu giảm giá đã tồn tại")
    else {
        if (!(voucher.MaPhieuGiamGia)) {
            maxVoucherId = await getMaxVoucherId()
    
            voucher.MaPhieuGiamGia = nextID(maxVoucherId, "VC")
        }
    
        await db.ref(`PhieuGiamGia/${voucher.MaPhieuGiamGia}`).set(voucher);
    
        customerIDs = await getCustomerIDByRankMinimum(voucher.HangToiThieu)
    
        await createDetailVoucher(voucher.MaPhieuGiamGia, customerIDs)
    }
};

const createDetailVoucher = async (voucherID, customerIDList) => {
    try {
        const detailVoucher = {
            MaPhieuGiamGia: voucherID,
            TrangThai: "Chưa sử dụng"
        };

        for (const customerID of customerIDList) {
            detailVoucher.MaKhachHang = customerID;
            await db.ref(`PhieuGiamGia/${voucherID}/ChiTiet/${customerID}`).set(detailVoucher);
        }

        return ("Thêm phiếu giảm giá thành công", true);
    } catch (error) {
        return (error.message, false);
    }
}

const deleteVoucher = async (voucherId) => {
    await db.ref(`PhieuGiamGia/${voucherId}`).remove();
};

const getMaxVoucherId = async () => {
    const vouchers = await getVouchers();
    return vouchers.length > 0 
        ? vouchers.map(voucher => voucher.MaPhieuGiamGia).reduce((max, current) => (current > max ? current : max), "")
        : null;
};

module.exports = {
    getVouchers,
    addVoucher,
    deleteVoucher,
    getMaxVoucherId
};
