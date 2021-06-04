using Newtonsoft.Json;

namespace THttpWebRequest.Utility
{
    public static class UtilityObject
    {
        public static T DeserializeJsonAs<T>(this string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj);
        }

        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}