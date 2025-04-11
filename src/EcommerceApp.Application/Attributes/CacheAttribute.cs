namespace Infrastructure.Cache.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CacheAttribute : Attribute
    {
        public string Key { get; }
        public int ExpirationTime { get; }

        public CacheAttribute(string key, int expirationTime = 60)
        {
            Key = key;
            ExpirationTime = expirationTime;
        }
    }

}
