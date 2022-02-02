using Amiable.Adapter.Kum;
using Amiable.Adapter.MQ;
using Amiable.SDK;
using Amiable.SDK.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Amiable.Adapters.Kum;
using Amiable.Adapters.LYP;
using Amiable.Adapters.MQ;
using Amiable.Adapters.Xlz;
using Amiable.Adapters.XQ;
using Amiable.Core.Adapters.Ono;
using Amiable.SDK.Enum;
using Amiable.SDK.EventArgs;
using System.Text;
using Amiable.Core.Adapters.Mini;
using Amiable.Core.BotEvents;

namespace Amiable.Core
{
    public static partial class AmiableService
    {
        /// <summary>
        /// 设置App信息
        /// </summary>
        public static void SetAppInfo()
        {
            App.AppInfo = new AppInfo
            {
                Name = "赛马Light",
                Author = "Heer Kaisair",
                Version = "1.0.0",
                Description = "由Heer鼎力驱动! 服务端By Node.Js",
                AppId = "light.horserace"
            };
        }

        /// <summary>
        /// 在这里注册事件
        /// </summary>
        private static void RegEvents()
        {
            AddPluginEvent<JoinEvent>();
            AddPluginEvent<CreateRaceEvent>();
            AddPluginEvent<StartRaceEvent>();
        }


        /// <summary>
        /// 对AppService的建造
        /// </summary>
        /// <param name="service"></param>
        public static void ServiceBuilder(AppService service)
        {
            //添加对这些框架的API包装器
            service.AddMQConfig().AddKumConfig().AddXQConfig().AddLypConfig().AddXlzConfig().AddOnoConfig()
                .AddMiniConfig();
        }
    }
}