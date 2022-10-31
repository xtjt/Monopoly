using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyEntities
{
    public class PeoplePositonModel
    {
        /// <summary>
        /// 人名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 当前位置(索引+1)
        /// </summary>
        public int Current { get; set; } = 1;

        /// <summary>
        /// 总共投掷次数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 对于规则C，是否有等待（可以抽象个类用于特殊化处理）
        /// </summary>
        public bool NeedWait { get; set; }

        /// <summary>
        /// 轨迹
        /// </summary>
        public List<string> Trajectorys { get; set; } = new List<string>();
    }
}
