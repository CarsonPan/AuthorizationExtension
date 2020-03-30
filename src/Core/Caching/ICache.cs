namespace AuthorizationExtension.Core
{
    public interface ICache
    {
        bool TryGetValue<T>(string key,out T value);
        void Set<T>(string key,T value,ExpirationMode expirationMode,int expiration);
        void Remove(string key);
    }
}