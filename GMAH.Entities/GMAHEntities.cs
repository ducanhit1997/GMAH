using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace GMAH.Entities
{
    public partial class GMAHEntities : DbContext
    {
        public GMAHEntities()
            : base("name=GMAHEntities")
        {
        }

        public virtual DbSet<ATTENDANCE> ATTENDANCEs { get; set; }
        public virtual DbSet<CLASS> CLASSes { get; set; }
        public virtual DbSet<CLASS_SUBJECT> CLASS_SUBJECT { get; set; }
        public virtual DbSet<FIELDSTUDY> FIELDSTUDies { get; set; }
        public virtual DbSet<GRADERULE> GRADERULEs { get; set; }
        public virtual DbSet<GRADERULEDETAIL> GRADERULEDETAILs { get; set; }
        public virtual DbSet<GRADERULELIST> GRADERULELISTs { get; set; }
        public virtual DbSet<HEAD_OF_SUBJECT> HEAD_OF_SUBJECT { get; set; }
        public virtual DbSet<PERMISSION> PERMISSIONs { get; set; }
        public virtual DbSet<REPORT> REPORTs { get; set; }
        public virtual DbSet<REPORT_HISTORY> REPORT_HISTORY { get; set; }
        public virtual DbSet<REPORT_STATUS> REPORT_STATUS { get; set; }
        public virtual DbSet<ROLE> ROLEs { get; set; }
        public virtual DbSet<SCORE> SCOREs { get; set; }
        public virtual DbSet<SCORE_LOG> SCORE_LOG { get; set; }
        public virtual DbSet<SCORE_TYPE> SCORE_TYPE { get; set; }
        public virtual DbSet<REPORT_FILE> REPORT_FILE { get; set; }
        public virtual DbSet<SEMESTER> SEMESTERs { get; set; }
        public virtual DbSet<SEMESTERRANK> SEMESTERRANKs { get; set; }
        public virtual DbSet<STUDENT> STUDENTs { get; set; }
        public virtual DbSet<STUDENT_CLASS> STUDENT_CLASS { get; set; }
        public virtual DbSet<SUBJECT> SUBJECTs { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<SYSTEMSETTING> SYSTEMSETTINGs { get; set; }
        public virtual DbSet<TEACHER> TEACHERs { get; set; }
        public virtual DbSet<TEACHER_SUBJECT> TEACHER_SUBJECT { get; set; }
        public virtual DbSet<USER> USERs { get; set; }
        public virtual DbSet<YEAR> YEARs { get; set; }
        public virtual DbSet<TIMELINE> TIMELINEs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CLASS>()
                .HasMany(e => e.CLASS_SUBJECT)
                .WithRequired(e => e.CLASS)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<CLASS>()
                .HasMany(e => e.STUDENT_CLASS)
                .WithRequired(e => e.CLASS)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<PERMISSION>()
                .Property(e => e.ActionName)
                .IsUnicode(false);

            modelBuilder.Entity<PERMISSION>()
                .HasMany(e => e.ROLEs)
                .WithMany(e => e.PERMISSIONs)
                .Map(m => m.ToTable("ROLE_PERMISSION").MapLeftKey("IdPermission").MapRightKey("IdRole"));

            modelBuilder.Entity<REPORT>()
                .HasMany(e => e.REPORT_HISTORY)
                .WithRequired(e => e.REPORT)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<REPORT>()
                .HasMany(e => e.REPORT_FILE)
                .WithRequired(e => e.REPORT)
                .HasForeignKey(e => e.IdReport)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<REPORT_STATUS>()
                .Property(e => e.StatusName)
                .IsFixedLength();

            modelBuilder.Entity<REPORT_STATUS>()
                .HasMany(e => e.REPORTs)
                .WithRequired(e => e.REPORT_STATUS)
                .HasForeignKey(e => e.ReportStatus)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<REPORT_STATUS>()
                .HasMany(e => e.REPORT_HISTORY)
                .WithRequired(e => e.REPORT_STATUS)
                .HasForeignKey(e => e.ReportStatus)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ROLE>()
                .HasMany(e => e.USERs)
                .WithRequired(e => e.ROLE)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<SCORE>()
                .HasMany(e => e.SCORE_LOG)
                .WithRequired(e => e.SCORE)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<SEMESTER>()
                .HasMany(e => e.SCOREs)
                .WithOptional(e => e.SEMESTER)
                .WillCascadeOnDelete();

            modelBuilder.Entity<SEMESTER>()
                .HasMany(e => e.SEMESTERRANKs)
                .WithOptional(e => e.SEMESTER)
                .WillCascadeOnDelete();

            modelBuilder.Entity<STUDENT>()
                .Property(e => e.StudentCode)
                .IsUnicode(false);

            modelBuilder.Entity<STUDENT>()
                .HasMany(e => e.STUDENT_CLASS)
                .WithRequired(e => e.STUDENT)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<STUDENT>()
                .HasMany(e => e.USERs)
                .WithMany(e => e.STUDENTs1)
                .Map(m => m.ToTable("PARENT_STUDENT").MapLeftKey("IdStudent").MapRightKey("IdUser"));

            modelBuilder.Entity<STUDENT_CLASS>()
                .HasMany(e => e.ATTENDANCEs)
                .WithRequired(e => e.STUDENT_CLASS)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<STUDENT_CLASS>()
                .HasMany(e => e.SCOREs)
                .WithRequired(e => e.STUDENT_CLASS)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<SUBJECT>()
                .Property(e => e.SubjectCode)
                .IsUnicode(false);

            modelBuilder.Entity<SUBJECT>()
                .HasMany(e => e.CLASS_SUBJECT)
                .WithRequired(e => e.SUBJECT)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<SUBJECT>()
                .HasMany(e => e.GRADERULEDETAILs)
                .WithOptional(e => e.SUBJECT)
                .WillCascadeOnDelete();

            modelBuilder.Entity<SUBJECT>()
                .HasMany(e => e.HEAD_OF_SUBJECT)
                .WithRequired(e => e.SUBJECT)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<SUBJECT>()
                .HasMany(e => e.SCOREs)
                .WithRequired(e => e.SUBJECT)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<SUBJECT>()
                .HasMany(e => e.TEACHER_SUBJECT)
                .WithRequired(e => e.SUBJECT)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<SUBJECT>()
                .HasMany(e => e.TIMELINEs)
                .WithRequired(e => e.SUBJECT)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<CLASS>()
                .HasMany(e => e.TIMELINEs)
                .WithRequired(e => e.CLASS)
                .HasForeignKey(x => x.IdClass)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<SEMESTER>()
                .HasMany(e => e.TIMELINEs)
                .WithRequired(e => e.SEMESTER)
                .HasForeignKey(x => x.IdSemester)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<SYSTEMSETTING>()
                .Property(e => e.SettingKey)
                .IsUnicode(false);

            modelBuilder.Entity<TEACHER>()
                .HasMany(e => e.CLASSes)
                .WithOptional(e => e.TEACHER)
                .HasForeignKey(e => e.IdFormTeacher);

            modelBuilder.Entity<TEACHER>()
                .HasMany(e => e.HEAD_OF_SUBJECT)
                .WithRequired(e => e.TEACHER)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<TEACHER>()
                .HasMany(e => e.TEACHER_SUBJECT)
                .WithRequired(e => e.TEACHER)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<USER>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<USER>()
                .Property(e => e.HashPassword)
                .IsUnicode(false);

            modelBuilder.Entity<USER>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<USER>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<USER>()
                .HasMany(e => e.ATTENDANCEs)
                .WithRequired(e => e.USER)
                .HasForeignKey(e => e.AssistantID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<USER>()
                .HasMany(e => e.REPORTs)
                .WithRequired(e => e.USER)
                .HasForeignKey(e => e.IdUserSubmitReport)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<USER>()
                .HasMany(e => e.MYREPORTs)
                .WithRequired(e => e.STUDENTUSER)
                .HasForeignKey(e => e.SubmitForIdUser)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<USER>()
                .HasMany(e => e.REPORT_HISTORY)
                .WithRequired(e => e.USER)
                .HasForeignKey(e => e.IdUserUpdate)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<USER>()
                .HasMany(e => e.SCORE_LOG)
                .WithRequired(e => e.USER)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<USER>()
                .HasMany(e => e.STUDENTs)
                .WithRequired(e => e.USER)
                .HasForeignKey(e => e.IdUser)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<USER>()
                .HasMany(e => e.TEACHERs)
                .WithRequired(e => e.USER)
                .WillCascadeOnDelete(false);
        }
    }
}
