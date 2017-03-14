
namespace Common
{
    using System.ServiceModel;
    using System.Threading.Tasks;

    // Define a service contract.
    [ServiceContract]
    public interface ICalculatorService
    {
        [OperationContract]
        Task<string> AddAsync(int b, int a);
        [OperationContract]
        Task<string> SubtractAsync(int a, int b);
    }
}
