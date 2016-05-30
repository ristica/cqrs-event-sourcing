using System;
using BankAccount.CommandStackDal.Abstraction;

namespace BankAccount.EventHandlers.Base
{
    public abstract class BaseCustomerEventHandler
    {
        protected readonly ICommandStackDatabase Database;

        protected BaseCustomerEventHandler(ICommandStackDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException($"Database was not initialized");
            }

            this.Database = database;
        }
    }
}
