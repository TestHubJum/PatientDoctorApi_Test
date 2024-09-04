namespace PatientDoctorApi.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int OfficeId { get; set; }
        public Office Office { get; set; }
        public int SpecializationId { get; set; }
        public Specialization Specialization { get; set; }
        public int? SectionId { get; set; }
        public Section Section { get; set; }
    }
}
