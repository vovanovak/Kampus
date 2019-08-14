using Kampus.Persistence.Entities.UserRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class StudentDetailsEntityTypeConfiguration : IEntityTypeConfiguration<StudentDetails>
    {
        public void Configure(EntityTypeBuilder<StudentDetails> builder)
        {
            builder.HasKey(sd => sd.StudentDetailsId);
            builder.HasOne(b => b.University).WithMany().HasForeignKey(b => b.UniversityId);
            builder.HasOne(b => b.Faculty).WithMany().HasForeignKey(b => b.FacultyId);
        }
    }
}