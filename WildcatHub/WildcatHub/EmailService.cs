
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace WildcatHub
    {
        public static class EmailService
        {
            public static string SmtpHost = "smtp.gmail.com";
            public static int SmtpPort = 587;

            public static string SmtpUser = "wildcathub.cit@gmail.com";
            public static string SmtpPassword = "fdovfvgdzspsjyks";

            public static string FromName = "WildCatHub";
            public static string FromEmail = "wildcathub.cit@gmail.com";

        public static void SendVerificationCode(string toEmail, string toName, string code)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(FromName, FromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = "WildcatHub Verification Code";

            message.Body = new TextPart("html")
            {
                Text = $@"
<div style='font-family:Segoe UI,Arial,sans-serif;max-width:520px;margin:auto;padding:24px;background:#ffffff;border:1px solid #eee;border-radius:12px;'>
    <h2 style='color:#6f4a8e;margin-bottom:6px;'>WildcatHub</h2>
    <p style='color:#555;margin-top:0;'>Email Verification</p>
    <hr style='border:none;border-top:1px solid #eee;margin:18px 0;' />
    <p>Hello <strong>{toName}</strong>,</p>
    <p>Your verification code is:</p>
    <div style='margin:24px 0;padding:18px;text-align:center;background:#f6eff8;border:2px solid #d8c2e3;border-radius:10px;'>
        <span style='font-size:34px;font-weight:bold;letter-spacing:8px;color:#6f4a8e;'>{code}</span>
    </div>
    <p>This code expires in <strong>2 minutes</strong>.</p>
    <p style='font-size:12px;color:#888;'>If you did not request this, you may ignore this email.</p>
</div>"
            };

            using var client = new SmtpClient();
            client.Connect(SmtpHost, SmtpPort, SecureSocketOptions.StartTls);
            client.Authenticate(SmtpUser, SmtpPassword);
            client.Send(message);
            client.Disconnect(true);
        }




        public static void SendEquipmentDeletedReservationNotice(
    string toEmail,
    string fullName,
    string equipmentName,
    int quantityReserved,
    DateTime reservationDate)
        {
            string subject = "WildcatHub Reservation Cancelled";

            string body = $@"
Hello {fullName},

We would like to inform you that the equipment item ""{equipmentName}"" has been removed by the administrator.

Because of this, your reservation has been cancelled.

Reservation Details:
- Item: {equipmentName}
- Quantity: {quantityReserved}
- Reservation Date: {reservationDate:MMMM dd, yyyy}

You may wait in case the administrator adds the equipment again, or reserve a different available equipment instead.

Thank you,
WildcatHub
";

            SendEmail(toEmail, subject, body);
        }





        private static void SendEmail(string toEmail, string subject, string body)
        {
            string senderEmail = "wildcathub.cit@gmail.com";
            string senderPassword = "fdovfvgdzspsjyks";

            using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
            {
                mail.From = new System.Net.Mail.MailAddress(senderEmail, "WildcatHub");
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;

                using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }



        public static void SendClaimSuccessEmail(
    string toEmail,
    string fullName,
    string equipmentName,
    int quantityBorrowed,
    DateTime borrowDate,
    DateTime dueDate)
        {
            string subject = "WildcatHub - Equipment Claimed Successfully";

            string body = $@"
Hello {fullName},

Your reservation has been successfully claimed. Please return the item on or before the due date to avoid penalties.

Borrowing Details:
- Item: {equipmentName}
- Quantity: {quantityBorrowed}
- Date Claimed: {borrowDate:MMMM dd, yyyy}
- Due Date: {dueDate:MMMM dd, yyyy}

Penalty for late returns is ₱10 per day.

Thank you,
WildcatHub";

            SendEmail(toEmail, subject, body);
        }


        public static void SendMaintenanceNotice(
            string toEmail,
            string fullName,
            string equipmentName,
            int quantityReserved,
            DateTime reservationDate)
        {
            string subject = "WildcatHub - Reservation Cancelled (Maintenance)";

            string body = $@"
Hello {fullName},

We regret to inform you that the equipment item ""{equipmentName}"" has been temporarily placed under maintenance by the administrator.

As a result, your reservation has been cancelled.

Reservation Details:
- Item: {equipmentName}
- Quantity: {quantityReserved}
- Reservation Date: {reservationDate:MMMM dd, yyyy}

You may reserve a different available equipment in the meantime. We apologize for the inconvenience.

Thank you,
WildcatHub";

            SendEmail(toEmail, subject, body);
        }


        public static void SendOverdueReminderEmail(
            string toEmail,
            string fullName,
            string equipmentName,
            int quantityBorrowed,
            DateTime dueDate,
            int overdueDays,
            decimal penaltyAmount)
        {
            string subject = "WildcatHub - Overdue Item Reminder";

            string body = $@"
Hello {fullName},

This is a reminder that you have an overdue borrowed item. Please return it as soon as possible to avoid further penalties.

Overdue Details:
- Item: {equipmentName}
- Quantity: {quantityBorrowed}
- Due Date: {dueDate:MMMM dd, yyyy}
- Days Overdue: {overdueDays} day(s)
- Current Penalty: ₱{penaltyAmount:0.00}

Please return the item immediately to the equipment office.

Thank you,
WildcatHub";

            SendEmail(toEmail, subject, body);
        }





    }
    }