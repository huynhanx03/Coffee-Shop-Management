using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Coffee.Utils.Helper
{
    public class Helper
    {
        /// <summary>
        /// Tăng ID của phần tử lên 1
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string nextID(string ID, string key)
        {
            if (ID is null || string.IsNullOrEmpty(ID))
                return key + "0001";

            string newCodeString = $"000{int.Parse(ID.Substring(key.Length)) + 1}";
            return key + newCodeString.Substring(newCodeString.Length - 4, 4);
        }

        /// <summary>
        /// Kiểm tra định dạng số điện thoại
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool checkPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^0\d{9}$");
        }

        /// <summary>
        /// Kiểm tra định dạng email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool checkEmail(string email)
        {
            return Regex.IsMatch(email, @"^([\w\.-]+)@([\w-]+)((\.(\w){2,3})+)$");
        }

        /// <summary>
        /// Kiểm tra định dạng CCCD/CMND
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public static bool checkCardID(string cardID)
        {
            return Regex.IsMatch(cardID, @"^\d{9}$|^\d{12}$");
        }

        /// <summary>
        /// Mã hoá mật khẩu
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Hash(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(password));
            //get hash result after compute it  
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

        public static object converntUnit(string IngredientID, double quantity, int type)
        {
            // Kg -> 1000g

            // l -> 1000ml

            if (IngredientID == "DV0001" || IngredientID == "DV0003")
            {
                if (quantity >= 1)
                {
                    switch (type)
                    {
                        case 1:
                            return IngredientID;

                        case 2:
                            return quantity;
                    }

                    return null;
                }
                else
                {
                    switch(type)
                    {
                        case 1:
                            switch (IngredientID)
                            {
                                case "DV0001":
                                    return "DV0002";

                                case "DV0003":
                                    return "DV0004";

                            }
                            break;

                        case 2:
                            return quantity * 1000;
                    }

                    return null;
                }
            }
            else
            {
                if (quantity < 1000)
                {
                    switch (type)
                    {
                        case 1:
                            return IngredientID;

                        case 2:
                            return quantity;
                    }

                    return null;
                }
                else
                {
                    switch (type)
                    {
                        case 1:
                            switch (IngredientID)
                            {
                                case "DV0002":
                                    return "DV0001";

                                case "DV0004":
                                    return "DV0003";

                            }
                            break;

                        case 2:
                            return quantity / 1000;
                    }

                    return null;
                }
            }
        }

        public static string getToken()
        {
            return Properties.Settings.Default.Token;
        }

        public static string getUserID()
        {
            return Properties.Settings.Default.UserID;
        }
    }
}
