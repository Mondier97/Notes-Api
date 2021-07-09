using Boomtown_Notes_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Boomtown_Notes_Api
{
    /// <summary>
    /// Entity Framework context used to interact with our notes in the database.
    /// Documentation referenced found at: https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
    /// </summary>
    public class NotesContext : DbContext
    {
        private string connectionString = "data source=notes.sqlite";
        // Represents the notes within our database
        public DbSet<Note> Notes { get; set; }

        public NotesContext()
        {
            Database.EnsureCreated();
        }

        public NotesContext(string connectionString)
        {
            this.connectionString = connectionString;
            Database.EnsureCreated();
        }

        // Sets our app to use a local database file for holding notes
        protected override void OnConfiguring(DbContextOptionsBuilder options)
           => options.UseSqlite(connectionString);

        // Allows ids to be automatically generated for notes through entity framework
        protected override void OnModelCreating(ModelBuilder modelBuilder)
           => modelBuilder.Entity<Note>()
            .Property(n => n.Id)
            .ValueGeneratedOnAdd();
    }
}
