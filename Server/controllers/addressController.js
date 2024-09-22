const db = require('../config/firebase')

const getNewId = async (userId) => {
    try {
        const addressRef = db.ref(`DiaChi/${userId}`)
        const snapshot = await addressRef.once('value')
        const addresses = snapshot.val()
        if (addresses) {
            const currentId = parseInt(Object.keys(addresses)[Object.keys(addresses).length - 1].slice(2))
            const newId = 'DC' + String(currentId + 1).padStart(4, '0')
            return newId
        } else {
            return 'DC0001'
        }
    } catch (error) {
        console.log(error)
    }
}

const addAddress = async (req, res) => {
    try {
        const { name, phone, detail_address, location } = req.body
        const userId = req.params.userId
        const newId = await getNewId(userId)

        await db.ref('DiaChi/' + userId + '/' + newId).set({
            MaDC: newId,
            HoTen: name,
            SoDienThoai: phone,
            DiaChi: detail_address + ' ' + location.address,
            latitude: location.latitude,
            longtitude: location.longtitude,
            Default: false,
        })

        return res.status(200).json({ success: true, message: 'Thêm địa chỉ thành công' })
    } catch (error) {
        return res.status(500).json({ success: false, message: 'Thêm địa chỉ thất bại' })
    }
}

const getAddresses = async (userId) => {
    try {
        const addressesSnapshot = await db.ref(`DiaChi/${userId}`).once('value')
        const addresses = addressesSnapshot.val()
        return addresses ? Object.values(addresses) : []
    } catch (error) {
        console.log(error)
    }
}

const getAddress = async (req, res) => {
    const userId = req.params.userId
    try {
        const addresses = await getAddresses(userId)

        return res.status(200).json({ success: true, data: addresses })
    } catch (error) {
        return res.status(500).json({ success: false, message: 'Lấy địa chỉ thất bại' })
    }
}

const setDefaultAddress = async (req, res) => {
    const userId = req.params.userId
    const MaDC = req.params.MaDC

    try {
        const addresses = await getAddresses(userId)
        const address = await db.ref(`DiaChi/${userId}/${MaDC}`).once('value')
        const addressData = address.val()
        if (!addressData) {
            return res.status(404).json({ message: 'Địa chỉ không tồn tại' })
        }
        
        await db.ref(`DiaChi/${userId}/${MaDC}`).update({ Default: true })

        for (const key in addresses) {
            if (key !== MaDC) {
                await db.ref(`DiaChi/${userId}/${key}`).update({ Default: false })
            }
        }

        return res.status(200).json({ success: true, message: 'Cập nhật địa chỉ mặc định thành công' })
    } catch (error) {
        return res.status(500).json({ success: false, message: 'Cập nhật địa chỉ mặc định thất bại' })
    }
}

module.exports = { addAddress, getAddress, setDefaultAddress }
