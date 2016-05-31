using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankAccount.Infrastructure;
using BankAccount.Infrastructure.Buses;
using EventStore;
using EventStore.Dispatcher;
using Newtonsoft.Json;
using msmq = System.Messaging;

namespace BankAccount.EventStore
{
    public class CommitsDispatcher : IDispatchCommits
    {
        #region Fields

        #endregion

        #region C-Tor

        #endregion

        #region IDispatchCommits impementation

        public void Dispatch(Commit commit)
        {
            try
            {
                Task.Run(() =>
                {
                    foreach (var ev in commit.Events.Select(@event => Converter.ChangeTo(@event.Body, @event.Body.GetType())))
                    {
                        try
                        {
                            using (var queue = new msmq.MessageQueue(".\\private$\\bankaccount-tx"))
                            {
                                var message = new msmq.Message();
                                var jsonBody = JsonConvert.SerializeObject(ev);
                                message.BodyStream = new MemoryStream(Encoding.Default.GetBytes(jsonBody));
                                var tx = new msmq.MessageQueueTransaction();
                                tx.Begin();
                                queue.Send(message, tx);
                                tx.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debugger.Break();
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Dispose()
        {
            
        }

        #endregion
    }
}
