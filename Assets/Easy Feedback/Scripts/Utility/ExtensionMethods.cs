using UnityEngine;
namespace EasyFeedback
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Wraps a class around a json array so that it can be deserialized by JsonUtility
        /// </summary>
        /// <param name="source"></param>
        /// <param name="topClass"></param>
        /// <returns></returns>
        public static string WrapToClass(this string source, string topClass)
        {
            return string.Format("{{\"{0}\": {1}}}", topClass, source);
        }
    }
}
