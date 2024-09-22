const { body, validationResult } = require('express-validator');
const db = require('../config/firebase');

const registerValidationRules = () => {
    return [
        body('username').custom(async (value) => {
            const snapshot = await db.ref('NguoiDung/').orderByChild('TaiKhoan').equalTo(value).once('value');
            const userData = snapshot.val();

            if (userData) {
                return Promise.reject('Tên đăng nhập đã tồn tại');
            }
        }),
        body('email')
            .isEmail()
            .withMessage('Email không hợp lệ')
            .normalizeEmail()
            .custom(async (value) => {
                const snapshot = await db.ref('NguoiDung/').orderByChild('Email').equalTo(value).once('value');
                const userData = snapshot.val();

                if (userData) {
                    return Promise.reject('Email đã tồn tại');
                }
            }),
        body('password').isLength({ min: 8 }).withMessage('Mật khẩu phải có ít nhất 8 ký tự')
    ];
};

const validate = (req, res, next) => {
    const errors = validationResult(req);
    if (errors.isEmpty()) {
        return next();
    }
    return res.status(422).json({ errors: errors.array() });
};

module.exports = {
    registerValidationRules,
    validate,
};
