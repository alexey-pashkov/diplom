// using AuthService.Models;
// using Microsoft.EntityFrameworkCore;

// namespace AuthService;
// public partial class AppDbContext : DbContext
// {
//     public virtual DbSet<User> Users { get; set; }
//     public virtual DbSet<Chat> Chats { get; set; }
//     public virtual DbSet<UserRole> UserRoles { get; set; }
//     public virtual DbSet<UsersInChats> UsersInChats { get; set; }

//     public AppDbContext()
//     {
//     }

//     public AppDbContext(DbContextOptions<AppDbContext> options)
//         : base(options)
//     {
//     }

//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         modelBuilder.Entity<User>(entity =>
//         {
//             entity.HasKey(e => e.UserId).HasName("users_pkey");

//             entity.ToTable("users");

//             entity.Property(e => e.UserId)
//                 .HasColumnName("user_id")
//                 .ValueGeneratedOnAdd();
//             entity.Property(e => e.Login)
//                 .HasMaxLength(64)
//                 .HasColumnName("login");
//             entity.Property(e => e.PasswordHash)
//                 .HasColumnName("password_hash");
//             entity.Property(e => e.PasswordSalt)
//                 .HasColumnName("password_salt");
//         });

//         modelBuilder.Entity<Chat>(entity =>
//         {
//             entity.HasKey(e => e.ChatId).HasName("chats_pkey");

//             entity.ToTable("chats");

//             entity.Property(e => e.ChatId)
//                 .HasColumnName("chat_id")
//                 .ValueGeneratedOnAdd();
//             entity.Property(e => e.CreationDate).HasColumnName("creation_date");
//             entity.Property(e => e.Name)
//                 .HasMaxLength(128)
//                 .HasColumnName("name");
//         });

//         modelBuilder.Entity<UserRole>(entity =>
//         {
//             entity.HasKey(e => e.RoleId).HasName("user_role_pkey");

//             entity.ToTable("user_role");

//             entity.Property(e => e.RoleId).HasColumnName("role_id")
//                 .ValueGeneratedNever();
//             entity.Property(e => e.RoleName).HasColumnName("role_name")
//                 .HasMaxLength(15);
//         });

//         modelBuilder.Entity<UsersInChats>(entity =>
//         {
//             entity.HasKey(e => new { e.UserId, e.ChatId }).HasName("users_in_chats_pkey");

//             entity.ToTable("users_in_chats");

//             entity.Property(e => e.UserId).HasColumnName("user_id");
//             entity.Property(e => e.ChatId).HasColumnName("chat_id");
//             entity.Property(e => e.RoleId).HasColumnName("user_role");

//             entity.HasOne(d => d.Chat).WithMany(p => p.UsersInChats)
//                 .HasForeignKey(d => d.ChatId)
//                 .HasConstraintName("chat_fkey");

//             entity.HasOne(d => d.User).WithMany(p => p.UsersInChats)
//                 .HasForeignKey(d => d.UserId)
//                 .HasConstraintName("user_fkey");

//             entity.HasOne(u => u.UserRole).WithMany(r => r.UsersInChats)
//                 .HasForeignKey(u => u.RoleId)
//                 .HasConstraintName("user_role_fkey");
//         });


//         OnModelCreatingPartial(modelBuilder);
//     }

//     partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
// }