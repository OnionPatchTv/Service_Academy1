namespace Service_Academy1.Models
{
    public class ProgramListViewModel
    {
        public IEnumerable<ProgramsModel> Programs { get; set; } = [];
        public List<EnrollmentModel> UserEnrollments { get; set; } = [];
    }

}
