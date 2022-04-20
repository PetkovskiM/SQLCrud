using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public class FullContactModel
    {
        public ContactModel Contact { get; set; }
        public List<EmailModel> Emails { get; set; } = new List<EmailModel>();
        public List<PhoneNumberModel> PhoneNumbers { get; set; } = new List<PhoneNumberModel>();
    }
}
