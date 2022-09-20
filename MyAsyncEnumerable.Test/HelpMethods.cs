namespace MyEnumTesting
{
    public class HelpMethods
    {
        public static bool CompareAggregateEx(AggregateException aggEx, AggregateException imitation)
        {
            var isExsEqual = true;
            if (imitation.InnerExceptions.Count != aggEx.InnerExceptions.Count) return false;
            foreach (var inEx1 in imitation.InnerExceptions)
            {
                var inEx2 = aggEx.InnerExceptions.FirstOrDefault(i => i.Message.Contains(inEx1.Message));
                isExsEqual = isExsEqual && inEx2 != null;
            }
            return isExsEqual;
        }

        public static AggregateException ConstructAggEx(List<int> numbers)
        {
            var list = new List<Exception>();
            foreach (var number in numbers)
            {
                list.Add(new Exception("Error at number: " + number));
            }
            return new AggregateException(list);
        }

        public static async Task<(List<int>? results, AggregateException? exceptions)> GetResultFromRunAwaitForeach(IAsyncEnumerable<int> enumerable)
        {
            var results = new List<int>();
            var ex = new AggregateException();
            try
            {
                await foreach (var res in enumerable)
                {
                    results.Add(res);
                }
            }
            catch (AggregateException ex2)
            {
                ex = ex2;
            }
            return (results.Count == 0 ? null : results,
                ex.InnerExceptions.Count == 0 ? null : ex);
        }
    }
}
