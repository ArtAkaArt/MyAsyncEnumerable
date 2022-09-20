namespace MyAsyncEnumerable
{
    public static class TaskListExtention
    {
        public static async IAsyncEnumerable<T> ToAsyncEumerable<T>(this IEnumerable<Func<CancellationToken, Task<T>>> tasks, int capacity = 10, ErrorsHandleMode mode = ErrorsHandleMode.ReturnAllErrors, int tasksLimit = 4)
        {
            var iterator = new MyAsyncEnumerator<T>(tasks, tasksLimit, mode, capacity);

            while (await iterator.MoveNextAsync())
            {
                yield return iterator.Current!;
            }
        }
        public static async IAsyncEnumerable<T> ToAsyncEumerable<T>(this IEnumerable<Func<CancellationToken, Task<T>>> tasks, ErrorsHandleMode mode, int tasksLimit = 4)
        {
            var iterator = new MyAsyncEnumerator<T>(tasks, tasksLimit, mode, 10);

            while (await iterator.MoveNextAsync())
            {
                yield return iterator.Current!;
            }
        }
        public static async IAsyncEnumerable<T> ToAsyncEumerable<T>(this IEnumerable<Func<Task<T>>> tasks, int capacity = 10, ErrorsHandleMode mode = ErrorsHandleMode.ReturnAllErrors, int tasksLimit = 4)
        {
            var iterator = new MyAsyncEnumerator<T>(tasks, tasksLimit, mode, capacity);

            while (await iterator.MoveNextAsync())
            {
                yield return iterator.Current!;
            }
        }
        public static async IAsyncEnumerable<T> ToAsyncEumerable<T>(this IEnumerable<Func<Task<T>>> tasks, ErrorsHandleMode mode, int tasksLimit = 4)
        {
            var iterator = new MyAsyncEnumerator<T>(tasks, tasksLimit, mode, 10);

            while (await iterator.MoveNextAsync())
            {
                yield return iterator.Current!;
            }
        }
    }
}

