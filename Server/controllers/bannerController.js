const { getBanners, addBanner, deleteBanner } = require('../dao/bannerDAO');

const getBannersHandler = async (req, res) => {
    try {
        const banners = await getBanners();

        return res.status(200).json({ success: true, data: banners });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const addBannerHandler = async (req, res) => {
    const { MaBanner, HinhAnh } = req.body;

    try {
        await addBanner(MaBanner, HinhAnh);
        return res.status(201).json({ success: true, message: 'Banner added successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const deleteBannerHandler = async (req, res) => {
    const { bannerId } = req.params;

    try {
        await deleteBanner(bannerId);
        return res.status(200).json({ success: true, message: 'Banner deleted successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const getMaxBannerIdHandler = async (req, res) => {
    try {
        const banners = await getBanners();
        if (banners) {
            const maxBannerId = Object.values(banners)
                .map(banner => banner.MaBanner)
                .reduce((max, current) => (current > max ? current : max), ""); // Assuming MaBanner is string comparable
            return res.status(200).json({ success: true, maxBannerId });
        } else {
            return res.status(200).json({ success: true, maxBannerId: null });
        }
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

module.exports = {
    getBannersHandler,
    addBannerHandler,
    deleteBannerHandler,
    getMaxBannerIdHandler
};
