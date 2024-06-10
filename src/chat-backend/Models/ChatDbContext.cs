using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ChatApp.Models;

namespace ChatApp;

public partial class ChatDbContext : DbContext
{
    public ChatDbContext()
    {
    }

    public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Media> Media { get; set; }

    public virtual DbSet<Message> Messeges { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UsersInChats> UsersInChats { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=ConnectionStrings:DBConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("chats_pkey");

            entity.ToTable("chats");

            entity.Property(e => e.ChatId).HasColumnName("chat_id");
            entity.Property(e => e.CreationDate).HasColumnName("creation_date");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Media>(entity =>
        {
            entity.HasKey(e => e.MediaId).HasName("media_pkey");

            entity.ToTable("media");

            entity.Property(e => e.MediaId).HasColumnName("media_id");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.MessegeId).HasColumnName("messege_id");

            entity.HasOne(d => d.Messege).WithMany(p => p.Media)
                .HasForeignKey(d => d.MessegeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("messege_id");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("messeges_pkey");

            entity.ToTable("messeges");

            entity.Property(e => e.MessageId).HasColumnName("messege_id");
            entity.Property(e => e.ChatId).HasColumnName("chat_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Chat).WithMany(p => p.Messeges)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chat_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Messeges)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("user_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Login)
                .HasMaxLength(64)
                .HasColumnName("login");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.PasswordSalt).HasColumnName("password_salt");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("user_role_pkey");

            entity.ToTable("user_role");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(15)
                .HasColumnName("role_name");
        });
        
        modelBuilder.Entity<UsersInChats>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ChatId }).HasName("users_in_chats_pkey");

            entity.ToTable("users_in_chats");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ChatId).HasColumnName("chat_id");
            entity.Property(e => e.UserRole).HasColumnName("user_role");

            entity.HasOne(d => d.Chat).WithMany(p => p.UsersInChats)
                .HasForeignKey(d => d.ChatId)
                .HasConstraintName("chat_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UsersInChats)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_fkey");

            entity.HasOne(d => d.UserRoleNavigation).WithMany(p => p.UsersInChats)
                .HasForeignKey(d => d.UserRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_role_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
