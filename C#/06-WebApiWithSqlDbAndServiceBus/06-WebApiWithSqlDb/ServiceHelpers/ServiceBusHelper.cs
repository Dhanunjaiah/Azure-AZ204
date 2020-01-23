using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace _06_WebApiWithSqlDb.ServiceHelpers
{
    public class ServiceBusHelper
    {
        private static IQueueClient queueClient;

        public ServiceBusHelper(string connectionSting, string queueName)
        {

            queueClient = new QueueClient(connectionSting, queueName);
        }

        public async Task SendMessageAsync(string messageText)
        {
            Message message = new Message(Encoding.UTF8.GetBytes(messageText));
            await queueClient.SendAsync(message);
            Console.WriteLine("Message sent successfully");
        }

        public void RegisterEventHandler(Func<Message,CancellationToken, Task> messageProcessor)
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };
            queueClient.RegisterMessageHandler(messageProcessor, messageHandlerOptions);
        }

        public static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            
            var messageBody = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine("Message received:" + messageBody );
            // Complete the message so that it is not received again.
            // This can be done only if the queue Client is created in ReceiveMode.PeekLock mode (which is the default).
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
