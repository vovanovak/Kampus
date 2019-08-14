using Kampus.Persistence.Entities.UniversityRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class FacultyEntityTypeConfiguration : IEntityTypeConfiguration<Faculty>
    {
        public void Configure(EntityTypeBuilder<Faculty> builder)
        {
            builder.HasKey(b => b.FacultyId);
            builder.HasOne(f => f.University).WithMany(u => u.Faculties).HasForeignKey(f => f.UniversityId);
        }
    }
}