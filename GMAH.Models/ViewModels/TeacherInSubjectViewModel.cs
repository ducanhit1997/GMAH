using System.Collections.Generic;

namespace GMAH.Models.ViewModels
{
    public class TeacherInSubjectViewModel
    {
        public List<SubjectViewModel> Subjects { get; set; }
        public bool IsAllowEditHeadOfSubject { get; set; }
        public List<UserViewModel> HeadOfSubject { get; set; }
        public List<UserViewModel> Teachers { get; set; }
        public List<UserViewModel> HeadOfCurrentSubject { get; set; }
        public List<UserViewModel> TeacherInCurrentSubject { get; set; }
    }
}
