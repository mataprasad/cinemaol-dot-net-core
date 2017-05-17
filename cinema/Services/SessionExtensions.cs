using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace WebApplication.Services
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static void SetObjectAsJson<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static object GetObjectFromJson(this ISession session, string key)
        {
            return GetSessionDataAsObject<object>(session, key);
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            return GetSessionDataAsObject<T>(session, key);
        }

        public static object GetItem(this ISession session, string key)
        {
            return GetSessionDataAsObject<object>(session, key);
        }

        public static T GetItem<T>(this ISession session, string key)
        {
            return GetSessionDataAsObject<T>(session, key);
        }

        private static T GetSessionDataAsObject<T>(ISession session, string key)
        {
            var value = session.GetString(key);
            if (value == null)
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}