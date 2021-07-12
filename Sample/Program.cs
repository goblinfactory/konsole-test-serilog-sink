using System;
using System.Threading;
using Konsole;
using Konsole.Sink;
using Serilog;

namespace Sample
{
  public class Program
  {
    public static void Main()
    {
      var window = new Window();
      var (main, right) = window.SplitLeftRight(BorderCollapse.None);
      var (logs, errors) = right.SplitTopBottom("info", "errors and warnings");

      Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Verbose()
          .WriteTo.KonsoleWindow(logs, errors)
          .CreateLogger();

      try
      {
        main.WriteLine("Sample starting...");
        Log.Debug("[DBG] sample is running");
        for (int x = 0; x < 10; x++)
        {
          Log.Debug($"make scroll {x}");
          Console.ReadKey(true);
        }
        Log.Information("[INF] Hello {Name} from thread {ThreadId}", Environment.GetEnvironmentVariable("USERNAME"), Thread.CurrentThread.ManagedThreadId);
        Log.Warning("[WRN] No coins remain at position {@Position}", new { Lat = 25, Long = 134 });

        Fail();
      }
      catch (Exception e)
      {
        Log.Error(e, "[ERR] Something went wrong");
      }
      finally
      {
        main.WriteLine("shutting down.");
      }

      Log.CloseAndFlush();
    }

    static void Fail()
    {
      throw new DivideByZeroException();
    }
  }
}
