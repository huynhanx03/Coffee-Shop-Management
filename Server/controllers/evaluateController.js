const { getEvaluates, deleteEvaluate } = require('../dao/evaluateDAO');

const getEvaluatesHandler = async (req, res) => {
    try {
        const evaluates = await getEvaluates();

        return res.status(200).json({ success: true, data: evaluates });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

const deleteEvaluateHandler = async (req, res) => {
    const { evaluateID } = req.params;

    try {
        await deleteEvaluate(evaluateID);
        return res.status(200).json({ success: true, message: 'Evaluate deleted successfully' });
    } catch (error) {
        return res.status(500).json({ success: false, message: error.message });
    }
};

module.exports = {
    getEvaluatesHandler,
    deleteEvaluateHandler
};
