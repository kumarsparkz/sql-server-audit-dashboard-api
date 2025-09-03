using Microsoft.EntityFrameworkCore;
using AuditDashboard.Models;

namespace AuditDashboard.Data
{
    public class AuditDashboardContext : DbContext
    {
        public AuditDashboardContext(DbContextOptions<AuditDashboardContext> options) : base(options)
        {
        }

        public DbSet<MonitoredServer> MonitoredServers { get; set; }
        public DbSet<ServerMetric> ServerMetrics { get; set; }
        public DbSet<DatabaseMetric> DatabaseMetrics { get; set; }
        public DbSet<QueryPerformance> QueryPerformance { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<SecurityEvent> SecurityEvents { get; set; }
        public DbSet<ActiveAlert> ActiveAlerts { get; set; }
        public DbSet<AlertDefinition> AlertDefinitions { get; set; }
        public DbSet<DashboardUser> DashboardUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal properties with precision and scale
            modelBuilder.Entity<ActiveAlert>(entity =>
            {
                entity.HasKey(e => e.AlertID);
                entity.Property(e => e.CurrentValue)
                    .HasColumnType("decimal(18,4)");
                entity.Property(e => e.ThresholdValue)
                    .HasColumnType("decimal(18,4)");
            });

            modelBuilder.Entity<AlertDefinition>(entity =>
            {
                entity.HasKey(e => e.AlertDefinitionID);
                entity.Property(e => e.Threshold)
                    .HasColumnType("decimal(18,4)");
            });

            modelBuilder.Entity<DatabaseMetric>(entity =>
            {
                entity.HasKey(e => e.DatabaseMetricID);
                entity.Property(e => e.DatabaseSize)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.DataSize)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.DataUsed)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.LogSize)
                    .HasColumnType("decimal(18,2)");
                entity.Property(e => e.PercentUsed)
                    .HasColumnType("decimal(5,2)");
            });

            modelBuilder.Entity<QueryPerformance>(entity =>
            {
                entity.HasKey(e => e.QueryPerformanceID);
                entity.Property(e => e.AvgDuration)
                    .HasColumnType("decimal(18,4)");
                entity.Property(e => e.AvgCPUTime)
                    .HasColumnType("decimal(18,4)");
            });

            modelBuilder.Entity<ServerMetric>(entity =>
            {
                entity.HasKey(e => e.ServerMetricID);
                entity.Property(e => e.MetricValue)
                    .HasColumnType("decimal(18,4)");
            });

            modelBuilder.Entity<SecurityEvent>(entity =>
            {
                entity.HasKey(e => e.SecurityEventID);
            });

            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.HasKey(e => e.SessionID);
            });

            modelBuilder.Entity<DashboardUser>(entity =>
            {
                entity.HasKey(e => e.UserID);
            });

            modelBuilder.Entity<MonitoredServer>(entity =>
            {
                entity.HasKey(e => e.ServerID);
            });

            // Configure relationships
            modelBuilder.Entity<ServerMetric>()
                .HasOne(sm => sm.MonitoredServer)
                .WithMany(ms => ms.ServerMetrics)
                .HasForeignKey(sm => sm.ServerID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatabaseMetric>()
                .HasOne(dm => dm.MonitoredServer)
                .WithMany()
                .HasForeignKey(dm => dm.ServerID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SecurityEvent>()
                .HasOne(se => se.MonitoredServer)
                .WithMany()
                .HasForeignKey(se => se.ServerID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ActiveAlert>()
                .HasOne(aa => aa.MonitoredServer)
                .WithMany(ms => ms.ActiveAlerts)
                .HasForeignKey(aa => aa.ServerID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QueryPerformance>()
                .HasOne(qp => qp.MonitoredServer)
                .WithMany()
                .HasForeignKey(qp => qp.ServerID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserSession>()
                .HasOne(us => us.MonitoredServer)
                .WithMany()
                .HasForeignKey(us => us.ServerID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}