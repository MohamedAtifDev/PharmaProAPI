using GraduationProjectAPI.BL.VM;
using Microsoft.Identity.Client;
using MimeKit;

namespace GraduationProjectAPI.BL.Services
{
    public class PrescrptionSender
    {
        public static async Task<bool> SendPrescription(int id,string emaildata,string DoctorName,string url)
        {
            
            try
            {
                var email = new MimeMessage()
                {
                    Sender = MailboxAddress.Parse("atiffahmykhamis@gmail.com"),
                    Subject = "Message from Pharma PRO "


                };
                email.To.Add(MailboxAddress.Parse(emaildata)); ;
                var builder = new BodyBuilder();


                builder.HtmlBody = $@"

<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Digital Prescription</title>
    <link href='https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500&display=swap' rel='stylesheet'>
    <style>
        body {{
            font-family: 'Roboto', sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }}

        .email-content {{
            max-width: 600px;
            margin: 20px auto;
            padding: 20px;
            text-align: center;
        }}

        .email-content p {{
            color: #666;
        }}

        .email-content a {{
            display: inline-block;
            margin-top: 20px;
            padding: 10px 20px;
            color: #fff;
            background-color: #4a90e2;
            text-decoration: none;
            border-radius: 5px;
        }}
    </style>
</head>
<body>
    <div class='email-content'>
        <p>Dear {DoctorName},</p>
        <p>Your digital prescription is ready. Please follow the instructions carefully and feel free to reach out if you have any questions.</p>
        <a href={url+"/"+id} target=""_blank"">View Prescription</a>
    </div>
</body>
</html>
";
               email.Body = builder.ToMessageBody();


                email.From.Add(new MailboxAddress("Pharma PRO", "atiffahmykhamis@gmail.com"));



                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    smtp.Connect("smtp.gmail.com", 587, false);
                    smtp.Authenticate("atiffahmykhamis@gmail.com", "fmnjnhsdhmrugigq");
                    await smtp.SendAsync(email);
                    smtp.Disconnect(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
