// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using MonopolyCommon.Services;
using MonopolyServices;


//获取选手人数
var peopleCount = new InputService().GetPeopleCount();
//转换为名称--根据人数生成名称
var peopleName = CommonService.GetPeopleName(peopleCount);
if (peopleName == null || peopleName.Count <= 0)
{
    Console.WriteLine($@"人数超过支持限制，请重试");
    return;
}
//随机出投掷顺序
var randomPeople = CommonService.GetRandom(peopleName);
//执行
var getResult = MonopolyService.Instance.GetResult(randomPeople);
//轨迹记录
var peopleInfo = MonopolyService.GetPeopleInfo();
//按照要求进行输出
for (var i = 0; i < peopleName.Count; i++)
{
    var item = peopleInfo.Keys.Where(m => m == peopleName[i]).FirstOrDefault();
    if (item == null)
    {
        continue;
    }

    Console.WriteLine($@"{item}:投掷次数 {peopleInfo[item].Trajectorys.Count}");
}

Console.WriteLine("\r\n");

Console.WriteLine("投掷和行走结果：");
var sortTips = "0:";
for (var i = 0; i < peopleName.Count; i++)
{
    sortTips += $@"{peopleName[i]}在节点1，";
}
sortTips += $@"投掷顺序({string.Join(",", peopleInfo.Keys)})";
Console.WriteLine(sortTips);
for (var i = 0; i < peopleInfo.Values.Max(m => m.Trajectorys.Count); i++)
{
    Console.WriteLine($@"第{i+1}轮：");
    for (var j = 0; j < peopleName.Count;j++)
    {
        var item = peopleInfo.Keys.Where(m => m == peopleName[j]).FirstOrDefault();
        if (item == null)
        {
            continue;
        }

        Console.WriteLine($@"{item}:{(i> peopleInfo[item].Trajectorys.Count - 1 ? "已经到达终点" : peopleInfo[item].Trajectorys[i])}");
    }
}

Console.WriteLine("\r\n");
//最小轮次
var min = peopleInfo.Values.Min(m=>m.Trajectorys.Count);
Console.WriteLine($@"获胜选手：{string.Join(",", peopleInfo.Values.Where(m => m.Trajectorys.Count == min).Select(m => m.Name))}");
Console.ReadLine();
