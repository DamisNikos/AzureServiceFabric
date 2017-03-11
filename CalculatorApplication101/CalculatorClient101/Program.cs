using CalculatorService101;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorClient101
{
    class Program
    {
        static void Main(string[] args)
        {
            ICalculatorService calculatorClient = ServiceProxy.Create<ICalculatorService>
                (new Uri("fabric:/CalculatorApplication101/CalculatorService101"));
            var result = calculatorClient.AddAsync(1, 2).Result;
            Console.WriteLine(result);
            Console.ReadLine();
            result = calculatorClient.SubtractAsync(5, 3).Result;
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}
