using System;
namespace Numero.Helpers
{
    public class AppSettings
    {
        //Jwt Properties
        public string Site { get; set; }
        public string Audience { get; set; }
        public string ExpireTime { get; set; }
        public string Secret { get; set; }

        //SendGrid Mailing Properties
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
        public string TemplateId { get; set; }


    }
}



