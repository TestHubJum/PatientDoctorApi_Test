namespace PatientDoctorApi.Models.ViewModels
{
    public class DoctorEditView
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int OfficeId { get; set; }
        public int SpecializationId { get; set; }
        public int? SectionId { get; set; }
    }
}
