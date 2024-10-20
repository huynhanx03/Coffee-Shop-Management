const bcrypt = require('bcrypt')

function nextID(ID, key) {
  if (!ID || ID.trim() === "") {
    return `${key}0001`;
  }

  let newCodeString = `000${(parseInt(ID.slice(key.length)) + 1)}`;
  return `${key}${newCodeString.slice(-4)}`;
}

function convertToDateString(timeString) {
  return timeString.split(" ")[1];
}

function convertToDateTime(timeString) {
  const [time, date] = timeString.split(" ");
  const [hours, minutes, seconds] = time.split(":").map(Number);
  const [day, month, year] = date.split("/").map(Number);
  return new Date(year, month - 1, day, hours, minutes, seconds);
}

const convertUnit = (ingredientID, quantity, type) => {
  // Kg -> 1000g
  // l -> 1000ml

  if (ingredientID === "DV0001" || ingredientID === "DV0003") {
    if (quantity >= 1) {
      switch (type) {
        case 1:
          return ingredientID;
        case 2:
          return quantity;
        default:
          return null;
      }
    } else {
      switch (type) {
        case 1:
          switch (ingredientID) {
            case "DV0001":
              return "DV0002";
            case "DV0003":
              return "DV0004";
            default:
              return ingredientID;
          }
        case 2:
          return quantity * 1000;
        default:
          return quantity;
      }
    }
  } else {
    if (quantity < 1000) {
      switch (type) {
        case 1:
          return ingredientID;
        case 2:
          return quantity;
        default:
          return null;
      }
    } else {
      switch (type) {
        case 1:
          switch (ingredientID) {
            case "DV0002":
              return "DV0001";
            case "DV0004":
              return "DV0003";
            default:
              return ingredientID;
          }
        case 2:
          return quantity / 1000;
        default:
          return quantity;
      }
    }
  }
};

const optionsDateTime = {
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
  second: '2-digit',
  hour12: false
};

const verifyPassword = async (password, hash) => {
  try {
      return await bcrypt.compare(password, hash)
  } catch (error) {
      throw error
  }
}

const hashPassword = async (password) => {
  try {
      const saltRound = 10
      const hash = await bcrypt.hash(password, saltRound)
      return hash
  } catch (error) {
      throw error
  }
}

module.exports = {
  nextID,
  convertToDateString,
  convertToDateTime,
  convertUnit,
  optionsDateTime,
  verifyPassword,
  hashPassword
};