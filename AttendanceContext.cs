using AttendancesServices.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AttendancesServices
{
    public partial class AttendanceContext : DbContext
    {
        private const string connectionString = "Server=127.0.0.1;Port=3309;Uid=root;Pwd=Riz286;Database=aeams28012021;Charset=utf8mb4; Keepalive=60;default command timeout=60; Connection Timeout=60";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(connectionString);
        }

        public DbSet<AttendancesMachine> Attendances { get; set; }
        // public DbSet<EmployeeAddress> EmployeeAddress { get; set; }
        // public DbSet<Department> Departments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Configuring the one to many relationship

            modelBuilder.Entity<AttendancesMachine>()
                    .HasKey(e => new { e.AsmId, e.Date }); //, e => e.date

            /* modelBuilder.Entity<Employee>()
                 .HasOne<Department>(e => e.Department)
                 .WithMany(d => d.Employees)
                 .HasForeignKey(e => e.DepartmentID);*/
        }
    }
}
