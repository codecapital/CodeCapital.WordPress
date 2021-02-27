using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CodeCapital.WordPress.Core.Extensions
{
    public class WordPressOptionsExtension : CoreOptionsExtension
    {
        public string TablePrefix { get; }

        public WordPressOptionsExtension(string tablePrefix)
        {
            TablePrefix = tablePrefix;
        }
    }

    // Non needed, we only want to pass TablePrefix
    //public class WordPressOptionsExtension : IDbContextOptionsExtension
    //{
    //    public string TablePrefix { get; set; }

    //    public WordPressOptionsExtension(string tablePrefix)
    //    {
    //        TablePrefix = tablePrefix;
    //    }

    //    public bool ApplyServices(IServiceCollection services)
    //    {
    //        return true;
    //    }

    //    public long GetServiceProviderHashCode()
    //    {
    //        return 0;
    //    }

    //    public void Validate(IDbContextOptions options)
    //    {
    //    }

    //    public string LogFragment { get; }
    //}
}