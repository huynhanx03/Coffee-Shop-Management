const db = require('../config/firebase');

const addCustomerRank = async (customerID, detailRank) => {
    await db.ref(`ChiTietMucDoThanThiet/${customerID}/ChiTiet/${detailRank.MaMucDoThanThiet}`).set(detailRank);
};

const addInitialCustomerRank = async (customerID) => {
    await db.ref(`ChiTietMucDoThanThiet/${customerID}`).update({ MaKhachHang: customerID });
};

const deleteDetailRank = async (userID) => {
    await db.ref(`ChiTietMucDoThanThiet/${userID}`).remove();
};

const getRankCustomers = async () => {
    const rankCustomersSnapshot = await db.ref('MucDoThanThiet/').once('value');

    rankCustomers = rankCustomersSnapshot.val()

    return rankCustomers ? Object.values(rankCustomers) : [];
};

const getCustomerIDByRankMinimum = async (rankID) => {
    try {
        const context = db.ref('ChiTietMucDoThanThiet/');
        const snapshot = await context.once('value');
        const customerData = snapshot.val();

        if (!customerData) {
            // throw new Error('No customer data found');
        }

        const customerIDList = Object.keys(customerData);
        const result = [];

        for (const customerID of customerIDList) {
            const detailRankSnapshot = await db.ref(`ChiTietMucDoThanThiet/${customerID}/ChiTiet`).once('value');
            const detailRankData = detailRankSnapshot.val();

            if (!detailRankData) {
                continue;
            }

            const detailRankList = Object.values(detailRankData);
            const lastDetailRank = detailRankList[detailRankList.length - 1];

            if (lastDetailRank.MaMucDoThanThiet >= rankID) {
                result.push(customerID);
            }
        }

        return result;
    } catch (error) {
        return null;
    }
}

const getRank = async (rankID) => {
    try {
        const response = await db.ref(`MucDoThanThiet/${rankID}`).once('value');
        const rank = response.val();
    
        return rank
    } catch (error) {
        return null
    }
}

module.exports = {
    addCustomerRank,
    addInitialCustomerRank,
    deleteDetailRank,
    getRankCustomers,
    getCustomerIDByRankMinimum,
    getRank
};