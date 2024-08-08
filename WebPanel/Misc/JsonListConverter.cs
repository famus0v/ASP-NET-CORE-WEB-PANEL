using Newtonsoft.Json;
using NuGet.Packaging.Signing;

namespace WebPanel.Misc
{
    public static class JsonListConverter
    {
        public static string CreateNewList(int id)
        {
            List<int> newList = new() { id };
            string json = JsonConvert.SerializeObject(newList);

            return json;
        }

        public static string AddToStringList(string json,int id)
        {
            List<int>? ints = JsonConvert.DeserializeObject<List<int>>(json);
            ints?.Add(id);

            return JsonConvert.SerializeObject(ints);
        }

        public static string AddToStringList(string json, List<int> ids)
        {
            List<int>? ints = JsonConvert.DeserializeObject<List<int>>(json);
            ints?.AddRange(ids);

            return JsonConvert.SerializeObject(ints);
        }

        public static string DeleteIntoList(string json, int id)
        {
            List<int>? ints = JsonConvert.DeserializeObject<List<int>>(json);
            ints?.Remove(id);

            return JsonConvert.SerializeObject(ints);
        }

        public static List<int> GetListIntoString(string json) => json != null ? JsonConvert.DeserializeObject<List<int>>(json) ?? new List<int>() : new List<int>();
    }
}
