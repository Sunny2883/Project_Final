using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using Promact.CustomerSuccess.Platform.Services.Dtos;
using Promact.CustomerSuccess.Platform.Entities;
using Volo.Abp.Domain.Repositories;

namespace Promact.CustomerSuccess.Platform.Services.Emailing
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<Stakeholder, Guid> _stakeholderRepository;
        public EmailService(IConfiguration configuration, IRepository<Stakeholder, Guid> stakeholderRepository)
        {
            _configuration = configuration;
            _stakeholderRepository = stakeholderRepository;
        }

        public async void SendEmailToStakeHolder(EmailToStakeHolderDto request)
        {
            try
            {
                var stakeholders = await _stakeholderRepository.GetListAsync(s => s.ProjectId == request.ProjectId);

                foreach (var stakeholder in stakeholders)
                {
                    var emailDto = new EmailDto
                    {
                        To = stakeholder.Contact,
                        Subject = request.Subject,
                        Body = Template.GetEmailTemplate(stakeholder.Name)
                    };

                    // Send email asynchronously
                    Task.Run(() => SendEmail(emailDto));
                }
            }
            catch (Exception ex) { }
        }

        public void SendEmail(EmailDto request)
        {
            Console.WriteLine("=================================================");
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var port = int.Parse(_configuration["EmailSettings:Port"]);
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];
            var fromAddress = _configuration["EmailSettings:FromAddress"];
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(fromAddress));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text =request.Body
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(smtpServer, port, SecureSocketOptions.StartTls);
                    client.Authenticate(username, password);
                    client.Send(email);
                    client.Disconnect(true);
                }

            }
            catch (Exception ex) { }
           
        }
    }
}
