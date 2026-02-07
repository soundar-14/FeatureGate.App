using System;
using System.Collections.Generic;
using System.Text;

namespace FeatureGate.Application.Interfaces.Services.Cache
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan ttl);
        Task RemoveByPrefixAsync(string prefix);
    }
}
