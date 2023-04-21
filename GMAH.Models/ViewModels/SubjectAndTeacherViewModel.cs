using System.Collections.Generic;

namespace GMAH.Models.ViewModels
{
    public class SubjectAndTeacherViewModel
    {
        public SubjectViewModel Subject { get; set; }
        public List<UserViewModel> Teacher { get; set; }
    }
}
