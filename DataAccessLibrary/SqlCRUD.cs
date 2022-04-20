using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLibrary
{
    public class SqlCRUD
    {

        private readonly string _connection;
        SqlDataAccess db = new SqlDataAccess();

        public SqlCRUD(string connection)
        {
            _connection = connection;
        }

        public List<ContactModel> GetAllContacts()
        {
            string sql = "select Id, FirstName, LastName from dbo.Contacts;";

            return db.LoadData<ContactModel, dynamic>(sql, new { }, _connection);

        } 

        public FullContactModel GetContact(int id)
        {
            FullContactModel output = new FullContactModel();

            string sql = "select Id, FirstName, LastName from dbo.Contacts where Id = @Id;";
            output.Contact = db.LoadData<ContactModel,dynamic>(sql, new { Id = id }, _connection).FirstOrDefault();
            
            if(output.Contact == null)
            {
                return null;
            }

            sql = @"select e.*
                    from EmailAddresses e 
                    inner
                    join ContactEmail ce on e.Id = ce.EmailAddressId
                    where ce.ContactId = @Id";
            output.Emails = db.LoadData<EmailModel, dynamic>(sql, new { Id = id }, _connection);

            sql = @"select *
                    from dbo.PhoneNumbers p
                    inner
                    join dbo.ContactPhoneNumbers cp on cp.PhoneNumerId = p.Id
                    where cp.ContactId = @Id";
            output.PhoneNumbers = db.LoadData<PhoneNumberModel, dynamic>(sql, new { Id = id }, _connection);

            return output;
        }

        public void AddContact(FullContactModel model)
        {
            string sql = "insert into dbo.contacts (FirstName, LastName) values (@FirstName, @LastName);";
            db.SaveData(sql, new {model.Contact.FirstName, model.Contact.LastName }, _connection);

            sql = "select Id from dbo.contacts where FirstName = @FirstName and LastName = @LastName;";
            int modelId = db.LoadData<IdModel, dynamic>(sql, new { model.Contact.FirstName, model.Contact.LastName }, _connection).First().Id;

            foreach(var mail in model.Emails)
            {
                if(mail.Id == 0)
                {
                    sql = "insert into dbo.EmailAddresses (EmailAddress) values (@EmailAddress);";
                    db.SaveData(sql, new {mail.EmailAddress}, _connection);
                }

                sql = "select id from dbo.EmailAddresses where EmailAddress = @EmailAddress;";
                int mailId = db.LoadData<IdModel, dynamic> (sql, new {mail.EmailAddress}, _connection).First().Id;

                sql = "insert into dbo.contactemail (contactid, EmailAddressid) values (@ContactId, @EmailAddressId);";
                db.SaveData(sql, new {ContactId = modelId, EmailAddressId = mailId}, _connection);
            }

            foreach(var phone in model.PhoneNumbers)
            {
                if(phone.Id == 0)
                {
                    sql = "insert into dbo.PhoneNumbers (PhoneNumber) values (@PhoneNumber);";
                    db.SaveData(sql, new {phone.PhoneNumber }, _connection);
                }

                sql = "select id from dbo.PhoneNumbers where PhoneNumber = @PhoneNumber;";
                int phoneId = db.LoadData<IdModel, dynamic>(sql, new {phone.PhoneNumber }, _connection).First().Id;

                sql = "insert into dbo.ContactPhoneNumbers (contactid, phoneNumerId) values (@ContactId, @PhoneNumberId);";
                db.SaveData(sql, new { ContactId = modelId, PhoneNumberId = phoneId }, _connection);
            }
        }

        public void UpdateContact(int id, string firstName, string lastName)
        {
            string sql = "update dbo.contacts set firstname = @FirstName, lastname = @LastName where id = @Id";
            if(id > 0)
            {
                db.SaveData(sql, new {id, firstName, lastName}, _connection);
            }
        }

        public void DeletePhoneNumber (int contactId, int phoneNumberId)
        {
            string sql = "select id, contactid, phonenumerid from dbo.ContactPhoneNumbers where contactId = @ContactId and phoneNumerId = @PhoneNumberId";
            var links = db.LoadData<ContactPhoneNumberModel, dynamic>(sql, new { ContactId = contactId, PhoneNumberId = phoneNumberId }, _connection);

            sql = "delete from dbo.ContactPhoneNumbers where contactId = @ContactId and phoneNumerId = @PhoneNumberId";
            db.SaveData (sql, new { ContactId = contactId, PhoneNumberId = phoneNumberId }, _connection);

            if(links.Count == 1)
            {
                sql = "delete from dbo.phonenumbers where id = @Id";
                db.SaveData(sql, new {Id = phoneNumberId}, _connection);
            }
        }
    }
}
