using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rnwood.SmtpServer.Tests
{
    public static class TaskExtensions
    {
        public static async Task WithTimeout(this Task task, string descriptionOfTask)
        {
            await WithTimeout(task, 10, descriptionOfTask).ConfigureAwait(false);
        }

        public static async Task WithTimeout(this Task task, int seconds, string descriptionOfTask)
        {
            Task completedTask = await Task.WhenAny(task, Task.Delay(seconds * 1000)).ConfigureAwait(false);

            if (completedTask != task)
            {
                throw new TimeoutException("Timeout waiting for " + descriptionOfTask);
            }
        }
    }
}