
var circuitBreaker = new CircuitBreaker(
            failureThreshold: 3,
            openTimeout: TimeSpan.FromSeconds(3)
        );

for (int i = 0; i < 10; i++)
{
    try
    {
        circuitBreaker.Execute(PerformOperation);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    Thread.Sleep(1000);
}

static void PerformOperation()
{
    throw new Exception("Simulated operation failure");
}


public class CircuitBreaker(int failureThreshold, TimeSpan openTimeout)
{
    private readonly object _lockObj = new();

    private int _failureCount;
    private DateTime _lastFailureTime;
    private CircuitBreakerState _state = CircuitBreakerState.Closed;

    public void Execute(Action operation)
    {
        if (_state == CircuitBreakerState.Open && HalfOpenPermitted())
        {
            lock (_lockObj)
                _state = CircuitBreakerState.HalfOpen;
            Console.WriteLine("Circuit breaker is half open.");
        }
        else if (_state == CircuitBreakerState.Open && !HalfOpenPermitted())
        {
            Console.WriteLine("Circuit breaker is open.");
            return;
        }

        if (_state == CircuitBreakerState.HalfOpen)
        {
            TryExecute(operation);
            return;
        }

        TryExecute(operation);
    }

    private bool HalfOpenPermitted() =>
        DateTime.UtcNow - _lastFailureTime > openTimeout;

    private void TryExecute(Action operation)
    {
        try
        {
            operation();
            Reset();
        }
        catch (Exception)
        {
            HandleFailure();
            throw;
        }
    }

    private void Reset()
    {
        lock (_lockObj)
        {
            _failureCount = 0;
            _state = CircuitBreakerState.Closed;
        }
    }

    private void HandleFailure()
    {
        lock (_lockObj)
        {
            _failureCount++;
            if (_failureCount < failureThreshold)
                return;

            _state = CircuitBreakerState.Open;
            _lastFailureTime = DateTime.UtcNow;
        }
    }
}

public enum CircuitBreakerState
{
    Closed,
    Open,
    HalfOpen
}

