const express = require('express');
const router = express.Router();
const employeePositionController = require('../controllers/employeePositionController');
const employeeController = require('../controllers/employeeController')
const authenticateToken = require('../middleware/authMiddleware');

router.get('/positions', authenticateToken, employeePositionController.getEmployeePositionHandler);
router.get('/employees', authenticateToken, employeeController.getEmployeesHandle)
router.post('/employee', authenticateToken, employeeController.addEmployeeHandler)
router.put('/employee/:employeeID', authenticateToken, employeeController.updateEmployeeHandler)
router.delete('/employee/:employeeID', authenticateToken, employeeController.deleteEmployeeHandler)

module.exports = router