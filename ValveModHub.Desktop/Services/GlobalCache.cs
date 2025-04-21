using Microsoft.Extensions.Caching.Memory;

namespace ValveModHub.Desktop.Services;

public static class GlobalCache
{
    public static IMemoryCache Cache { get; set; }

    static GlobalCache()
    {
        Cache = new MemoryCache(new MemoryCacheOptions());
    }
}