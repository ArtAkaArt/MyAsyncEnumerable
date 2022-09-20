namespace MyAsyncEnumerable
{
    public class MyAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly IEnumerable<Func<CancellationToken, Task<T>>>? tasksWithToken;
        private readonly IEnumerable<Func<Task<T>>>? tasks;
        private readonly int tasksLimit;
        private readonly ErrorsHandleMode mode;
        private readonly int capacity;

        public MyAsyncEnumerable(IEnumerable<Func<CancellationToken, Task<T>>> tasks, int capacity = 10, 
                                 ErrorsHandleMode mode = ErrorsHandleMode.ReturnAllErrors, int tasksLimit = 4)
        {
            this.capacity = capacity;
            this.tasksLimit = tasksLimit;
            this.mode = mode;
            tasksWithToken = tasks;
        }
        public MyAsyncEnumerable(IEnumerable<Func<Task<T>>> tasks, int capacity = 10, 
                                 ErrorsHandleMode mode = ErrorsHandleMode.ReturnAllErrors, int tasksLimit = 4)
        {
            this.capacity = capacity;
            this.tasksLimit = tasksLimit;
            this.mode = mode;
            this.tasks = tasks;
        }
        public MyAsyncEnumerable(IEnumerable<Func<CancellationToken, Task<T>>> tasks, ErrorsHandleMode mode, int tasksLimit = 4) : 
            this (tasks,10, mode, tasksLimit)
        {
        }
        public MyAsyncEnumerable(IEnumerable<Func<Task<T>>> tasks, ErrorsHandleMode mode, int tasksLimit = 4) :
            this (tasks, 10, mode, tasksLimit)
        {
        }
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            if (tasksWithToken is not null)
                return new MyAsyncEnumerator<T>(tasksWithToken, tasksLimit, mode, capacity);
            return new MyAsyncEnumerator<T>(tasks!, tasksLimit, mode, capacity);
        }
    }
}
