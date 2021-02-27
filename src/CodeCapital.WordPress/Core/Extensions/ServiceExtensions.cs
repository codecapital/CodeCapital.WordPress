using CodeCapital.WordPress.Core.Repositories;
using CodeCapital.WordPress.Persistence;
using CodeCapital.WordPress.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace CodeCapital.WordPress.Core.Extensions
{
    public static class ServiceExtensions
    {
        private static void AddCommon(IServiceCollection services, IConfiguration configuration, bool enableSensitiveLogging = false, bool enableDetailedErrors = false)
        {
            //if (services == null) throw new ArgumentNullException(nameof(services));
            _ = services ?? throw new ArgumentNullException(nameof(services));

            //if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            services.Configure<WordPressSettings>(options => configuration.GetSection(nameof(WordPressSettings)).Bind(options));

            services.AddDbContext<WordPressDbContext>(options =>
                {
                    options.UseMySql(configuration.GetConnectionString("WordPress"), mySqlOptions =>
                    {
                        //mySqlOptions.ServerVersion(new Version(10, 1, 38), ServerType.MariaDb);
                        //mySqlOptions.AnsiCharSet(CharSet.Utf8mb4);
                        //mySqlOptions.UnicodeCharSet(CharSet.Utf8mb4);
                    });
                    options.EnableSensitiveDataLogging(enableSensitiveLogging);
                    options.EnableDetailedErrors(enableDetailedErrors);
                }
                //.UseWordPress(configuration.GetSection("WordPressSettings:TablePrefix").Value)
            );

            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ITermRepository, TermRepository>();
            services.AddScoped<MetadataRepository>();
            services.AddScoped<MenuRepository>();
            services.AddScoped<OptionRepository>();
            services.AddScoped<AttachmentRepository>();
            services.TryAddSingleton<IShortCodeService, ShortCodeService>();
            services.TryAddScoped<IPostProcessingService, PostProcessingService>();
        }

        /// <summary>
        /// Uses default "ConnectionStrings": { "WordPress": "" } and and WordPressSettings:TablePrefix
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="enableSensitiveLogging"></param>
        /// <param name="enableDetailedErrors"></param>
        /// <returns></returns>
        public static void AddWordPress(this IServiceCollection services, IConfiguration configuration, bool enableSensitiveLogging = false, bool enableDetailedErrors = false)
        {
            //if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            AddCommon(services, configuration, enableSensitiveLogging, enableDetailedErrors);

            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ITermService, TermService>();
            services.AddScoped<IMetadataService, MetadataService>();
        }

        /// <summary>
        /// Uses default "ConnectionStrings": { "WordPress": "" } and and WordPressSettings:TablePrefix
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="enableSensitiveLogging"></param>
        /// <param name="enableDetailedErrors"></param>
        public static void AddCachedWordPress(this IServiceCollection services, IConfiguration configuration, bool enableSensitiveLogging = false, bool enableDetailedErrors = false)
        {
            //if (services == null) throw new ArgumentNullException(nameof(services));
            _ = services ?? throw new ArgumentNullException(nameof(services));

            AddCommon(services, configuration, enableSensitiveLogging, enableDetailedErrors);

            services.AddScoped<ResetService>();
            services.AddScoped<CacheService>();
            services.AddScoped<IMenuService, CachedMenuService>();
            services.AddScoped<IPostService, CachedPostService>();
            services.AddScoped<ITermService, CachedTermService>();
            services.AddScoped<IMetadataService, CachedMetadataService>();
        }

        //public static IServiceCollection AddWordPress(this IServiceCollection services, IConfiguration configuration,
        //    string tablePrefix = null)
        //{
        //    services.AddDbContext<WordPressDbContext>(options => options
        //        .UseMySql(configuration.GetSection("WordPressSettings:TablePrefix").Value)
        //    //.UseWordPress(tablePrefix)
        //    );

        //    return services;
        //}
    }
}