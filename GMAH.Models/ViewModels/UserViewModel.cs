namespace GMAH.Models.ViewModels
{
    public class UserViewModel
    {
        public int IdUser { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Role { get; set; }
        public int IdRole { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? IdTeacherSubject { get; set; }
        public string CitizenID { get; set; }
        public string Address { get; set; }
        public int? FromYear { get; set; }
        public int? ToYear { get; set; }
    }
}
