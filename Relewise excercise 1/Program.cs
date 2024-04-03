using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Relewise_excercise_1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var excercise1 = new Excercise1();
            var job = new Excercise2();
            var excercise3 = new Excercise3();
            var arguments = new JobArguments(
                Guid.NewGuid(),
                null,
                null);

            // Create delegates
            Func<string, Task> info = async message => Console.WriteLine(message);
            Func<string, Task> warn = async message => Console.WriteLine($"Warning: {message}");

            // Call execute methods for each exercise
            string result = await excercise1.Execute(arguments, info, warn, default);
            string result2 = await job.Execute(arguments, info, warn, default);
            string result3 = await excercise3.Execute(arguments, info, warn, default);
        }
    }
}
