using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DAM.Models.Salt_MH
{
    public class MH_GM
    {
        public string MH(string code)
        {
            byte[] ascii = Encoding.ASCII.GetBytes(code);
            string mh1 = keyMH(ascii);
            return mh1;
        }

        public string GM(string code)
        {
            string[] asciiValues1 = code.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] bytes1 = new byte[asciiValues1.Length];

            for (int i = 0; i < asciiValues1.Length; i++)
            {
                if (int.TryParse(asciiValues1[i], out int x))
                {
                    bytes1[i] = (byte)x;
                }
                else
                {
                    return "Invalid ASCII value: " + asciiValues1[i];
                }
            }
            string gm1 = keyGM(bytes1);
            string[] asciiValues2 = gm1.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] bytes2 = new byte[asciiValues2.Length];

            for (int i = 0; i < asciiValues2.Length; i++)
            {
                if (int.TryParse(asciiValues2[i], out int x))
                {
                    bytes2[i] = (byte)x;
                }
                else
                {
                    return "Invalid ASCII value: " + asciiValues2[i];
                }
            }
            return Encoding.ASCII.GetString(bytes2);
        }

        public string keyGM(byte[] ascii)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < ascii.Length; i++)
            {
                if (i % 2 == 0)
                {
                    result.Append((ascii[i] - 3) / 2 + " ");
                }
                else
                {
                    result.Append(ascii[i] + 23 + " ");
                }
            }
            return result.ToString();
        }
        public string keyMH(byte[] ascii)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < ascii.Length; i++)
            {
                if (i % 2 == 0)
                {
                    result.Append(ascii[i] * 2 + 3 + " ");
                }
                else
                {
                    result.Append(ascii[i] - 23 + " ");
                }
            }
            return result.ToString();
        }
    }
}