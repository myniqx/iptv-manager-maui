using iptv_manager_maui.Sources.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace iptv_manager_maui.Sources.Database
{
	public class M3UDatabase : DbContext
	{
		public DbSet<Profile> Profiles { get; set; }
		public DbSet<Group> Groups { get; set; }
		public DbSet<M3UEntitiy> M3UObjects { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
			optionsBuilder.UseSqlite($"Filename={dbPath}");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// M3UObject'te ProfileID ve Url alanlarına indeks ekle
			modelBuilder.Entity<M3UEntitiy>()
				.HasIndex(m => new { m.ProfileID, m.Url })
				.IsUnique();

			// Group'ta ParentGroupID ve ProfileID alanlarına indeks ekle
			modelBuilder.Entity<Group>()
				.HasIndex(g => new { g.ProfileID, g.ParentGroupID });

			// Group için kendine referans veren ilişki
			modelBuilder.Entity<Group>()
				.HasOne(g => g.ParentGroup)
				.WithMany(g => g.ChildGroups)
				.HasForeignKey(g => g.ParentGroupID)
				.OnDelete(DeleteBehavior.Cascade); // Parent Group silinirse Child Groups da silinir

			// Group ve M3UObject ilişkisi
			modelBuilder.Entity<M3UEntitiy>()
				.HasOne(m => m.UpperGroup)
				.WithMany(g => g.M3UObjects)
				.HasForeignKey(m => m.GroupID)
				.OnDelete(DeleteBehavior.Cascade); // Group silinirse ona bağlı M3UObjects da silinir

			// Profile ile BaseGroup ilişkisi
			modelBuilder.Entity<Profile>()
				.HasOne(p => p.BaseGroup)
				.WithMany()
				.HasForeignKey(p => p.BaseGroupID)
				.OnDelete(DeleteBehavior.Cascade); // Profile silinirse BaseGroup ve ona bağlı her şey silinir

			// Profile ile M3UObject arasındaki ilişki için Cascade silme
			modelBuilder.Entity<M3UEntitiy>()
				.HasOne(m => m.Profile)
				.WithMany()
				.HasForeignKey(m => m.ProfileID)
				.OnDelete(DeleteBehavior.Cascade);

			// Profile ile Group arasındaki ilişki için Cascade silme
			modelBuilder.Entity<Group>()
				.HasOne(g => g.Profile)
				.WithMany()
				.HasForeignKey(g => g.ProfileID)
				.OnDelete(DeleteBehavior.Cascade);
		}

	}
}
