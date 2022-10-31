using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyCommon.Services
{
    public class CommonService
    {
        /// <summary>
        /// 校验数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumeric(string str)
        {
            if (str == null || str.Length == 0)
                return false;
            foreach (char c in str)
            {
                if (!Char.IsNumber(c))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 随机顺序
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<string> GetRandom(List<string> input)
        {
            var templeInput = new List<string> { };
            input.ForEach(m => { templeInput.Add(m); });
            var result = new List<string>();

            while (templeInput.Count > 0)
            {
                var item = templeInput.ElementAt(new Random().Next(templeInput.Count));
                result.Add(item);
                templeInput.Remove(item);
            }
            return result;
        }

        /// <summary>
        /// 生成人员名称
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static List<string> GetPeopleName(int num)
        {
            string[] people = new[] { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };
            if (num > people.Length)
            {
                return new List<string>();
            }

            var result = new List<string>();
            for (var i = 0; i < num; i++)
            {
                result.Add(people[i]);
            }

            return result;
        }

    }
}
