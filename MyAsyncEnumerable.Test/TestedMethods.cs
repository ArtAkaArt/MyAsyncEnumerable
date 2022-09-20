using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyEnumTesting
{
    public class TestedMethods
    {
        public static async Task<int> GetResultIn1SecCT(int number, CancellationToken ct)
        {
            await Task.Delay(1000, ct);
            return number;
        }
        public static async Task<int> GetResultCT(int number, CancellationToken ct)
        {
            await Task.Delay(100, ct);
            return number;
        }
        public static async Task<int> GetResultIn10SecCT(int number, CancellationToken ct)
        {
            await Task.Delay(10000, ct);
            return 1010101;
        }
        public static async Task<int> GetExceptionCT(int number, CancellationToken ct)
        {
            await Task.Delay(100, ct);
            throw new Exception("Error at number: " + number);
        }

        public static async Task<int> GetResultIn1Sec(int number)
        {
            await Task.Delay(1000);
            return number;
        }
        public static async Task<int> GetResult(int number)
        {
            await Task.Delay(100);
            return number;
        }
        public static async Task<int> GetResultIn10Sec(int number)
        {
            await Task.Delay(10000);
            return 1010101;
        }
        public static async Task<int> GetException(int number)
        {
            await Task.Delay(100);
            throw new Exception("Error at number: " + number);
        }
    }
}
