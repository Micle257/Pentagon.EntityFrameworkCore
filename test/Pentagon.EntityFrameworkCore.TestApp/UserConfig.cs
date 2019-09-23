namespace Pentagon.EntityFrameworkCore.TestApp {
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    class UserConfig : IEntityTypeConfiguration<User>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(a => a.Name)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.HasIndex(a => a.Name)
                   .IsUnique();

            builder.Property(a => a.a1)
                   .HasDefaultValue(6);
        }
    }
}