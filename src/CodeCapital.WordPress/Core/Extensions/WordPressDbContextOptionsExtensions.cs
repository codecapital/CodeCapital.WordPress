using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CodeCapital.WordPress.Core.Extensions
{
    public static class WordPressDbContextOptionsExtensions
    {
        // Probably no needed because we get tablePrefix from IConfiguration
        public static DbContextOptionsBuilder UseWordPress(this DbContextOptionsBuilder optionsBuilder,
            string tablePrefix)
        {
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(new WordPressOptionsExtension(tablePrefix));

            return optionsBuilder;
        }
    }
}