using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Kampus.Host.Extensions
{
    public static class SessionExtensions
    {
        public static T Get<T>(this ISession session, string key)
        {
            return JsonConvert.DeserializeObject<T>(session.GetString(key));
        }

        public static void Add<T>(this ISession session, string key, T data)
        {
            var json = JsonConvert.SerializeObject(data, typeof(T), null);
            session.SetString(key, json);
        }
    }
}
