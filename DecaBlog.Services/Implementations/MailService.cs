
using System;
using System.Threading.Tasks;
using DecaBlog.Models.DTO;
using DecaBlog.Services.Interfaces;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

namespace DecaBlog.Services.Implementations
{
    public class MailService : IMailService
    {
        private IMailjetClient _client;
        public MailService(IMailjetClient client)
        {
            _client = client;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contact"></param>
        /// the following should be used as place holders in the html document 
        /// for email message body = {Message}
        /// for Link = {Link}
        /// <returns></returns>
        public async Task SendMailAsync(EmailMessage contact)
        {
            try
            {
                foreach (var mail in contact.ToEmail)
                {
                    string email = mail;
                    MailjetRequest request = new MailjetRequest { Resource = SendV31.Resource }
                    .Property(Send.Messages, new JArray {
                        new JObject
                        {
                            {
                                "From",new JObject
                                {
                                    {"Email","decagonblog@gmail.com"},
                                    {"Name", "Deca-Blog"}
                                }
                            },
                            {
                                "To", new JArray
                                {
                                    new JObject
                                    {
                                        {"Email", mail },
                                    }
                                }
                            },
                            { "Subject",contact.Subject },
                            { "TextPart",contact.PlainTextMessage },
                            { "HtmlPart",  $@"{contact.Template}" },
                            { "CustomId","AppGettingStartedTest" }
                        }
                    });
                    MailjetResponse response = await _client.PostAsync(request);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
