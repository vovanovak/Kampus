using Kampus.Persistence.Entities.UserRelated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kampus.Persistence.EntityTypeConfigurations
{
    public class StudentDetailsEntityTypeConfiguration : IEntityTypeConfiguration<StudentDetails>
    {
        public void Configure(EntityTypeBuilder<StudentDetails> builder)
        {
            builder.ToTable("StudentDetails");
            builder.HasKey(sd => sd.StudentDetailsId);
            builder.HasOne(b => b.University);
            builder.HasOne(b => b.Faculty);
        }
    }
}