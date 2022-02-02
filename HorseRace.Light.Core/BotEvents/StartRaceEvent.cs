using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amiable.SDK.Enum;
using Amiable.SDK.EventArgs;
using Amiable.SDK.Interface;
using Amiable.SDK.Wrapper;

namespace Amiable.Core.BotEvents
{
    public class StartRaceEvent : IPluginEvent

    {
        public AmiableEventType EventType => AmiableEventType.Group;

        public async void Process(AmiableEventArgs e)
        {
            var msg = e as AmiableMessageEventArgs;
            if (msg != null && msg.RawMessage.StartsWith("/开始赛马"))
            {
                try
                {
                    var sb = new StringBuilder();
                    var group = msg.GroupId.ToString();
                    var race = HorseRaceLight.GetRace(group);

                    var startInfo = await race.StartRace();
                    sb.AppendLine($"比赛开始!!");
                    sb.AppendLine($"赛场选手数量:{startInfo.Data.Horses.Count}");
                    msg.SendMessage(sb.ToString());


                    await RaceTask.Create(group, msg.ApiWrapper);
                }
                catch (Exception exception)
                {
                    msg.SendMessage(exception.Message);
                }
            }
        }
    }

    public class RaceTask
    {
        public static Task Create(string group, IApiWrapper msgApiWrapper)
        {
            return Task.Factory.StartNew(() =>
            {
                var race = HorseRaceLight.GetRace(group);

                while (!race.IsEnd())
                {
                    var data = race.Next().Result;

                    var sb = new StringBuilder();
                    data.Logs.ForEach(x => sb.AppendLine(x));

                    data.Race.Horses.ForEach(x =>
                    {
                        //TODO 这边可能需要替换后缀什么的
                        //TODO 到时候把赛场的具体文本生成丢给API
                        var line = new string('=', data.Race.Distance - x.Step) + x.Display + new string('=', x.Step);
                        sb.AppendLine($"[{(x.StepChange >= 0 ? "+" : "")}{x.StepChange}]{line}");
                    });
                    msgApiWrapper.SendGroupMessage(group, sb.ToString());
                    Thread.Sleep(8000);
                }
            }).ContinueWith(
                (obj) =>
                {
                    if (obj.Exception != null)
                        if (obj.Exception.InnerException != null)
                            msgApiWrapper.SendGroupMessage(@group,
                                "[比赛出现异常]" + obj.Exception.InnerException.Message.ToString());
                }
                , TaskContinuationOptions.OnlyOnFaulted).ContinueWith(t =>
            {
                //等待任务完成就发送结束
                var race = HorseRaceLight.GetRace(group);

                var sb = new StringBuilder();

                var first = race.EndRace.Horses.OrderByDescending(x => x.Step).First();

                sb.AppendLine("比赛结束!");
                sb.AppendLine($"获胜者是:{first.Display}!!!");
                sb.AppendLine($"恭喜这位同学!![@{first.UserId}]!!!");
                sb.AppendLine($"当前场次的比赛场地已关闭!");
                msgApiWrapper.SendGroupMessage(group, sb.ToString());
                HorseRaceLight.DeleteRace(group);
            });
        }
    }
}