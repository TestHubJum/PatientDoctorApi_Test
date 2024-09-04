using Microsoft.EntityFrameworkCore;

namespace PatientDoctorApi.Models
{
    public class ApplicationContext: DbContext
    {
        public DbSet<Patient> Patients { get; set; } = null!;
        public DbSet<Doctor> Doctors { get; set; } = null!;
        public DbSet<Section> Sections { get; set; } = null!;
        public DbSet<Specialization> Specializations { get; set; } = null!;
        public DbSet<Office> Offices { get; set; } = null!; 


        public ApplicationContext(DbContextOptions<ApplicationContext> options)
    : base(options) 
        {

        }

    }
}
