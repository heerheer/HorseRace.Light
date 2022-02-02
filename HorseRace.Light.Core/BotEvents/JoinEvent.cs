using System;
using System.Linq;
using System.Text;
using Amiable.SDK.Enum;
using Amiable.SDK.EventArgs;
using Amiable.SDK.Interface;

namespace Amiable.Core.BotEvents
{
    public class JoinEvent : IPluginEvent
    {
        public AmiableEventType EventType => AmiableEventType.Group;

        public async void Process(AmiableEventArgs e)
        {
            var msg = e as AmiableMessageEventArgs;
            if (msg != null && msg.RawMessage.StartsWith("/签订赛马契约"))
            {
                var _name = msg.RawMessage.Substring(7).Trim();
                var name = string.IsNullOrEmpty(_name) ? "野生马" : _name;
                try
                {
                    var sb = new StringBuilder();
                    var group = msg.GroupId.ToString();
                    var user = msg.UserId.ToString();
                    sb.AppendLine($"[@{user}]");
                    var race = HorseRaceLight.GetRace(group);
                    var logs = await race.JoinRace(user, name);
                    sb.AppendLine($"契约赛马场:加入赛马成功!");


                    logs.ForEach(x => sb.AppendLine(x));

                    msg.SendMessage(sb.ToString());
                }
                catch (Exception exception)
                {
                    msg.SendMessage(exception.Message);
                }
            }
        }
    }
}