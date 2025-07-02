using CustomGuid.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Prodata.WebForm.Models.Auth;
using Prodata.WebForm.Models.MasterData;
using Prodata.WebForm.Models.System;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Web;

namespace Prodata.WebForm.Models
{
	public class AppDbContext : CustomIdentityDbContext<User>
	{
        public AppDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        #region Override method
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<CustomIdentityRole>().ToTable("Roles");
            modelBuilder.Entity<CustomIdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<CustomIdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<CustomIdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<TransferApprovalLog>().ToTable("TransferApprovalLog");  
            modelBuilder.Entity<AdditionalBudgetLog>().ToTable("AdditionalBudgetLog"); 
            modelBuilder.Ignore<BaseModel>();
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<BaseModel>()
                .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Id = Guid.NewGuid(); // Ensure GUID is set for new records

                    // Check if properties exist before setting them
                    if (entry.Entity.GetType().GetProperty("CreatedBy") != null)
                        entry.CurrentValues["CreatedBy"] = Prodata.WebForm.Auth.Id();

                    if (entry.Entity.GetType().GetProperty("CreatedDate") != null)
                        entry.CurrentValues["CreatedDate"] = DateTime.Now;
                }

                // Check if properties exist before setting them
                if (entry.Entity.GetType().GetProperty("UpdatedBy") != null)
                    entry.CurrentValues["UpdatedBy"] = Prodata.WebForm.Auth.Id();

                if (entry.Entity.GetType().GetProperty("UpdatedDate") != null)
                    entry.CurrentValues["UpdatedDate"] = DateTime.Now;
            }
        }
        #endregion

        public DbSet<Module> Modules { get; set; }

        public DbSet<RoleModule> RoleModules { get; set; }

        public DbSet<BudgetType> BudgetTypes { get; set; }

        public DbSet<Budget> Budgets { get; set; }

        public DbSet<FormType> FormTypes { get; set; }

        public DbSet<Form> Forms { get; set; }

        public DbSet<FormBudget> FormBudgets { get; set; }

        public DbSet<Vendor> Vendors { get; set; }

        public DbSet<FormVendor> FormVendors { get; set; }

        public DbSet<ApprovalLimit> ApprovalLimits { get; set; }

        public DbSet<TransferApprovalLimits> TransferApprovalLimits { get; set; }

        public DbSet<Approval> Approvals { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<TransfersTransaction> TransfersTransaction { get; set; }

        public DbSet<TransferDocument> TransferDocuments { get; set; }

        public DbSet<TransferApprovalLog> TransferApprovalLog { get; set; }

        public DbSet<AdditionalBudgetDocuments> AdditionalBudgetDocuments { get; set; }

        public DbSet<AdditionalBudgetRequests> AdditionalBudgetRequests { get; set; }

        public DbSet<AdditionalLoaCogsLimits> AdditionalLoaCogsLimits { get; set; } 

        public DbSet<AdditionalLoaFinanceLimits> AdditionalLoaFinanceLimits { get; set; }

        public DbSet<AdditionalBudgetLog> AdditionalBudgetLog { get; set; }

        public DbSet<AdditionalCumulativeLimits> AdditionalCumulativeLimits { get; set; }

        public DbSet<Attachment> Attachments { get; set; }
    }
}