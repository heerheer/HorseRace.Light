using System;
using System.Collections.Generic;
using System.Text;
using Amiable.SDK.Enum;
using Amiable.SDK.EventArgs;
using Amiable.SDK.Interface;

namespace Amiable.Core.BotEvents
{
    public class CreateRaceEvent : IPluginEvent
    {
        public AmiableEventType EventType => AmiableEventType.Group;

        public async void Process(AmiableEventArgs e)
        {
            var msg = e as AmiableMessageEventArgs;
            if (msg != null && msg.RawMessage == "/创建契约赛马场")
            {
                try
                {
                    var sb = new StringBuilder();
                    var group = msg.GroupId.ToString();
                    if (HorseRaceLight.CanCreate(group))
                    {
                        var api = new RaceApiUtil();
                        var createInfo = await api.CreateRace();

                        HorseRaceLight.AddRace(group, api);
                        sb.AppendLine("赛马场地已创建!");
                        sb.AppendLine($"场地ID:{createInfo.raceId}");
                    }
                    else
                    {
                        sb.AppendLine("赛马场地无法创建!");
                    }

                    msg.SendMessage(sb.ToString());
                }
                catch (Exception exception)
                {
                    msg.SendMessage(exception.Message);
                }
            }
        }
    }

    public class HorseRaceLight
    {
        public static Dictionary<string, RaceApiUtil> RaceApiUtils = new();

        public static void AddRace(string group, RaceApiUtil api)
        {
            if (RaceApiUtils.ContainsKey(group))
            {
                RaceApiUtils.Remove(group);
            }

            RaceApiUtils.Add(group, api);
        }

        public static void DeleteRace(string group)
        {
            if (RaceApiUtils.ContainsKey(group))
            {
                RaceApiUtils.Remove(group);
            }
        }

        public static bool CanCreate(string group)
        {
            if (!RaceApiUtils.ContainsKey(group))
                return true;

            if (RaceApiUtils[group].IsEnd())
                return true;

            return false;
        }

        public static RaceApiUtil GetRace(string group)
        {
            if (!RaceApiUtils.ContainsKey(group))
            {
                throw new("比赛不存在，请先创建");
            }

            return RaceApiUtils[group];
        }
    }
}