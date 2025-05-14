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
        public DbSet<EmailVerification>VerifyEmail { get; set; }
        public DbSet<ShippingDetails> shippingDetails { get; set; }

        public DbSet<CartItem> Cart { get; set; }
        public DbSet<Payment> PaymentRecords { get; set; }
        public DbSet<Track> trackOrder { get; set; }
        public DbSet<TrackAllOrders> OrderStatus { get; set; }


    }
}
