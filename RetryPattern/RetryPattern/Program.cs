var retryHelper = new RetryHelper(
            maxRetries: 5,
            initialDelay: 500,
            maxDelay: 5000
        );

retryHelper.Execute(PerformOperation);

static void PerformOperation()
{
    throw new Exception("Simulated operation failure");
}

public class RetryHelper(int maxRetries, int initialDelay, int maxDelay)
{
    public void Execute(Action action)
    {
        int attempts = 0;
        int delay = initialDelay;

        while (attempts < maxRetries)
        {
            try
            {
                attempts++;
                Console.WriteLine($"Attempt # {attempts}");
                action();

                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                if (attempts >= maxRetries)
                {
                    Console.WriteLine("Retry helper stopped.");
                    throw;
                }

                Console.WriteLine($"Wait for {delay} ms");
                Thread.Sleep(delay);


                delay = Math.Min(delay * 2, maxDelay);
            }
        }
    }
}

