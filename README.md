# MyAsyncEnumerable

## Description

MyAsyncEnumerable is a library for .NET, written in C#. This library allows to create a MyAsyncEnumerable Class that implements the interface IAsyncEnumerable<T> on the basis of the collection of functions IEnumerable<Func<Task<T>>> or IEnumerable<Func<CancellationToken, Task<T>>>, or to convert such collection into MyAsyncEnumerable Class, and allows to get an asynchronous sequence produced by executing a compiled query.

```csharp
    public async Task<Data> SomeMethod(int i)
    {
      // Do something asynchronously and return result
    }

    var initialCollection = new List<Func<Task<<Data>>>();
    // Or version with CancellationToken
    // var initialCollection = new List<Func<CancellationToken, Task<Tank>>>();
    // var token = new CancellationToken();
    for (int i = 0; i < 100; i++)
    {
      var closure = i;
      initialCollection.Add(async () => SomeMethod(closure));
      // initialCollection.Add((async (token) => await SomeMethod(closure, token)));
    }
    var asyncEn = new MyAsyncEnumerable(initialCollection);
    // Or extension method
    // var asyncEn = initialCollection.ToAsyncEnumerable();
    
    // Usage
    try
    {
      await foreach (var result in asyncEn)
      {
      // Do something with results
      }
    }catch (AggregateException ex)
    {
    // Do something with AggregateException (see into ErrorsHandleMode parameter)
    }
```
## NuGet

[MyAsyncEnumerable at NuGet](https://www.nuget.org/packages/MyAsyncEnumerable)

## Constructors

```csharp
// with token
    public MyAsyncEnumerable(IEnumerable<Func<CancellationToken, Task<T>>> tasks, int capacity = 10, 
                             ErrorsHandleMode mode = ErrorsHandleMode.ReturnAllErrors, int tasksLimit = 4)
```
```csharp
    public MyAsyncEnumerable(IEnumerable<Func<CancellationToken, Task<T>>> tasks, ErrorsHandleMode mode, int tasksLimit = 4)
```
```csharp
    // without token
    public MyAsyncEnumerable(IEnumerable<Func<Task<T>>> tasks, int capacity = 10, 
                             ErrorsHandleMode mode = ErrorsHandleMode.ReturnAllErrors, int tasksLimit = 4)
```
```csharp
    public MyAsyncEnumerable(IEnumerable<Func<Task<T>>> tasks, ErrorsHandleMode mode, int tasksLimit = 4)
```

Parameters:

1. tasks

Original collection of functions.
```csharp
    var asyncEn = new MyAsyncEnumerable(initialCollection);
```
2. capacity

The parameter for transferring the size of the original collection of functions. Is used for some optimization. By default is 10.
```csharp
    var asyncEn = new MyAsyncEnumerable(initialCollection, initialCollection.Count);
```
3. mode

The parameter ErrorsHandleMode of Enum type. Defines actions to be taken if exceptions are thrown while executing a compiled query. All exceptions will be returned as AggregateException.
This parameter has 3 possible mode values:
 * IgnoreErrors. All exceptions will be ignored without canceling iteration.
 * ReturnAllErrors. All exceptions will be collected in AggregateException, that will be thrown after the end of iteration.
 * EndAtFirstError. Iteration will be canceled at first exception, that will be thrown as AggregateException into the calling method. 
```csharp
    var asyncEn = new MyAsyncEnumerable(initialCollection, ErrorsHandleMode. EndAtFirstError);
```
Default value is ErrorsHandleMode.ReturnAllErrors.

4. tasksLimit

The parameter defining how many threads will be used for execution of the collection of functions. By default is 4.
```csharp
    var asyncEn = new MyAsyncEnumerable(initialCollection, initialCollection.Count ErrorsHandleMode. EndAtFirstError, 3);
	  //or
    var asyncEn = new MyAsyncEnumerable(initialCollection, ErrorsHandleMode. EndAtFirstError, 3);
```

## Extension method

Extension method .ToAsyncEnymerable() applies to collections IEnumerable<Func<Task<T>>> or IEnumerable<Func<CancellationToken, Task<T>>>. This method has no parameter tasks; otherwise it takes the same parameters as constructors with the same default values.
```csharp
    var asyncEn = initialCollection.ToAsyncEnumerable();
    var asyncEn = initialCollection.ToAsyncEnumerable(initialCollection.Count);
    var asyncEn = initialCollection.ToAsyncEnumerable(ErrorsHandleMode. EndAtFirstError);
    var asyncEn = initialCollection.ToAsyncEnumerable(initialCollection.Count, ErrorsHandleMode.EndAtFirstError, 3);
    var asyncEn = initialCollection.ToAsyncEnumerable(ErrorsHandleMode. EndAtFirstError, 3);
```
