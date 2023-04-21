namespace GMAH.Entities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDBv2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ATTENDANCE",
                c => new
                    {
                        IdAttendance = c.Int(nullable: false, identity: true),
                        IdStudentClass = c.Int(nullable: false),
                        CheckinTime = c.DateTime(),
                        DateAttendance = c.DateTime(storeType: "date"),
                        IsAvailable = c.Boolean(),
                        IsLeavePermission = c.Boolean(),
                        AssistantID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdAttendance)
                .ForeignKey("dbo.STUDENT_CLASS", t => t.IdStudentClass)
                .ForeignKey("dbo.USER", t => t.AssistantID)
                .Index(t => t.IdStudentClass)
                .Index(t => t.AssistantID);
            
            CreateTable(
                "dbo.STUDENT_CLASS",
                c => new
                    {
                        IdStudentClass = c.Int(nullable: false, identity: true),
                        IdStudent = c.Int(nullable: false),
                        IdClass = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdStudentClass)
                .ForeignKey("dbo.STUDENT", t => t.IdStudent)
                .ForeignKey("dbo.CLASS", t => t.IdClass)
                .Index(t => t.IdStudent)
                .Index(t => t.IdClass);
            
            CreateTable(
                "dbo.CLASS",
                c => new
                    {
                        IdClass = c.Int(nullable: false, identity: true),
                        ClassName = c.String(maxLength: 50),
                        IdFormTeacher = c.Int(),
                        IdRule = c.Int(),
                        IdField = c.Int(),
                        IdYear = c.Int(),
                    })
                .PrimaryKey(t => t.IdClass)
                .ForeignKey("dbo.TEACHER", t => t.IdFormTeacher)
                .ForeignKey("dbo.GRADERULE", t => t.IdRule)
                .ForeignKey("dbo.YEAR", t => t.IdYear)
                .ForeignKey("dbo.FIELDSTUDY", t => t.IdField)
                .Index(t => t.IdFormTeacher)
                .Index(t => t.IdRule)
                .Index(t => t.IdField)
                .Index(t => t.IdYear);
            
            CreateTable(
                "dbo.CLASS_SUBJECT",
                c => new
                    {
                        IdClassSubject = c.Int(nullable: false, identity: true),
                        IdClass = c.Int(nullable: false),
                        IdTeacherSubject = c.Int(),
                        IdSubject = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdClassSubject)
                .ForeignKey("dbo.SUBJECT", t => t.IdSubject)
                .ForeignKey("dbo.TEACHER_SUBJECT", t => t.IdTeacherSubject)
                .ForeignKey("dbo.CLASS", t => t.IdClass)
                .Index(t => t.IdClass)
                .Index(t => t.IdTeacherSubject)
                .Index(t => t.IdSubject);
            
            CreateTable(
                "dbo.SCORE_TYPE",
                c => new
                    {
                        IdScoreType = c.Int(nullable: false, identity: true),
                        ScoreName = c.String(maxLength: 50),
                        ScoreWeight = c.Byte(),
                        IdClassSubject = c.Int(),
                    })
                .PrimaryKey(t => t.IdScoreType)
                .ForeignKey("dbo.CLASS_SUBJECT", t => t.IdClassSubject)
                .Index(t => t.IdClassSubject);
            
            CreateTable(
                "dbo.SCORE",
                c => new
                    {
                        IdScore = c.Int(nullable: false, identity: true),
                        IdSubject = c.Int(nullable: false),
                        IdStudentClass = c.Int(nullable: false),
                        Score = c.Double(),
                        IdSemester = c.Int(),
                        IdScoreType = c.Int(),
                        ScoreNote = c.String(maxLength: 500),
                        IdYear = c.Int(),
                    })
                .PrimaryKey(t => t.IdScore)
                .ForeignKey("dbo.SEMESTER", t => t.IdSemester, cascadeDelete: true)
                .ForeignKey("dbo.YEAR", t => t.IdYear)
                .ForeignKey("dbo.SUBJECT", t => t.IdSubject)
                .ForeignKey("dbo.SCORE_TYPE", t => t.IdScoreType)
                .ForeignKey("dbo.STUDENT_CLASS", t => t.IdStudentClass)
                .Index(t => t.IdSubject)
                .Index(t => t.IdStudentClass)
                .Index(t => t.IdSemester)
                .Index(t => t.IdScoreType)
                .Index(t => t.IdYear);
            
            CreateTable(
                "dbo.SCORE_LOG",
                c => new
                    {
                        IdScoreLog = c.Int(nullable: false, identity: true),
                        IdUser = c.Int(nullable: false),
                        IdScore = c.Int(nullable: false),
                        LogMessage = c.String(storeType: "ntext"),
                        CreatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.IdScoreLog)
                .ForeignKey("dbo.USER", t => t.IdUser)
                .ForeignKey("dbo.SCORE", t => t.IdScore)
                .Index(t => t.IdUser)
                .Index(t => t.IdScore);
            
            CreateTable(
                "dbo.USER",
                c => new
                    {
                        IdUser = c.Int(nullable: false, identity: true),
                        IdRole = c.Int(nullable: false),
                        Username = c.String(nullable: false, maxLength: 50, unicode: false),
                        HashPassword = c.String(maxLength: 255, unicode: false),
                        Fullname = c.String(maxLength: 50),
                        Email = c.String(maxLength: 50, unicode: false),
                        Phone = c.String(maxLength: 10, unicode: false),
                        IsDeleted = c.Boolean(),
                        CitizenId = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.IdUser)
                .ForeignKey("dbo.ROLE", t => t.IdRole)
                .Index(t => t.IdRole);
            
            CreateTable(
                "dbo.REPORT_HISTORY",
                c => new
                    {
                        IdReportHistory = c.Int(nullable: false, identity: true),
                        HistoryDate = c.DateTime(),
                        IdUserUpdate = c.Int(nullable: false),
                        ReportStatus = c.Int(nullable: false),
                        Comment = c.String(storeType: "ntext"),
                        IdReport = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdReportHistory)
                .ForeignKey("dbo.REPORT", t => t.IdReport)
                .ForeignKey("dbo.REPORT_STATUS", t => t.ReportStatus)
                .ForeignKey("dbo.USER", t => t.IdUserUpdate)
                .Index(t => t.IdUserUpdate)
                .Index(t => t.ReportStatus)
                .Index(t => t.IdReport);
            
            CreateTable(
                "dbo.REPORT",
                c => new
                    {
                        IdReport = c.Int(nullable: false, identity: true),
                        ReportType = c.Int(),
                        ReportTitle = c.String(maxLength: 255),
                        ReportContent = c.String(storeType: "ntext"),
                        ReportStatus = c.Int(nullable: false),
                        IdUserSubmitReport = c.Int(nullable: false),
                        SubmitDate = c.DateTime(),
                        LastUpdateDate = c.DateTime(),
                        EditField = c.String(maxLength: 500),
                        EditValue = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.IdReport)
                .ForeignKey("dbo.REPORT_STATUS", t => t.ReportStatus)
                .ForeignKey("dbo.USER", t => t.IdUserSubmitReport)
                .Index(t => t.ReportStatus)
                .Index(t => t.IdUserSubmitReport);
            
            CreateTable(
                "dbo.REPORT_STATUS",
                c => new
                    {
                        IdReportStatus = c.Int(nullable: false),
                        StatusName = c.String(maxLength: 10, fixedLength: true),
                    })
                .PrimaryKey(t => t.IdReportStatus);
            
            CreateTable(
                "dbo.ROLE",
                c => new
                    {
                        IdRole = c.Int(nullable: false),
                        RoleName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.IdRole);
            
            CreateTable(
                "dbo.PERMISSION",
                c => new
                    {
                        IdPermission = c.Int(nullable: false, identity: true),
                        ActionName = c.String(maxLength: 255, unicode: false),
                    })
                .PrimaryKey(t => t.IdPermission);
            
            CreateTable(
                "dbo.STUDENT",
                c => new
                    {
                        IdStudent = c.Int(nullable: false, identity: true),
                        IdUser = c.Int(nullable: false),
                        StudentCode = c.String(maxLength: 20, unicode: false),
                    })
                .PrimaryKey(t => t.IdStudent)
                .ForeignKey("dbo.USER", t => t.IdUser)
                .Index(t => t.IdUser);
            
            CreateTable(
                "dbo.TEACHER",
                c => new
                    {
                        IdTeacher = c.Int(nullable: false, identity: true),
                        IdUser = c.Int(nullable: false),
                        TeacherCode = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.IdTeacher)
                .ForeignKey("dbo.USER", t => t.IdUser)
                .Index(t => t.IdUser);
            
            CreateTable(
                "dbo.HEAD_OF_SUBJECT",
                c => new
                    {
                        IdHeadOfSubject = c.Int(nullable: false, identity: true),
                        IdTeacher = c.Int(nullable: false),
                        IdSubject = c.Int(nullable: false),
                        FromYear = c.Int(),
                        ToYear = c.Int(),
                    })
                .PrimaryKey(t => t.IdHeadOfSubject)
                .ForeignKey("dbo.SUBJECT", t => t.IdSubject)
                .ForeignKey("dbo.TEACHER", t => t.IdTeacher)
                .Index(t => t.IdTeacher)
                .Index(t => t.IdSubject);
            
            CreateTable(
                "dbo.SUBJECT",
                c => new
                    {
                        IdSubject = c.Int(nullable: false, identity: true),
                        SubjectName = c.String(maxLength: 50),
                        SubjectCode = c.String(maxLength: 20, unicode: false),
                    })
                .PrimaryKey(t => t.IdSubject);
            
            CreateTable(
                "dbo.GRADERULEDETAIL",
                c => new
                    {
                        IdRuleDetail = c.Int(nullable: false, identity: true),
                        IdRuleList = c.Int(nullable: false),
                        IdSubject = c.Int(),
                        MinAvgScore = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.IdRuleDetail)
                .ForeignKey("dbo.GRADERULELIST", t => t.IdRuleList, cascadeDelete: true)
                .ForeignKey("dbo.SUBJECT", t => t.IdSubject, cascadeDelete: true)
                .Index(t => t.IdRuleList)
                .Index(t => t.IdSubject);
            
            CreateTable(
                "dbo.GRADERULELIST",
                c => new
                    {
                        IdRuleList = c.Int(nullable: false, identity: true),
                        IdRule = c.Int(nullable: false),
                        IdRank = c.Int(nullable: false),
                        MinAvgScore = c.Double(),
                        IdBehaviour = c.Int(),
                    })
                .PrimaryKey(t => t.IdRuleList)
                .ForeignKey("dbo.GRADERULE", t => t.IdRule, cascadeDelete: true)
                .Index(t => t.IdRule);
            
            CreateTable(
                "dbo.GRADERULE",
                c => new
                    {
                        IdRule = c.Int(nullable: false, identity: true),
                        IdSemester = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdRule)
                .ForeignKey("dbo.SEMESTER", t => t.IdSemester, cascadeDelete: true)
                .Index(t => t.IdSemester);
            
            CreateTable(
                "dbo.SEMESTER",
                c => new
                    {
                        IdSemester = c.Int(nullable: false, identity: true),
                        SemesterName = c.String(maxLength: 50),
                        IsCurrentSemester = c.Boolean(),
                        DateStart = c.DateTime(storeType: "smalldatetime"),
                        DateEnd = c.DateTime(storeType: "smalldatetime"),
                        ScoreWeight = c.Int(),
                        IdYear = c.Int(),
                    })
                .PrimaryKey(t => t.IdSemester)
                .ForeignKey("dbo.YEAR", t => t.IdYear)
                .Index(t => t.IdYear);
            
            CreateTable(
                "dbo.SEMESTERRANK",
                c => new
                    {
                        IdSemesterRank = c.Int(nullable: false, identity: true),
                        IdSemester = c.Int(),
                        IdYear = c.Int(),
                        IdStudentClass = c.Int(nullable: false),
                        AvgScore = c.Double(),
                        IdRank = c.Int(),
                        IdBehaviour = c.Int(),
                    })
                .PrimaryKey(t => t.IdSemesterRank)
                .ForeignKey("dbo.STUDENT_CLASS", t => t.IdStudentClass, cascadeDelete: true)
                .ForeignKey("dbo.YEAR", t => t.IdYear)
                .ForeignKey("dbo.SEMESTER", t => t.IdSemester, cascadeDelete: true)
                .Index(t => t.IdSemester)
                .Index(t => t.IdYear)
                .Index(t => t.IdStudentClass);
            
            CreateTable(
                "dbo.YEAR",
                c => new
                    {
                        IdYear = c.Int(nullable: false, identity: true),
                        YearName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.IdYear);
            
            CreateTable(
                "dbo.TEACHER_SUBJECT",
                c => new
                    {
                        IdTeacherSubject = c.Int(nullable: false, identity: true),
                        IdTeacher = c.Int(nullable: false),
                        IdSubject = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdTeacherSubject)
                .ForeignKey("dbo.SUBJECT", t => t.IdSubject)
                .ForeignKey("dbo.TEACHER", t => t.IdTeacher)
                .Index(t => t.IdTeacher)
                .Index(t => t.IdSubject);
            
            CreateTable(
                "dbo.TIMELINE",
                c => new
                    {
                        IdSchedule = c.Int(nullable: false),
                        IdSubject = c.Int(nullable: false),
                        Period = c.Int(),
                        Date = c.DateTime(storeType: "date"),
                    })
                .PrimaryKey(t => new { t.IdSchedule, t.IdSubject })
                .ForeignKey("dbo.SUBJECT", t => t.IdSubject)
                .Index(t => t.IdSubject);
            
            CreateTable(
                "dbo.FIELDSTUDY",
                c => new
                    {
                        IdField = c.Int(nullable: false, identity: true),
                        FieldName = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.IdField);
            
            CreateTable(
                "dbo.sysdiagrams",
                c => new
                    {
                        diagram_id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 128),
                        principal_id = c.Int(nullable: false),
                        version = c.Int(),
                        definition = c.Binary(),
                    })
                .PrimaryKey(t => t.diagram_id);
            
            CreateTable(
                "dbo.SYSTEMSETTING",
                c => new
                    {
                        SettingKey = c.String(nullable: false, maxLength: 100, unicode: false),
                        SettingValue = c.String(storeType: "ntext"),
                        InputType = c.String(maxLength: 50),
                        SettingName = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.SettingKey);
            
            CreateTable(
                "dbo.ROLE_PERMISSION",
                c => new
                    {
                        IdPermission = c.Int(nullable: false),
                        IdRole = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IdPermission, t.IdRole })
                .ForeignKey("dbo.PERMISSION", t => t.IdPermission, cascadeDelete: true)
                .ForeignKey("dbo.ROLE", t => t.IdRole, cascadeDelete: true)
                .Index(t => t.IdPermission)
                .Index(t => t.IdRole);
            
            CreateTable(
                "dbo.PARENT_STUDENT",
                c => new
                    {
                        IdStudent = c.Int(nullable: false),
                        IdUser = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.IdStudent, t.IdUser })
                .ForeignKey("dbo.STUDENT", t => t.IdStudent, cascadeDelete: true)
                .ForeignKey("dbo.USER", t => t.IdUser, cascadeDelete: true)
                .Index(t => t.IdStudent)
                .Index(t => t.IdUser);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SCORE", "IdStudentClass", "dbo.STUDENT_CLASS");
            DropForeignKey("dbo.STUDENT_CLASS", "IdClass", "dbo.CLASS");
            DropForeignKey("dbo.CLASS", "IdField", "dbo.FIELDSTUDY");
            DropForeignKey("dbo.CLASS_SUBJECT", "IdClass", "dbo.CLASS");
            DropForeignKey("dbo.SCORE", "IdScoreType", "dbo.SCORE_TYPE");
            DropForeignKey("dbo.SCORE_LOG", "IdScore", "dbo.SCORE");
            DropForeignKey("dbo.TEACHER", "IdUser", "dbo.USER");
            DropForeignKey("dbo.TEACHER_SUBJECT", "IdTeacher", "dbo.TEACHER");
            DropForeignKey("dbo.HEAD_OF_SUBJECT", "IdTeacher", "dbo.TEACHER");
            DropForeignKey("dbo.TIMELINE", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.TEACHER_SUBJECT", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.CLASS_SUBJECT", "IdTeacherSubject", "dbo.TEACHER_SUBJECT");
            DropForeignKey("dbo.SCORE", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.HEAD_OF_SUBJECT", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.GRADERULEDETAIL", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.GRADERULEDETAIL", "IdRuleList", "dbo.GRADERULELIST");
            DropForeignKey("dbo.SEMESTERRANK", "IdSemester", "dbo.SEMESTER");
            DropForeignKey("dbo.SEMESTER", "IdYear", "dbo.YEAR");
            DropForeignKey("dbo.SEMESTERRANK", "IdYear", "dbo.YEAR");
            DropForeignKey("dbo.SCORE", "IdYear", "dbo.YEAR");
            DropForeignKey("dbo.CLASS", "IdYear", "dbo.YEAR");
            DropForeignKey("dbo.SEMESTERRANK", "IdStudentClass", "dbo.STUDENT_CLASS");
            DropForeignKey("dbo.SCORE", "IdSemester", "dbo.SEMESTER");
            DropForeignKey("dbo.GRADERULE", "IdSemester", "dbo.SEMESTER");
            DropForeignKey("dbo.GRADERULELIST", "IdRule", "dbo.GRADERULE");
            DropForeignKey("dbo.CLASS", "IdRule", "dbo.GRADERULE");
            DropForeignKey("dbo.CLASS_SUBJECT", "IdSubject", "dbo.SUBJECT");
            DropForeignKey("dbo.CLASS", "IdFormTeacher", "dbo.TEACHER");
            DropForeignKey("dbo.STUDENT", "IdUser", "dbo.USER");
            DropForeignKey("dbo.PARENT_STUDENT", "IdUser", "dbo.USER");
            DropForeignKey("dbo.PARENT_STUDENT", "IdStudent", "dbo.STUDENT");
            DropForeignKey("dbo.STUDENT_CLASS", "IdStudent", "dbo.STUDENT");
            DropForeignKey("dbo.SCORE_LOG", "IdUser", "dbo.USER");
            DropForeignKey("dbo.USER", "IdRole", "dbo.ROLE");
            DropForeignKey("dbo.ROLE_PERMISSION", "IdRole", "dbo.ROLE");
            DropForeignKey("dbo.ROLE_PERMISSION", "IdPermission", "dbo.PERMISSION");
            DropForeignKey("dbo.REPORT", "IdUserSubmitReport", "dbo.USER");
            DropForeignKey("dbo.REPORT_HISTORY", "IdUserUpdate", "dbo.USER");
            DropForeignKey("dbo.REPORT", "ReportStatus", "dbo.REPORT_STATUS");
            DropForeignKey("dbo.REPORT_HISTORY", "ReportStatus", "dbo.REPORT_STATUS");
            DropForeignKey("dbo.REPORT_HISTORY", "IdReport", "dbo.REPORT");
            DropForeignKey("dbo.ATTENDANCE", "AssistantID", "dbo.USER");
            DropForeignKey("dbo.SCORE_TYPE", "IdClassSubject", "dbo.CLASS_SUBJECT");
            DropForeignKey("dbo.ATTENDANCE", "IdStudentClass", "dbo.STUDENT_CLASS");
            DropIndex("dbo.PARENT_STUDENT", new[] { "IdUser" });
            DropIndex("dbo.PARENT_STUDENT", new[] { "IdStudent" });
            DropIndex("dbo.ROLE_PERMISSION", new[] { "IdRole" });
            DropIndex("dbo.ROLE_PERMISSION", new[] { "IdPermission" });
            DropIndex("dbo.TIMELINE", new[] { "IdSubject" });
            DropIndex("dbo.TEACHER_SUBJECT", new[] { "IdSubject" });
            DropIndex("dbo.TEACHER_SUBJECT", new[] { "IdTeacher" });
            DropIndex("dbo.SEMESTERRANK", new[] { "IdStudentClass" });
            DropIndex("dbo.SEMESTERRANK", new[] { "IdYear" });
            DropIndex("dbo.SEMESTERRANK", new[] { "IdSemester" });
            DropIndex("dbo.SEMESTER", new[] { "IdYear" });
            DropIndex("dbo.GRADERULE", new[] { "IdSemester" });
            DropIndex("dbo.GRADERULELIST", new[] { "IdRule" });
            DropIndex("dbo.GRADERULEDETAIL", new[] { "IdSubject" });
            DropIndex("dbo.GRADERULEDETAIL", new[] { "IdRuleList" });
            DropIndex("dbo.HEAD_OF_SUBJECT", new[] { "IdSubject" });
            DropIndex("dbo.HEAD_OF_SUBJECT", new[] { "IdTeacher" });
            DropIndex("dbo.TEACHER", new[] { "IdUser" });
            DropIndex("dbo.STUDENT", new[] { "IdUser" });
            DropIndex("dbo.REPORT", new[] { "IdUserSubmitReport" });
            DropIndex("dbo.REPORT", new[] { "ReportStatus" });
            DropIndex("dbo.REPORT_HISTORY", new[] { "IdReport" });
            DropIndex("dbo.REPORT_HISTORY", new[] { "ReportStatus" });
            DropIndex("dbo.REPORT_HISTORY", new[] { "IdUserUpdate" });
            DropIndex("dbo.USER", new[] { "IdRole" });
            DropIndex("dbo.SCORE_LOG", new[] { "IdScore" });
            DropIndex("dbo.SCORE_LOG", new[] { "IdUser" });
            DropIndex("dbo.SCORE", new[] { "IdYear" });
            DropIndex("dbo.SCORE", new[] { "IdScoreType" });
            DropIndex("dbo.SCORE", new[] { "IdSemester" });
            DropIndex("dbo.SCORE", new[] { "IdStudentClass" });
            DropIndex("dbo.SCORE", new[] { "IdSubject" });
            DropIndex("dbo.SCORE_TYPE", new[] { "IdClassSubject" });
            DropIndex("dbo.CLASS_SUBJECT", new[] { "IdSubject" });
            DropIndex("dbo.CLASS_SUBJECT", new[] { "IdTeacherSubject" });
            DropIndex("dbo.CLASS_SUBJECT", new[] { "IdClass" });
            DropIndex("dbo.CLASS", new[] { "IdYear" });
            DropIndex("dbo.CLASS", new[] { "IdField" });
            DropIndex("dbo.CLASS", new[] { "IdRule" });
            DropIndex("dbo.CLASS", new[] { "IdFormTeacher" });
            DropIndex("dbo.STUDENT_CLASS", new[] { "IdClass" });
            DropIndex("dbo.STUDENT_CLASS", new[] { "IdStudent" });
            DropIndex("dbo.ATTENDANCE", new[] { "AssistantID" });
            DropIndex("dbo.ATTENDANCE", new[] { "IdStudentClass" });
            DropTable("dbo.PARENT_STUDENT");
            DropTable("dbo.ROLE_PERMISSION");
            DropTable("dbo.SYSTEMSETTING");
            DropTable("dbo.sysdiagrams");
            DropTable("dbo.FIELDSTUDY");
            DropTable("dbo.TIMELINE");
            DropTable("dbo.TEACHER_SUBJECT");
            DropTable("dbo.YEAR");
            DropTable("dbo.SEMESTERRANK");
            DropTable("dbo.SEMESTER");
            DropTable("dbo.GRADERULE");
            DropTable("dbo.GRADERULELIST");
            DropTable("dbo.GRADERULEDETAIL");
            DropTable("dbo.SUBJECT");
            DropTable("dbo.HEAD_OF_SUBJECT");
            DropTable("dbo.TEACHER");
            DropTable("dbo.STUDENT");
            DropTable("dbo.PERMISSION");
            DropTable("dbo.ROLE");
            DropTable("dbo.REPORT_STATUS");
            DropTable("dbo.REPORT");
            DropTable("dbo.REPORT_HISTORY");
            DropTable("dbo.USER");
            DropTable("dbo.SCORE_LOG");
            DropTable("dbo.SCORE");
            DropTable("dbo.SCORE_TYPE");
            DropTable("dbo.CLASS_SUBJECT");
            DropTable("dbo.CLASS");
            DropTable("dbo.STUDENT_CLASS");
            DropTable("dbo.ATTENDANCE");
        }
    }
}
