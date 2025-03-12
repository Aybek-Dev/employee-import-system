using EmployeeImport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeImport.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuration for the Employee entity
    /// </summary>
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.PayrollNumber)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.HasIndex(e => e.Surname);
            
            builder.Property(e => e.Forenames)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(e => e.Surname)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(e => e.DateOfBirth)
                .IsRequired();
                
            builder.Property(e => e.Telephone)
                .HasMaxLength(20);
                
            builder.Property(e => e.Mobile)
                .HasMaxLength(20);
                
            builder.Property(e => e.Address)
                .HasMaxLength(200);
                
            builder.Property(e => e.Address2)
                .HasMaxLength(200);
                
            builder.Property(e => e.Postcode)
                .HasMaxLength(20);
                
            builder.Property(e => e.EmailHome)
                .HasMaxLength(100);
                
            builder.Property(e => e.StartDate)
                .IsRequired();
        }
    }
}