﻿using System;
using System.Collections.Generic;
using System.Linq;
using BankAccount.DbModel.ItemDb;
using BankAccount.ReadModel;
using EventStore;

namespace BankAccount.QueryStackDal
{
    public sealed class QueryStackRepository : IQueryStackRepository
    {
        private readonly IStoreEvents _eventStore;

        public QueryStackRepository(IStoreEvents eventStore)
        {
            _eventStore = eventStore;
        }

        public CustomerReadModel GetCustomerById(Guid aggregateId)
        {
            var obj = new Domain.CustomerDomainModel();

            IEnumerable<Commit> commits;

            var latestSnapshot = this._eventStore.Advanced.GetSnapshot(aggregateId, int.MaxValue);
            if (latestSnapshot?.Payload != null)
            {
                obj = (Domain.CustomerDomainModel)Convert.ChangeType(latestSnapshot.Payload, latestSnapshot.Payload.GetType());
                commits = this._eventStore.Advanced.GetFrom(aggregateId, latestSnapshot.StreamRevision + 1, int.MaxValue).ToList();
            }
            else
            {
                commits = this._eventStore.Advanced.GetFrom(aggregateId, 0, int.MaxValue).ToList();
            }

            foreach (var c in commits)
            {
                obj.LoadsFromHistory(c.Events);
            }

            return new CustomerReadModel
            {
                AggregateId = aggregateId,
                Version = obj.Version,
                LastName = obj.Person.LastName,
                FirstName = obj.Person.FirstName,
                IdCard = obj.Person.IdCard,
                IdNumber = obj.Person.IdNumber,
                Dob = obj.Person.Dob,
                Phone = obj.Contact.PhoneNumber,
                Email = obj.Contact.Email,
                Street = obj.Address.Street,
                Hausnumber = obj.Address.Hausnumber,
                Zip = obj.Address.Zip,
                State = obj.Address.State,
                City = obj.Address.City
            };
        }

        public IEnumerable<AccountReadModel> GetAccountsByCustomerId(Guid customerId)
        {
            using (var ctx = new BankAccountDbContext())
            {
                return ctx.AccountSet.Where(a => a.CustomerAggregateId == customerId).Select(e => new AccountReadModel
                {
                    Currency = e.Currency,
                    Id = e.AggregateId
                }).ToList();
            }
        }

        public AccountReadModel GetAccountById(Guid aggregateId)
        {
            using (var ctx = new BankAccountDbContext())
            {
                var model = ctx.AccountSet.SingleOrDefault(a => a.AggregateId == aggregateId);
                if (model == null)
                {
                    throw new ArgumentNullException("account");
                }

                return new AccountReadModel
                {
                    Currency = model.Currency,
                    Id = model.AggregateId,
                    CustomerId = model.CustomerAggregateId
                };
            }
        }

        public IEnumerable<CustomerReadModel> GetCustomers()
        {
            using (var ctx = new BankAccountDbContext())
            {
                return ctx.CustomerSet.Select(e => new CustomerReadModel
                {
                    AggregateId         = e.AggregateId,
                    Version             = e.Version,
                    FirstName           = e.FirstName,
                    LastName            = e.LastName
                }).ToList();
            }
        }
    }
}
