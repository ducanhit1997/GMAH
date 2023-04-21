namespace GMAH.Models.ViewModels
{
    public class SaveStudentBehaviourRequest
    {
        public int IdUser { get; set; }
        public int? IdSemester { get; set; }
        public int IdYear { get; set; }
        public int? IdBehaviour { get; set; }
    }
}
