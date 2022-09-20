using System.Diagnostics;

namespace MyAsyncEnumerable
{
    internal class MyAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private bool isStarted = true;
        private readonly ErrorsHandleMode mode;
        private readonly IEnumerable<Func<CancellationToken, Task<T>>>? tasksWithToken;
        private readonly IEnumerable<Func<Task<T>>>? tasks;
        private readonly CancellationTokenSource cts = new();
        private readonly SemaphoreSlim semaphore;
        private readonly List<Task<T>> taskCollection;
        private readonly List<Exception> exceptions = new();
        public T? Current { get; private set; }

        T IAsyncEnumerator<T>.Current => Current!;
        public MyAsyncEnumerator(IEnumerable<Func<CancellationToken, Task<T>>> tasks, int tasksLimit, ErrorsHandleMode mode, int capacity)
        {
            tasksWithToken = tasks;
            taskCollection = new(capacity);
            semaphore = new SemaphoreSlim(tasksLimit);
            this.mode = mode;
        }
        public MyAsyncEnumerator(IEnumerable<Func<Task<T>>> tasks, int tasksLimit, ErrorsHandleMode mode, int capacity)
        {
            this.tasks = tasks;
            taskCollection = new(capacity);
            semaphore = new SemaphoreSlim(tasksLimit);
            this.mode = mode;
        }
        public ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            cts.Cancel();
            return ValueTask.CompletedTask;
        }
        public async ValueTask<bool> MoveNextAsync()
        {
            if (isStarted && tasksWithToken is not null)
                RunAllTasksWithToken();
            if (isStarted && tasks is not null)
                RunAllTasks();

            isStarted = false;
            while (taskCollection.Count != 0)
            {
                var endedTask = await Task.WhenAny(taskCollection);
                taskCollection.Remove(endedTask);
                if (endedTask.Status == TaskStatus.RanToCompletion)
                {
                    Current = endedTask.Result;
                    return true;
                }
                else if (endedTask.Status == TaskStatus.Faulted)
                {
                    switch (mode)
                    {
                        case ErrorsHandleMode.IgnoreErrors:
                            break;
                        case ErrorsHandleMode.ReturnAllErrors:
                            exceptions.Add(endedTask.Exception!);
                            break;
                        case ErrorsHandleMode.EndAtFirstError:
                            throw endedTask.Exception!;
                    }
                }
                else if (endedTask.Status == TaskStatus.Canceled)
                {
                    cts.Cancel();
                    throw new AggregateException(new TaskCanceledException("Cancelation token requested"));
                }
                else Debug.Assert(false, "Awaited task has unexpected status");
            }
            if (exceptions.Count > 0) throw new AggregateException(exceptions);

            return false;
        }
        private void RunAllTasksWithToken()
        {
            var token = cts.Token;
            foreach (var func in tasksWithToken!)
            {
                var task = Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        return await func.Invoke(token);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, token);
                taskCollection.Add(task);
            }
        }
        private void RunAllTasks()
        {
            var token = cts.Token;
            foreach (var func in tasks!)
            {
                var task = Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        return await func.Invoke();
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, token);
                taskCollection.Add(task);
            }
        }
    }
}