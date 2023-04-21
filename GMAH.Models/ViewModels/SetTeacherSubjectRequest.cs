namespace GMAH.Models.ViewModels
{
    public class SetTeacherSubjectRequest
    {
        public int IdUser { get; set; }
        public int IdSubject { get; set; }
        public int? FromYear { get; set; }
        public int? ToYear { get; set; }
    }
}
