using Dmb.Crm.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dmb.Crm.Data.Context;

public class CrmContext : DbContext
{
    public CrmContext(DbContextOptions<CrmContext> options) : base(options)
    {
    }

    public DbSet<Agency> Agencies => Set<Agency>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<CrmUser> Users => Set<CrmUser>();
    public DbSet<UserLocation> UserLocations => Set<UserLocation>();
    public DbSet<AccountActivationToken> AccountActivationTokens => Set<AccountActivationToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();
    public DbSet<ExternalLogin> ExternalLogins => Set<ExternalLogin>();
    public DbSet<PendingExternalLogin> PendingExternalLogins => Set<PendingExternalLogin>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ContactTag> ContactTags => Set<ContactTag>();
    public DbSet<Pipeline> Pipelines => Set<Pipeline>();
    public DbSet<PipelineStage> PipelineStages => Set<PipelineStage>();
    public DbSet<Opportunity> Opportunities => Set<Opportunity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agency>(e =>
        {
            e.ToTable("crm_agencies");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Name).HasColumnName("name");
            e.Property(x => x.Slug).HasColumnName("slug");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<Location>(e =>
        {
            e.ToTable("crm_locations");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.AgencyId).HasColumnName("agency_id");
            e.Property(x => x.Name).HasColumnName("name");
            e.Property(x => x.Timezone).HasColumnName("timezone");
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(x => x.Agency).WithMany(x => x.Locations).HasForeignKey(x => x.AgencyId);
        });

        modelBuilder.Entity<CrmUser>(e =>
        {
            e.ToTable("crm_users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.AgencyId).HasColumnName("agency_id");
            e.Property(x => x.Username).HasColumnName("username");
            e.Property(x => x.Email).HasColumnName("email");
            e.Property(x => x.FirstName).HasColumnName("first_name");
            e.Property(x => x.LastName).HasColumnName("last_name");
            e.Property(x => x.PasswordHash).HasColumnName("password_hash");
            e.Property(x => x.PasswordSalt).HasColumnName("password_salt");
            e.Property(x => x.ContactNo).HasColumnName("contact_no");
            e.Property(x => x.Activated).HasColumnName("activated");
            e.Property(x => x.IsSuperAdmin).HasColumnName("is_super_admin");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(x => x.Agency).WithMany(x => x.Users).HasForeignKey(x => x.AgencyId);
        });

        modelBuilder.Entity<UserLocation>(e =>
        {
            e.ToTable("crm_user_locations");
            e.HasKey(x => new { x.UserId, x.LocationId });
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.LocationId).HasColumnName("location_id");
            e.Property(x => x.Role).HasColumnName("role");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasOne(x => x.User).WithMany(x => x.UserLocations).HasForeignKey(x => x.UserId);
            e.HasOne(x => x.Location).WithMany(x => x.UserLocations).HasForeignKey(x => x.LocationId);
        });

        modelBuilder.Entity<AccountActivationToken>(e =>
        {
            e.ToTable("crm_account_activation_tokens");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.TokenHash).HasColumnName("token_hash");
            e.Property(x => x.ExpiresAt).HasColumnName("expires_at");
            e.Property(x => x.UsedAt).HasColumnName("used_at");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<PasswordResetToken>(e =>
        {
            e.ToTable("crm_password_reset_tokens");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.TokenHash).HasColumnName("token_hash");
            e.Property(x => x.ExpiresAt).HasColumnName("expires_at");
            e.Property(x => x.UsedAt).HasColumnName("used_at");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<RevokedToken>(e =>
        {
            e.ToTable("crm_revoked_tokens");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Jti).HasColumnName("jti");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.ExpiresAt).HasColumnName("expires_at");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<ExternalLogin>(e =>
        {
            e.ToTable("crm_external_logins");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UserId).HasColumnName("user_id");
            e.Property(x => x.Provider).HasColumnName("provider");
            e.Property(x => x.ProviderUserId).HasColumnName("provider_user_id");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            e.HasIndex(x => new { x.Provider, x.ProviderUserId }).IsUnique();
            e.HasIndex(x => new { x.UserId, x.Provider }).IsUnique();
        });

        modelBuilder.Entity<PendingExternalLogin>(e =>
        {
            e.ToTable("crm_pending_external_logins");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Ticket).HasColumnName("ticket");
            e.Property(x => x.Provider).HasColumnName("provider");
            e.Property(x => x.ProviderUserId).HasColumnName("provider_user_id");
            e.Property(x => x.FirstName).HasColumnName("first_name");
            e.Property(x => x.LastName).HasColumnName("last_name");
            e.Property(x => x.Email).HasColumnName("email");
            e.Property(x => x.Phone).HasColumnName("phone");
            e.Property(x => x.Client).HasColumnName("client");
            e.Property(x => x.ReturnPath).HasColumnName("return_path");
            e.Property(x => x.CodeHash).HasColumnName("code_hash");
            e.Property(x => x.CodeExpiresAt).HasColumnName("code_expires_at");
            e.Property(x => x.ExpiresAt).HasColumnName("expires_at");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.Ticket).IsUnique();
        });

        modelBuilder.Entity<Company>(e =>
        {
            e.ToTable("crm_companies");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.LocationId).HasColumnName("location_id");
            e.Property(x => x.Name).HasColumnName("name");
            e.Property(x => x.Website).HasColumnName("website");
            e.Property(x => x.Phone).HasColumnName("phone");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<Contact>(e =>
        {
            e.ToTable("crm_contacts");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.LocationId).HasColumnName("location_id");
            e.Property(x => x.CompanyId).HasColumnName("company_id");
            e.Property(x => x.FirstName).HasColumnName("first_name");
            e.Property(x => x.LastName).HasColumnName("last_name");
            e.Property(x => x.Email).HasColumnName("email");
            e.Property(x => x.Phone).HasColumnName("phone");
            e.Property(x => x.Source).HasColumnName("source");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        });

        modelBuilder.Entity<Tag>(e =>
        {
            e.ToTable("crm_tags");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.LocationId).HasColumnName("location_id");
            e.Property(x => x.Name).HasColumnName("name");
            e.Property(x => x.Color).HasColumnName("color");
        });

        modelBuilder.Entity<ContactTag>(e =>
        {
            e.ToTable("crm_contact_tags");
            e.HasKey(x => new { x.ContactId, x.TagId });
            e.Property(x => x.ContactId).HasColumnName("contact_id");
            e.Property(x => x.TagId).HasColumnName("tag_id");
            e.HasOne(x => x.Contact).WithMany(x => x.ContactTags).HasForeignKey(x => x.ContactId);
            e.HasOne(x => x.Tag).WithMany(x => x.ContactTags).HasForeignKey(x => x.TagId);
        });

        modelBuilder.Entity<Pipeline>(e =>
        {
            e.ToTable("crm_pipelines");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.LocationId).HasColumnName("location_id");
            e.Property(x => x.Name).HasColumnName("name");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<PipelineStage>(e =>
        {
            e.ToTable("crm_pipeline_stages");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.LocationId).HasColumnName("location_id");
            e.Property(x => x.PipelineId).HasColumnName("pipeline_id");
            e.Property(x => x.Name).HasColumnName("name");
            e.Property(x => x.SortOrder).HasColumnName("sort_order");
            e.HasOne(x => x.Pipeline).WithMany(x => x.Stages).HasForeignKey(x => x.PipelineId);
        });

        modelBuilder.Entity<Opportunity>(e =>
        {
            e.ToTable("crm_opportunities");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.LocationId).HasColumnName("location_id");
            e.Property(x => x.PipelineId).HasColumnName("pipeline_id");
            e.Property(x => x.StageId).HasColumnName("stage_id");
            e.Property(x => x.ContactId).HasColumnName("contact_id");
            e.Property(x => x.CompanyId).HasColumnName("company_id");
            e.Property(x => x.Name).HasColumnName("name");
            e.Property(x => x.Value).HasColumnName("value").HasPrecision(12, 2);
            e.Property(x => x.Status).HasColumnName("status");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(x => x.Pipeline).WithMany().HasForeignKey(x => x.PipelineId);
            e.HasOne(x => x.Stage).WithMany().HasForeignKey(x => x.StageId);
            e.HasOne(x => x.Contact).WithMany().HasForeignKey(x => x.ContactId);
            e.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        });
    }
}
