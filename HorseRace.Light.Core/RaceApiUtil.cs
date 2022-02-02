using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Amiable.Core
{
    /// <summary>
    /// 创建一个赛场API实例，赛场ID会被包裹在此对象。
    /// </summary>
    public class RaceApiUtil
    {
        private static string _url = "http://127.0.0.1:3099";

        public static async Task<string> Post(string url, string body)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(body);
                content.Headers.ContentType= MediaTypeHeaderValue.Parse("application/json");
                var resp = await client.PostAsync(url, content);
                return await resp.Content.ReadAsStringAsync();
            }
        }

        public static async Task<string> Get(string url)
        {
            using (var client = new HttpClient())
            {
                var resp = await client.GetAsync(url);
                return await resp.Content.ReadAsStringAsync();
            }
        }

        public string RaceId;

        private bool _started;

        private bool _ended;

        public RaceApiModels.RaceNextInfo.RaceInfo EndRace;

        public async Task<T> Get<T>(string url)
        {
            using (var client = new HttpClient())
            {
                var resp = await client.GetAsync(url);
                return await JsonSerializer.DeserializeAsync<T>(await resp.Content.ReadAsStreamAsync());
            }
        }


        /// <summary>
        /// 返回RaceId
        /// </summary>
        /// <returns></returns>
        public async Task<RaceApiModels.RaceCreateInfo> CreateRace()
        {
            var url = _url + "/race/create";
            var data = await Get<RaceApiModels.RaceCreateInfo>(url);
            RaceId = data.raceId;
            return data;
        }

        /// <summary>
        /// 加入赛马，返回加入日志消息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <returns>日志消息</returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<string>> JoinRace(string userId, string name)
        {
            if (_started)
            {
                throw new("比赛已经开始");
            }

            var url = _url + $"/race/{RaceId}/join";
            var joinInfo = new { userId, horseDisplay = name };
            var data = await Post(url, JsonSerializer.Serialize(joinInfo));
            if (data != "false")
            {
                var logs = JsonSerializer.Deserialize<List<string>>(data);
                return logs;
            }
            else
            {
                throw new("提示:加入赛马失败");
            }
        }

        /// <summary>
        /// 开始比赛。内部对象状态会变成开始。（缓存。当然开始失败了我也没办法。）
        /// </summary>
        /// <returns></returns>
        public async Task<RaceApiModels.RaceStartInfo> StartRace()
        {
            if (_started)
            {
                throw new("比赛已开始");
            }

            _started = true;
            var url = _url + $"/race/{RaceId}/start";
            var data = await Get<RaceApiModels.RaceStartInfo>(url);
            return data;
        }

        public async Task<RaceApiModels.RaceNextInfo> Next()
        {
            if (!_started)
            {
                throw new("比赛未开始");
            }

            if (_ended)
            {
                throw new("比赛已结束");
            }

            var url = _url + $"/race/{RaceId}/next";
            var data = await Get<RaceApiModels.RaceNextInfo>(url);
            if (data.End)
            {
                _ended = true;
                this.EndRace = data.Race;
            }

            return data;
        }

        public bool IsEnd()
        {
            return _ended;
        }
    }
}