using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class RefNo
    {

        public string getrefno()
        {
            // declare array string to generate random string with combination of small,capital letters and numbers
            char[] charArr = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            //char[] charArr = "0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new Random();
            int noofcharacters = Convert.ToInt32(12);
            for (int i = 0; i < noofcharacters; i++)
            {
                //It will not allow Repetation of Characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos).ToString()))
                    strrandom += charArr.GetValue(pos);
                else
                    i--;
            }
            return strrandom;
        }
    }
