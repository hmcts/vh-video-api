using Microsoft.EntityFrameworkCore;

namespace RefData
{
    public partial class RefDataContext : DbContext
    {
        public RefDataContext()
        {
        }

        public RefDataContext(DbContextOptions<RefDataContext> options)
            : base(options)
        {
        }
    }
}
