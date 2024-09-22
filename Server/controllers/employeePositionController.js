const { getPositionEmployees } = require("../dao/employeePositionDAO")

const getEmployeePositionHandler = async (req, res) => {
    try {
        const EmployeePositiones = await getPositionEmployees()

        return res.status(200).json({ success: true, data: EmployeePositiones })
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message })
    }
}

module.exports = {
    getEmployeePositionHandler
};