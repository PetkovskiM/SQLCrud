
using DataAccessLibrary;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace SQLCrud
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SqlCRUD sql = new SqlCRUD(GetConnectionString());

            //GetAllContacts(sql); 
            //GetContact(sql, 1);
            //AddContact(sql);
            //UpdateContact(sql);
            DeleteContactNumber(sql);
            Console.WriteLine("Done !");
        }

        private static void GetAllContacts(SqlCRUD sql)
        {
           var rows = sql.GetAllContacts();

            foreach (var row in rows)
            {
                Console.WriteLine($"{row.Id}: {row.FirstName} {row.LastName}");
            }
        }

        private static void GetContact(SqlCRUD sql, int id)
        {
            var contact = sql.GetContact(id);

            Console.WriteLine($"{contact.Contact.Id}: {contact.Contact.FirstName} {contact.Contact.LastName}");
            Console.WriteLine("phone numbers: ");
            foreach(var phone in contact.PhoneNumbers)
            {
                Console.WriteLine(phone.PhoneNumber);
            }
            Console.WriteLine("emails: ");
            foreach (var mail in contact.Emails)
            {
                Console.WriteLine(mail.EmailAddress);
            }
        }

        private static void AddContact(SqlCRUD sql)
        {
            FullContactModel user = new FullContactModel
            {
                Contact = new ContactModel
                {
                    FirstName = "",
                    LastName = ""
                }
            };
            user.Emails.Add(new EmailModel { EmailAddress = "" });
            user.Emails.Add(new EmailModel { Id = 1, EmailAddress = "" });
            user.PhoneNumbers.Add(new PhoneNumberModel { PhoneNumber = "" });

            sql.AddContact(user);
        }

        private static void UpdateContact(SqlCRUD sql)
        {
            int id = 1;
            string firstName = "MILEEE";
            string lastName = "Petkoooovski";
            sql.UpdateContact(id, firstName, lastName);
        }

        private static void DeleteContactNumber(SqlCRUD sql)
        {
            sql.DeletePhoneNumber(1011, 2004);
        }

        private static string GetConnectionString(string connectionStringName = "Default")
        {
            string output = "";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build();

            output = config.GetConnectionString(connectionStringName);
    
            return output;
        }
    }
}
