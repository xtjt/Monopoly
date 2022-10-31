using MonopolyCommon.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServices
{
    public class InputService
    {
        /// <summary>
        /// 获取选手人数
        /// </summary>
        /// <param name="repeat"></param>
        /// <returns></returns>
        public int GetPeopleCount(bool repeat = default)
        {
            Console.WriteLine(repeat ? "选手人数输入有误，请重新输入：" : "请输入选手人数：");
            var sum = Console.ReadLine() ?? string.Empty;
            if (!CommonService.IsNumeric(sum))
            {
                return GetPeopleCount(true);
            }

            return Convert.ToInt32(sum);
        }

    }
}
