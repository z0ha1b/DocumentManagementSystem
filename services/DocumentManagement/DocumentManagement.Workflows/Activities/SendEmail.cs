using System.Net;
using System.Net.Mail;
using DocumentManagement.Core.Models;
using DocumentManagement.Core.Repositories.Interfaces;
using DocumentManagement.Core.Services.Interfaces;
using Elsa.Extensions;
using Elsa.Workflows.Core;
using Org.BouncyCastle.Utilities.IO;

namespace DocumentManagement.Workflows.Activities;

public class SendEmail : CodeActivity
{
    /*private IDocumentStore? _documentStore;
    private IFileStorage? _fileStorage;*/

    public IDictionary<string, object> Doc { get; set; } = default!;
    public List<Stream> Output { get; set; } = default!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        // Doc = context.Input;
        //
        // var doc = (Document)Doc["doc"];

        // _documentStore = context.GetRequiredService<IDocumentStore>();
        // _fileStorage = context.GetRequiredService<IFileStorage>();
        //
        // var document = await _documentStore.GetAsync(doc.Id, context.CancellationToken);
        // var fileStream = await _fileStorage.ReadAsync(document!.FileName, context.CancellationToken);
        //
        //
        // Output = fileStream;

        var data = context.GetVariable<List<Stream>>("docs");

        Output = data;

        SendEmailWithAttachment("daniyalarif3@gmail.com,zohaibahmedkhanlodhi@gmail.com,Burkhard.Fels@siecom.de", "Sending Files From Elsa", "Please find file(s) as attachments.", Output, "Attached File");

        await context.CompleteActivityAsync("Done");
    }

    private void SendEmailWithAttachment(string toEmail, string subject, string body, List<Stream> attachmentStreams, string attachmentFileName)
    {
        try
        {
            // Configure the SMTP client with Gmail's SMTP server settings
            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
            {
                smtpClient.Port = 587;

                smtpClient.Credentials = new NetworkCredential("zk70007@gmail.com", "uacblegucvjcuefk");
                smtpClient.EnableSsl = true;

                // Create and configure the email message
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("zk70007@gmail.com");
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    var attachment = new Attachment(@"C:\GitHub\DocumentManagementSystem\apps\DocumentManagement.Web\merged.pdf");
                    mailMessage.Attachments.Add(attachment);


                    // Send the email
                    smtpClient.Send(mailMessage);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}