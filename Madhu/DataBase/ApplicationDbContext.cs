using Humanizer;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Madhu.Models
{
    public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

      
        public DbSet<MyUsers> Users { get; set; }
        public DbSet<MyTranscationHistory> TranscationHistory { get; set; }

        public DbSet<MyLoginHistory> LoginHistory { get; set; }

        public DbSet<MyLoanRequest> LoanRequest { get; set; }
        public DbSet<MyGiftVoucher> GiftVoucher { get; set; }

    }
}
