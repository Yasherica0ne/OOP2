using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcForm
{
    class Calc
    {
        public static bool EqualsH(string str1, string str2)
        {
            if (str1.Length > str2.Length) return true;
            return false;
        }
        public static bool isEquals(string str1, string str2)
        {
            if (str1.Equals(str2)) return true;
            return false;
        }
        public static string leftSh(string str1, string str2)
        {
            int shift = Convert.ToInt32(str2);
            if (shift < 1 || shift > str1.Length)
            {
                throw new Exception("Указано неверное значение сддвига");
            }
            char[] buff = str1.ToCharArray();
            for (int i = 0; i < shift; i++)
            {
                string buf = str1;
                for (int j = 0; j < str1.Length; j++)
                {
                    //
                }
            }
        }
        public static string rightSh(string str1, string str2)
        {
            int shift = Convert.ToInt32(str2);
            if (shift < 1 || shift > str1.Length)
            {
                throw new Exception("Указано неверное значение сддвига");
            }
            return str1.Remove(0, shift);
        }
    }
}
