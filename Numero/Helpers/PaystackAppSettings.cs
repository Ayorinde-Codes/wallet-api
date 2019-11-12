using System;
namespace Numero.Helpers
{
    public class PaystackAppSettings
    {
        //PayStack Details
        public string LiveSecret { get; set; }
        public string TestSecret { get; set; }
        public string CallBackURL { get; set; }
    }
}
