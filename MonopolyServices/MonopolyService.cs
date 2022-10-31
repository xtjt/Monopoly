using MonopolyCommon.Enums;
using MonopolyEntities;

namespace MonopolyServices
{
    /// <summary>
    /// 服务类
    /// </summary>
    public class MonopolyService
    {
        /// <summary>
        /// 赛道
        /// </summary>
        private readonly static MonopolyEnum[] Track = new[] { MonopolyEnum.A, MonopolyEnum.A, MonopolyEnum.B, MonopolyEnum.A, MonopolyEnum.C, MonopolyEnum.A,
                                                               MonopolyEnum.A,MonopolyEnum.D, MonopolyEnum.A, MonopolyEnum.A, MonopolyEnum.E,MonopolyEnum.A,
                                                               MonopolyEnum.A, MonopolyEnum.A,MonopolyEnum.A, MonopolyEnum.E,MonopolyEnum.B, MonopolyEnum.A,
                                                               MonopolyEnum.C, MonopolyEnum.A };

        /// <summary>
        /// 位置信息
        /// </summary>
        private static readonly Dictionary<string, PeoplePositonModel> PeoplePositon = new() { };

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private MonopolyService() { }

        /// <summary>
        /// 单例构造
        /// </summary>
        public static readonly MonopolyService Instance = new();

        /// <summary>
        /// 获取执行结果
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, PeoplePositonModel> GetPeopleInfo() => PeoplePositon;

        public Dictionary<string, PeoplePositonModel> GetResult(List<string> randomPeople)
        {
            //赋值位置信息
            randomPeople.ForEach(item => { PeoplePositon.Add(item, new PeoplePositonModel() { Name = item }); });

            //所有人位置到达终点结束循环
            while (!PeoplePositon.Values.All(m => m.Current == Track.Length))
            {
                for (var i = 0; i < PeoplePositon.Keys.Count; i++)
                {
                    var item = PeoplePositon.ElementAt(i);
                    if (item.Value.Current == Track.Length)
                    {
                        continue;
                    }

                    //生成投掷选择数字
                    var selectNum = new Random().Next(1, 7);
                    //生成投掷数字
                    var castNum = new Random().Next(1, selectNum + 1);

                    //判断有无等待
                    var currentTrack = Track[item.Value.Current - 1];
                    if (currentTrack == MonopolyEnum.C && item.Value.NeedWait)
                    {
                        item.Value.NeedWait = false;
                        item.Value.Trajectorys.Add($@"投掷选择{selectNum}，结果{castNum}，投掷无效一次");
                        continue;
                    }

                    //获取轨迹
                    var trajectory = GetTrajectory(item.Value, castNum);

                    item.Value.Trajectorys.Add($@"投掷选择{selectNum}，结果{castNum}，{trajectory}");
                }
            }
            return PeoplePositon;
        }


        /// <summary>
        /// 获取轨迹
        /// </summary>
        /// <param name="model"></param>
        /// <param name="castNum"></param>
        /// <returns></returns>
        private string GetTrajectory(PeoplePositonModel model, int castNum = default)
        {
            //执行
            var (Trajectory, NextPosition) = ExcuteRules(model, castNum);

            //判断前进后位置有无其他人，有则踢回起始点
            if (NextPosition != Track.Length && NextPosition != 1)
            {
                //查询当前位置时候存在多个玩家
                if (PeoplePositon.Values.Count(m => m.Current == NextPosition) > 2)
                {
                    //踢出其他玩家
                    for (var j = 0; j < PeoplePositon.Keys.Count; j++)
                    {
                        var jitem = PeoplePositon.ElementAt(j);
                        if (jitem.Value.Current == NextPosition)
                        {
                            Trajectory += $@",将{jitem.Key}踢回至节点1";
                            jitem.Value.Current = 1;
                        }
                    }
                }
            }

            //连续运用规则
            var (_, IsFirst) = FindEIndex(NextPosition);
            if (Track[NextPosition - 1] == MonopolyEnum.B || Track[NextPosition - 1] == MonopolyEnum.D || (Track[NextPosition - 1] == MonopolyEnum.E && !IsFirst))
            {
                Trajectory += GetTrajectory(model);
            }

            return Trajectory;
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="model"></param>
        /// <param name="castNum">投掷数字</param>
        /// <returns>(轨迹，最终位置)</returns>
        private static (string Trajectory, int NextPosition) ExcuteRules(PeoplePositonModel model, int castNum)
        {
            var trajectory = string.Empty;
            var nextPositopn = default(int);

            //到达终点
            if (model.Current + castNum >= Track.Length)
            {
                model.Current = Track.Length;
                return ($@",前进至节点{Track.Length},到达终点", Track.Length);
            }

            var castTrack = Track[model.Current + castNum - 1];
            switch (castTrack)
            {
                case MonopolyEnum.A:
                    trajectory = $@",前进至节点{model.Current + castNum}";
                    nextPositopn = model.Current + castNum;
                    model.Current = model.Current + castNum;
                    break;
                case MonopolyEnum.B://回退一步
                    trajectory = $@",前进至节点{model.Current + castNum}，回退至节点{model.Current + castNum - 1}";
                    nextPositopn = model.Current + castNum - 1;
                    model.Current = model.Current + castNum - 1;
                    break;
                case MonopolyEnum.C://等待一轮
                    trajectory = $@",前进至节点{model.Current + castNum}";
                    nextPositopn = model.Current + castNum;
                    model.Current = model.Current + castNum;
                    model.NeedWait = true;
                    break;
                case MonopolyEnum.D://回退到起始
                    trajectory = $@",前进至节点{model.Current + castNum}，回退至节点{1}";
                    nextPositopn = 1;
                    model.Current = 1;
                    break;
                case MonopolyEnum.E://回到上一个节点，无则不处理
                    var (PositionIndex, IsFirst) = FindEIndex(model.Current + castNum);
                    trajectory = $@",{(IsFirst ? $@"前进至节点{model.Current + castNum}，" : string.Empty)}回退至节点{PositionIndex}";
                    nextPositopn = PositionIndex;
                    model.Current = PositionIndex;
                    break;
                default: break;
            }

            return (trajectory, nextPositopn);
        }

        /// <summary>
        /// 查询元素
        /// </summary>
        /// <returns></returns>
        private static (int PositionIndex, bool IsFirst) FindEIndex(int position)
        {
            //可以遍历到自己
            for (var i = 0; i < position; i++)
            {
                if (Track[i] == MonopolyEnum.E)
                {
                    return (i + 1, i + 1 == position);
                }
            }

            return default;
        }
    }
}
