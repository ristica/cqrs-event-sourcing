using System;
using BankAccount.Events;
using BankAccount.Infrastructure.Domain;
using BankAccount.Infrastructure.EventHandling;
using BankAccount.ValueTypes;

namespace BankAccount.Domain
{
    public class CustomerDomainModel : AggregateRoot,
        IHandle<CustomerCreatedEvent>,
        IHandle<CustomerDeletedEvent>,
        IHandle<PersonChangedEvent>,
        IHandle<ContactChangedEvent>,
        IHandle<AddressChangedEvent>
    {
        #region Properties

        public Person Person { get; set; }
        public Contact Contact { get; set; }
        public Address Address { get; set; }
        public State State { get; set; }

        #endregion

        #region Public methods called by command handlers

        public void CreateNewCustomer(
            Guid id,
            string firstName,
            string lastName,
            string idCard,
            string idNumber,
            DateTime dob,
            string email,
            string phone,
            string street,
            string zip,
            string hausnumber,
            string city,
            string state)
        {
            ApplyChange(
                new CustomerCreatedEvent
                {
                    AggregateId     = id,
                    Version         = this.Version,
                    Person          = new Person(firstName, lastName, dob, idCard, idNumber),
                    Contact         = new Contact(email, phone),
                    Address         = new Address(street, zip, hausnumber, city, state)
        });
        }

        public void ChangePerson(
            string firstName,
            string lastName,
            string idCard,
            string idNumber)
        {
            ApplyChange(
                new PersonChangedEvent
                {
                    AggregateId     = this.Id,
                    Version         = this.Version,
                    FirstName       = firstName,
                    LastName        = lastName,
                    IdCard          = idCard,
                    IdNumber        = idNumber,
                    State           = State.Open
                });
        }

        public void ChangeContact(
            string email,
            string phone)
        {
            ApplyChange(
                new ContactChangedEvent
                {
                    AggregateId     = this.Id,
                    Version         = this.Version,
                    Email           = email,
                    Phone           = phone
                });
        }

        public void ChangeAddress(
            string street,
            string hausnumber,
            string zip,
            string city,
            string state)
        {
            ApplyChange(
                new AddressChangedEvent
                {
                    AggregateId     = this.Id,
                    Version         = this.Version,
                    Street          = street,
                    Hausnumber      = hausnumber,
                    Zip             = zip,
                    City            = city,
                    State           = state
                });
        }

        public void DeleteBankAccount()
        {
            ApplyChange(
                new CustomerDeletedEvent
                {
                    AggregateId     = this.Id,
                    Version         = this.Version,
                    State           = State.Closed
                });
        }

        #endregion

        #region IHandle implementation of events

        public void Handle(CustomerCreatedEvent e)
        {
            this.Id         = e.AggregateId;
            this.Version    = e.Version;
            this.Person     = e.Person;
            this.Contact    = e.Contact;
            this.Address    = e.Address;
            this.State      = e.State;
        }

        public void Handle(PersonChangedEvent e)
        {
            this.Version            = e.Version;
            this.Person.FirstName   = e.FirstName;
            this.Person.LastName    = e.LastName;
            this.Person.IdCard      = e.IdCard;
            this.Person.IdNumber    = e.IdNumber;
        }

        public void Handle(ContactChangedEvent e)
        {
            this.Version = e.Version;
            this.Contact = new Contact(e.Email, e.Phone);
        }

        public void Handle(AddressChangedEvent e)
        {
            this.Version    = e.Version;
            this.Address    = new Address(e.Street, e.Zip, e.Hausnumber, e.City, e.State);
        }

        public void Handle(CustomerDeletedEvent e)
        {
            this.Version    = e.Version;
            this.State      = e.State;
        }

        #endregion

        #region Factory

        public static class Factory
        {
            public static CustomerDomainModel CreateNewInstance(
                Guid id,
                int version,
                string firstName,
                string lastName,
                string idCard,
                string idNumber,
                DateTime dob,
                string email,
                string phone,
                string street,
                string zip,
                string hausnumber,
                string city,
                string state)
            {
                var @event = new CustomerCreatedEvent
                {
                    AggregateId     = id,
                    Version         = version,
                    Person          = new Person(firstName, lastName, dob, idCard, idNumber),
                    Contact         = new Contact(email, phone),
                    Address         = new Address(street, zip, hausnumber, city, state),
                    State           = State.Open
                };
                var ba = new CustomerDomainModel();
                ba.ApplyChange(@event);
                return ba;
            }
        }

        #endregion
    }
}
