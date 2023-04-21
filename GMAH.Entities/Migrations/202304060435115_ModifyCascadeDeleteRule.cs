namespace GMAH.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyCascadeDeleteRule : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ATTENDANCE", "IdStudentClass", "dbo.STUDENT_CLASS");
            DropForeignKey("dbo.STUDENT_CLASS", "IdClass", "dbo.CLASS");
            DropForeignKey("dbo.SCORE", "IdStudentClass", "dbo.STUDENT_CLASS");
            DropForeignKey("dbo.STUDENT_CLASS", "IdStudent", "dbo.STUDENT");
            DropForeignKey("dbo.CLASS_SUBJECT", "IdClass", "dbo.CLASS");
            DropForeignKey("dbo.TIMELINE", "IdClass", "dbo.CLASS");
            DropForeignKey("dbo.CLASS_SUBJECT", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.SCORE_LOG", "IdScore", "dbo.SCORE");
            DropForeignKey("dbo.SCORE", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.USER", "IdRole", "dbo.ROLE");
            DropForeignKey("dbo.HEAD_OF_SUBJECT", "IdTeacher", "dbo.TEACHER");
            DropForeignKey("dbo.TEACHER_SUBJECT", "IdTeacher", "dbo.TEACHER");
            DropForeignKey("dbo.HEAD_OF_SUBJECT", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.TEACHER_SUBJECT", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.TIMELINE", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.TIMELINE", "IdSemester", "dbo.SEMESTER");
            AddForeignKey("dbo.ATTENDANCE", "IdStudentClass", "dbo.STUDENT_CLASS", "IdStudentClass", cascadeDelete: true);
            AddForeignKey("dbo.STUDENT_CLASS", "IdClass", "dbo.CLASS", "IdClass", cascadeDelete: true);
            AddForeignKey("dbo.SCORE", "IdStudentClass", "dbo.STUDENT_CLASS", "IdStudentClass", cascadeDelete: true);
            AddForeignKey("dbo.STUDENT_CLASS", "IdStudent", "dbo.STUDENT", "IdStudent", cascadeDelete: true);
            AddForeignKey("dbo.CLASS_SUBJECT", "IdClass", "dbo.CLASS", "IdClass", cascadeDelete: true);
            AddForeignKey("dbo.TIMELINE", "IdClass", "dbo.CLASS", "IdClass", cascadeDelete: true);
            AddForeignKey("dbo.CLASS_SUBJECT", "IdSubject", "dbo.SUBJECT", "IdSubject", cascadeDelete: true);
            AddForeignKey("dbo.SCORE_LOG", "IdScore", "dbo.SCORE", "IdScore", cascadeDelete: true);
            AddForeignKey("dbo.SCORE", "IdSubject", "dbo.SUBJECT", "IdSubject", cascadeDelete: true);
            AddForeignKey("dbo.USER", "IdRole", "dbo.ROLE", "IdRole", cascadeDelete: true);
            AddForeignKey("dbo.HEAD_OF_SUBJECT", "IdTeacher", "dbo.TEACHER", "IdTeacher", cascadeDelete: true);
            AddForeignKey("dbo.TEACHER_SUBJECT", "IdTeacher", "dbo.TEACHER", "IdTeacher", cascadeDelete: true);
            AddForeignKey("dbo.HEAD_OF_SUBJECT", "IdSubject", "dbo.SUBJECT", "IdSubject", cascadeDelete: true);
            AddForeignKey("dbo.TEACHER_SUBJECT", "IdSubject", "dbo.SUBJECT", "IdSubject", cascadeDelete: true);
            AddForeignKey("dbo.TIMELINE", "IdSubject", "dbo.SUBJECT", "IdSubject", cascadeDelete: true);
            AddForeignKey("dbo.TIMELINE", "IdSemester", "dbo.SEMESTER", "IdSemester", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TIMELINE", "IdSemester", "dbo.SEMESTER");
            DropForeignKey("dbo.TIMELINE", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.TEACHER_SUBJECT", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.HEAD_OF_SUBJECT", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.TEACHER_SUBJECT", "IdTeacher", "dbo.TEACHER");
            DropForeignKey("dbo.HEAD_OF_SUBJECT", "IdTeacher", "dbo.TEACHER");
            DropForeignKey("dbo.USER", "IdRole", "dbo.ROLE");
            DropForeignKey("dbo.SCORE", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.SCORE_LOG", "IdScore", "dbo.SCORE");
            DropForeignKey("dbo.CLASS_SUBJECT", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.TIMELINE", "IdClass", "dbo.CLASS");
            DropForeignKey("dbo.CLASS_SUBJECT", "IdClass", "dbo.CLASS");
            DropForeignKey("dbo.STUDENT_CLASS", "IdStudent", "dbo.STUDENT");
            DropForeignKey("dbo.SCORE", "IdStudentClass", "dbo.STUDENT_CLASS");
            DropForeignKey("dbo.STUDENT_CLASS", "IdClass", "dbo.CLASS");
            DropForeignKey("dbo.ATTENDANCE", "IdStudentClass", "dbo.STUDENT_CLASS");
            AddForeignKey("dbo.TIMELINE", "IdSemester", "dbo.SEMESTER", "IdSemester");
            AddForeignKey("dbo.TIMELINE", "IdSubject", "dbo.SUBJECT", "IdSubject");
            AddForeignKey("dbo.TEACHER_SUBJECT", "IdSubject", "dbo.SUBJECT", "IdSubject");
            AddForeignKey("dbo.HEAD_OF_SUBJECT", "IdSubject", "dbo.SUBJECT", "IdSubject");
            AddForeignKey("dbo.TEACHER_SUBJECT", "IdTeacher", "dbo.TEACHER", "IdTeacher");
            AddForeignKey("dbo.HEAD_OF_SUBJECT", "IdTeacher", "dbo.TEACHER", "IdTeacher");
            AddForeignKey("dbo.USER", "IdRole", "dbo.ROLE", "IdRole");
            AddForeignKey("dbo.SCORE", "IdSubject", "dbo.SUBJECT", "IdSubject");
            AddForeignKey("dbo.SCORE_LOG", "IdScore", "dbo.SCORE", "IdScore");
            AddForeignKey("dbo.CLASS_SUBJECT", "IdSubject", "dbo.SUBJECT", "IdSubject");
            AddForeignKey("dbo.TIMELINE", "IdClass", "dbo.CLASS", "IdClass");
            AddForeignKey("dbo.CLASS_SUBJECT", "IdClass", "dbo.CLASS", "IdClass");
            AddForeignKey("dbo.STUDENT_CLASS", "IdStudent", "dbo.STUDENT", "IdStudent");
            AddForeignKey("dbo.SCORE", "IdStudentClass", "dbo.STUDENT_CLASS", "IdStudentClass");
            AddForeignKey("dbo.STUDENT_CLASS", "IdClass", "dbo.CLASS", "IdClass");
            AddForeignKey("dbo.ATTENDANCE", "IdStudentClass", "dbo.STUDENT_CLASS", "IdStudentClass");
        }
    }
}
