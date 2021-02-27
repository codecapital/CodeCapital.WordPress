using CodeCapital.WordPress.Core.Repositories;
using CodeCapital.WordPress.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CodeCapital.WordPress.Persistence
{
    public class UnitOfWork
    {
        private readonly WordPressDbContext _context;

        //public readonly AppSettings AppSettings;
        public IPostRepository Posts { get; }
        //public ITermService TermService { get; }
        public IMetadataService MetadataService { get; }
        public TermTaxonomyRepository TermTaxonomies { get; }
        public OptionRepository Options { get; }
        public AttachmentRepository Attachments { get; }

        public UnitOfWork(
            WordPressDbContext context,
            //IOptions<AppSettings> appSettings,
            ITermRepository termRepository,
            MetadataRepository metadataRepository,
            IMetadataService metadataService,
            OptionRepository optionRepository,
            AttachmentRepository attachmentRepository,
            ILoggerFactory loggerFactory
            //IOptions<ClientSettings> clientSettings,
            //RazorViewRenderer razorView
            )
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            //AppSettings = appSettings.Value;
            Posts = new PostRepository(_context);
            //TermService = new TermService(termRepository, memoryCache);
            MetadataService = metadataService;
            TermTaxonomies = new TermTaxonomyRepository(_context);
            Options = new OptionRepository(_context);
            Attachments = new AttachmentRepository(_context);
            //RazorView = razorView;
            // this must be last one so the all services are working in this service
        }
    }
}
