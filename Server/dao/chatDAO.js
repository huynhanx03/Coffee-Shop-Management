const db = require('../config/firebase');
const { convertToDateString } = require('../utils/helper');
const { binarySearchUser } = require('../dsa/binarySearch');
const { nextID, convertToDateTime } = require('../utils/helper');

const getUserContacts = async () => {
    const messageSnapshot = await db.ref('TinNhan').once('value');
    const messageData = messageSnapshot.val();

    if (!messageData) {
        return [];
    }

    const userSnapshot = await db.ref('NguoiDung').once('value');
    const userData = userSnapshot.val();
    const listUser = Object.values(userData);
    const sizeUsers = listUser.length
    const listCustomerId = Object.keys(messageData);

    const listUserContact = [];

    for (const customerId of listCustomerId) {
        const chatSnapshot = await db.ref(`TinNhan/${customerId}`).once('value');
        const chatData = chatSnapshot.val();
        const listChat = Object.values(chatData);

        const lastChat = listChat.reverse().find(x => x.MaKH);
        // const lastChat = "";

        const findIndexCustomer = binarySearchUser(listUser, sizeUsers, customerId);

        const user = listUser[findIndexCustomer]
        // const user = listUser.find(x => x.MaNguoiDung === customerId);

        const userContact = {
            MaKhachHang: user.MaNguoiDung,
            HoTen: user.HoTen,
            HinhAnh: user.HinhAnh,
            ThoiGianTinNhanCuoiCung: convertToDateString(lastChat.ThoiGian),
            TinNhanCuoiCung: lastChat.NoiDung
        };

        listUserContact.push(userContact);
    }

    return listUserContact;
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
    maxMessageID = await getMaxMessageId(userID)

    const newMessageID = nextID(maxMessageID, "TN")

    await db.ref(`TinNhan/${userID}/${newMessageID}`).set(message);
};

const getMessages = async (userID, datetime) => {
    const messagesSnapshot = await db.ref(`TinNhan/${userID}`).once('value');

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