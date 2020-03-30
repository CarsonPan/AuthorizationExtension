namespace AuthorizationExtension.Core
{
    public class CacheOptions
    {
        public bool Enabled{get;set;}=true;
        /// <summary>
        /// 过期时间 单位s
        /// </summary>
        /// <value></value>
        public int Expiration{get;set;}=300;

        public ExpirationMode ExpirationMode{get;set;}=ExpirationMode.Sliding;
    }
}