const db = require('../config/firebase');

const getBanners = async () => {
    const bannersSnapshot = await db.ref('Banner/').once('value');
    return Object.values(bannersSnapshot.val());
};

const addBanner = async (bannerId, imageUrl) => {
    const bannerData = {
        MaBanner: bannerId,
        HinhAnh: imageUrl,
    };
    await db.ref(`Banner/${bannerId}`).set(bannerData);
};

const deleteBanner = async (bannerId) => {
    await db.ref(`Banner/${bannerId}`).remove();
};

module.exports = {
    getBanners,
    addBanner,
    deleteBanner,
};
