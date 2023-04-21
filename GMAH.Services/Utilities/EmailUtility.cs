using System.Net.Mail;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace GMAH.Services.Utilities
{
    /// <summary>
    /// Hàm gửi mail
    /// </summary>
    public class EmailUtility
    {
        private string emailSenderName;
        private string emailUser;
        private string emailPassword;
        private string emailSMTP;
        private string emailSMTPPort;

        public EmailUtility(string senderName, string username, string password, string smtp, string port)
        {
            emailSenderName = senderName;
            emailUser = username;
            emailPassword = password;
            emailSMTP = smtp;
            emailSMTPPort = port;
        }

        public void Send(string toEmail, string subject, string body, string filePath = null)
        {
            var smtp = new SmtpClient
            {
                Host = emailSMTP,
                Port = int.Parse(emailSMTPPort),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailUser, emailPassword)
            };

            // add from,to mailaddresses
            MailAddress from = new MailAddress(emailUser, emailSenderName);
            MailAddress to = new MailAddress(toEmail);
            MailMessage myMail = new MailMessage(from, to);

            // set subject and encoding
            myMail.Subject = subject;
            myMail.SubjectEncoding = Encoding.UTF8;

            // set body-message and encoding
            myMail.Body = body;
            myMail.BodyEncoding = Encoding.UTF8;

            if (!string.IsNullOrEmpty(filePath))
            {
                myMail.Attachments.Add(new Attachment(filePath));
            }

            // text or html
            myMail.IsBodyHtml = true;

            // Send mail
            smtp.Send(myMail);
        }

        // Gửi nhiều mail
        public void Send(List<string> toEmail, string subject, string body, string filePath = null)
        {
            var smtp = new SmtpClient
            {
                Host = emailSMTP,
                Port = int.Parse(emailSMTPPort),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailUser, emailPassword)
            };

            // add from,to mailaddresses
            MailAddress from = new MailAddress(emailUser, emailSenderName);
            
            MailMessage myMail = new MailMessage();
            myMail.From = from;

            foreach (var email in toEmail)
            {
                MailAddress to = new MailAddress(email);
                myMail.To.Add(to);
            }

            // set subject and encoding
            myMail.Subject = subject;
            myMail.SubjectEncoding = Encoding.UTF8;

            // set body-message and encoding
            myMail.Body = body;
            myMail.BodyEncoding = Encoding.UTF8;

            if (!string.IsNullOrEmpty(filePath))
            {
                myMail.Attachments.Add(new Attachment(filePath));
            }

            // text or html
            myMail.IsBodyHtml = true;

            // Send mail
            smtp.Send(myMail);
        }
    }
}
