using Xunit;
using MyAsyncEnumerable;
using System.Diagnostics;
using static MyEnumTesting.TestedMethods;
using static MyEnumTesting.HelpMethods;

namespace MyEnumTesting
{
    public class MyEnumerableTests
    {
        [Fact]
        public async void Run_FiftyFuncsMyAsyncEnumerable_FiftyResultExpected()
        {
            var funcs = Enumerable.Range(1, 50)
                        .Select(i => (Func<Task<int>>)(async () => await GetResult(i)));
            var myEnum = new MyAsyncEnumerable<int>(funcs);

            var (results, exceptions) = await GetResultFromRunAwaitForeach(myEnum);

            Assert.NotNull(results);
            results!.Sort();
            Assert.Null(exceptions);
            Assert.Equal(actual: results, expected: Enumerable.Range(1, 50).ToList());
        }

        [Fact]
        public async void Run_FiftyFuncsMyAsyncEnumerable_OneExceptionExpected()
        {
            var funcs = Enumerable.Range(2, 49)
                        .Select(i => (Func<Task<int>>)(async () => await GetResult(i)))
                        .ToList();
            funcs.Insert(0, async () => await GetException(1));
            var myEnum = new MyAsyncEnumerable<int>(funcs);
            var artificialResult = Enumerable.Range(2, 49).ToList();

            var (results, exceptions) = await GetResultFromRunAwaitForeach(myEnum);

            Assert.NotNull(results);
            results!.Sort();
            Assert.Equal(actual: results, expected: artificialResult);
            Assert.Contains("Error at number: 1", exceptions!.InnerExceptions.FirstOrDefault()!.Message);
            Assert.True(exceptions!.InnerExceptions.Count == 1);
        }

        [Fact]
        public async void Run_FiftyFuncsMyAsyncEnumerable_TenExceptionsExpected()
        {
            var funcs = Enumerable.Range(1, 40)
                        .Select(i => (Func<Task<int>>)(async () => await GetResult(i)))
                        .ToList();
            funcs.AddRange(Enumerable.Range(41, 10)
                        .Select(i => (Func<Task<int>>)(async () => await GetException(i))));
            var myEnum = new MyAsyncEnumerable<int>(funcs);
            var initialList = Enumerable.Range(1, 50);
            var artificialResult = initialList.Where(i => i < 41).ToList();
            var arificialAggEx = ConstructAggEx(initialList.Where(i => i > 40).ToList());

            var (results, exceptions) = await GetResultFromRunAwaitForeach(myEnum);

            Assert.NotNull(results);
            results!.Sort();
            Assert.Equal(actual: results, expected: artificialResult);
            Assert.True(CompareAggregateEx(exceptions!, arificialAggEx));
        }

        [Fact]
        public async void Run_FiftyFuncsMyAsyncEnumerable_FiftyExceptionsExpected()
        {
            var funcs = Enumerable.Range(1, 50)
                        .Select(i => (Func<Task<int>>)(async () => await GetException(i)));
            var myEnum = new MyAsyncEnumerable<int>(funcs);
            var arificialAggEx = ConstructAggEx(Enumerable.Range(1, 50).ToList());

            var (results, exceptions) = await GetResultFromRunAwaitForeach(myEnum);

            Assert.Null(results);
            Assert.True(CompareAggregateEx(exceptions!, arificialAggEx));
        }

        [Fact]
        public async void Run_OneFuncMyAsyncEnumerable_OneResultExpected()
        {
            var funcs = Enumerable.Range(1, 1)
                        .Select(i => (Func<Task<int>>)(async () => await GetResult(i)));
            var myEnum = new MyAsyncEnumerable<int>(funcs);

            var (results, exceptions) = await GetResultFromRunAwaitForeach(myEnum);

            Assert.Equal(actual: results, expected: Enumerable.Range(1, 1).ToList());
            Assert.Null(exceptions);
        }

        [Fact]
        public async void Run_OneFuncMyAsyncEnumerable_OneExceptionExpected()
        {
            var funcs = Enumerable.Range(1, 1)
                        .Select(i => (Func<Task<int>>)(async () => await GetException(i)));
            var myEnum = new MyAsyncEnumerable<int>(funcs);

            var (results, exceptions) = await GetResultFromRunAwaitForeach(myEnum);

            Assert.Null(results);
            Assert.Contains("Error at number: 1", exceptions!.InnerExceptions.First().Message);
            Assert.True(exceptions.InnerExceptions.Count == 1);
        }

        [Fact]
        public async void Run_EmptyListMyAsyncEnumerable_NoResultExpected()
        {
            var funcs = new List<Func<Task<int>>>();
            var myEnum = new MyAsyncEnumerable<int>(funcs);

            var (results, exceptions) = await GetResultFromRunAwaitForeach(myEnum);

            Assert.Null(results);
            Assert.Null(exceptions);
        }
        [Fact]
        public async void Run_TenFuncMyAsyncEnumerable_RuntimeLessThan6sExpected()
        {
            var funcs = Enumerable.Range(1, 10)
                        .Select(i => (Func<Task<int>>)(async () => await GetResultIn1Sec(i)));
            var myEnum = new MyAsyncEnumerable<int>(funcs, ErrorsHandleMode.IgnoreErrors, 2);
            var stopWatch = Stopwatch.StartNew();

            var (results, exceptions) = await GetResultFromRunAwaitForeach(myEnum);
            var time = stopWatch.Elapsed.Seconds;

            Assert.NotNull(results);
            results!.Sort();
            Assert.Equal(actual: results, expected: Enumerable.Range(1, 10).ToList());
            Assert.Null(exceptions);
            Assert.True(time < 6);
        }
        [Fact]
        public async void Run_TwentyFiveFuncsMyAsyncEnumerable_PredictableLastValueExpected()
        {
            var funcs = Enumerable.Range(1, 25)
                        .Select(i => (Func<Task<int>>)(async () => await GetResult(i)))
                        .ToList();
            funcs.Insert(0, async () => await GetResultIn10Sec(1));
            var myEnum = new MyAsyncEnumerable<int>(funcs);

            var (results, exceptions) = await GetResultFromRunAwaitForeach(myEnum);

            Assert.Equal(actual: results!.Last(), expected: 1010101);
        }
        [Fact]
        public async void Run_FiftyFuncsMyAsyncEnumerable_FortyNineResultsAndNoExceptionsExpected()
        {
            var funcs = Enumerable.Range(2, 49)
                        .Select(i => (Func<Task<int>>)(async () => await GetResult(i)))
                        .ToList();
            funcs.Insert(1, (async () => await GetException(1)));
            var myEnum = new MyAsyncEnumerable<int>(funcs, 4, ErrorsHandleMode.IgnoreErrors);

            var (results, exceptions) = await GetResultFromRunAwaitForeach(myEnum);

            Assert.NotNull(results);
            results!.Sort();
            Assert.Equal(actual: results, expected: Enumerable.Range(2, 49).ToList());
            Assert.Null(exceptions);
        }
        [Fact]
        public async void Run_FiftyFuncsMyAsyncEnumerable_ReturnNoResultsExpected()
        {
            var funcs = Enumerable.Range(1, 50)
                        .Select(i => (Func<Task<int>>)(async () => await GetException(i)));
            var myEnum = new MyAsyncEnumerable<int>(funcs, 4, ErrorsHandleMode.IgnoreErrors);

            var (results, exceptions) = await GetResultFromRunAwaitForeach(myEnum);

            Assert.Null(results);
            Assert.Null(exceptions);
        }
    }
}
