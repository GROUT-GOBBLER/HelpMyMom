using System.Net.Mail;
using System.Net;
using momUI.models;

namespace momUI
{
    public static class EmailServices
    {
        // Valid statuses: new, assigned, in progress, completed, approved 
        public static void SendNotifcation(String Target, String Name, String status, Ticket ticket)
        {
            string fromEmail = "gingervip66@gmail.com";
            string password = "udyw uyyi lsne kjrt";

            string[] msgBody = new string[]
            {
                $"Hello {Name}!",
                $"Your ticket (#{ticket.Id}) have now been updated to the {status} status, log into the app for additional details.",
                "Thank you for your time, Help My Mom App"
            };

            MailAddress from = new MailAddress(fromEmail);
            MailAddress to = new MailAddress(Target);
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Ticket #" + ticket.Id + " has been updated to " + status;
            message.Body = string.Join("\n", msgBody);

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(fromEmail, password);
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            try
            {
                smtpClient.Send(message);
                Console.WriteLine("Email sent successfully.");
            }
            catch (SmtpFailedRecipientsException ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }
    }
}