const db = require('../config/firebase');
const { convertToDateString } = require('../utils/helper');
const { binarySearchUser } = require('../dsa/binarySearch');
const { nextID, convertToDateTime } = require('../utils/helper');

const getUserContacts = async (prefix) => {
    try {
        const messageSnapshot = await db.ref('TinNhan').once('value');
        const messageData = messageSnapshot.val();

        if (!messageData) {
            return [];
        }

        const userSnapshot = await db.ref('NguoiDung').once('value');
        const userData = userSnapshot.val();
        const listUser = Object.values(userData);
        const sizeUsers = listUser.length;

        const listUserContact = Object.entries(messageData)
            .filter(([key]) => key.startsWith(`${prefix}-`))
            .map(([key, data]) => {
                const customerId = key.split('-')[1];
                const user = listUser.find(x => x.MaNguoiDung === customerId);

                if (user) {
                    return {
                        MaKhachHang: user.MaNguoiDung,
                        HoTen: user.HoTen,
                        HinhAnh: user.HinhAnh,
                    };
                }
                return null;
            })
            .filter(Boolean);

        return listUserContact;
    } catch (error) {
        console.error('Lỗi khi lấy danh sách User Contact:', error);
        return [];
    }
};

const getMaxMessageId = async (userID) => {
    try {
        const snapshot = await db.ref(`TinNhan/${userID}`).once('value');
        const data = snapshot.val();

        if (data) {
            const maxMessageID = Object.keys(data).reduce((max, current) => current > max ? current : max, "");
            return maxMessageID;
        }

        return ""
    } catch (error) {
        return ""
    }
};

const addMessage = async (message, userID) => {
    // maxMessageID = await getMaxMessageId(userID)

    // const newMessageID = nextID(maxMessageID, "TN")

    const key = 'ND0001' + '-' + userID
    const snapshot = await db.ref('TinNhan/' + key).once('value')
    const chat = snapshot.val()

    if (!chat) {
        throw new Error('Không tìm thấy cuộc trò chuyện!')
    }

    const newChatId = chat.NoiDung ? Object.keys(chat.NoiDung).length : 0

    await db.ref(`TinNhan/${key}/NoiDung/${newChatId}`).set(message);
};

const getMessages = async (userID, datetime) => {
    const messagesSnapshot = await db.ref(`TinNhan/ND0001-${userID}/NoiDung`).once('value');

    messages = messagesSnapshot.val()

    listMessage = Object.values(messages)

    if (datetime) {
        const listMessageBytime = []

        for (const mess of listMessage) {
            if (convertToDateTime(mess.ThoiGian) > datetime) {
                listMessageBytime.push(mess)
            }
        }

        return listMessageBytime;
    }

    return listMessage
};

module.exports = {
    getUserContacts,
    addMessage,
    getMessages
};