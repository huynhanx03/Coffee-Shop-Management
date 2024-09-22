const express = require('express');
const router = express.Router();
const bannerController = require('../controllers/bannerController');
const authenticateToken = require('../middleware/authMiddleware');

router.get('/banners', bannerController.getBannersHandler);
router.post('/banner', authenticateToken, bannerController.addBannerHandler);
router.delete('/banners/:bannerId', authenticateToken, bannerController.deleteBannerHandler);
router.get('/banners/maxBannerId', bannerController.getMaxBannerIdHandler);

module.exports = router;