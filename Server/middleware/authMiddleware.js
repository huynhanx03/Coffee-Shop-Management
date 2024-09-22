const jwt = require('jsonwebtoken');
const dotenv = require('dotenv');

dotenv.config();

const authenticateToken = (req, res, next) => {
    const authHeader = req.headers['authorization'];
    const token = authHeader && authHeader.split(' ')[1];

    if (token == null) return res.status(401).json({ success: false, message: 'Token không được cung cấp'})

    jwt.verify(token, process.env.SECRET_KEY, (err, user) => {
        if (err) {
            if (err.name === 'TokenExpiredError') {
                return res.status(401).json({ success: false, message: 'Token đã hết hạn'})
            }
            return res.status(403).json({ success: false, message: 'Token không hợp lệ'})
        }
        
        req.user = user
        next()
    })
};

module.exports = authenticateToken;