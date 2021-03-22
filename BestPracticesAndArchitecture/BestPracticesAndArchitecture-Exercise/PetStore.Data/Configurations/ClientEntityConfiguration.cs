using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PetStore.Common;
using PetStore.Models;

namespace PetStore.Data.Configurations
{
    public class ClientEntityConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.Property(c => c.FirstName)
                .HasMaxLength(GlobalConstants.ClientFirstNameMaxLength)
                .IsUnicode(true);

            builder.Property(c => c.LastName)
                .HasMaxLength(GlobalConstants.ClientLastNameMaxLength)
                .IsUnicode(true);

            builder.Property(c => c.UserName)
                .HasMaxLength(GlobalConstants.UserNameMaxLength)
                .IsUnicode(false);

            builder.Property(c => c.Email)
                .HasMaxLength(GlobalConstants.EmailNameMaxLength)
                .IsUnicode(false);

        }
    }
}
