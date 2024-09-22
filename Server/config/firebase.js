var admin = require("firebase-admin");

var serviceAccount = require("../serviceAccountKey.json");

admin.initializeApp({
  credential: admin.credential.cert(serviceAccount),
  databaseURL: "https://coffee-4053c-default-rtdb.asia-southeast1.firebasedatabase.app"
});

module.exports = admin.database();