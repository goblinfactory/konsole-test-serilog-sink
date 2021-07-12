using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;
using static System.ConsoleColor;


namespace Konsole.Sink
{
  public class KonsoleSink : ILogEventSink
  {
    IFormatProvider _formatProvider;
    private readonly IConsole _window;
    private readonly IConsole _errorWindow;
    private readonly bool _logWarningsToErrorWindow;

    public KonsoleSink(IFormatProvider formatProvider, IConsole window, IConsole errorWindow, bool logWarningsToErrorWindow)
    {
      _formatProvider = formatProvider;
      _window = window;
      _errorWindow = errorWindow;
      _logWarningsToErrorWindow = logWarningsToErrorWindow;
    }

    public void Emit(LogEvent logEvent)
    {
      var text = logEvent.RenderMessage(_formatProvider);
      var eWin = _errorWindow ?? _window;
      switch (logEvent.Level)
      {
        // Anything and everything you might want to know about a running block of code.
        case LogEventLevel.Verbose:
        // Internal system events that aren't necessarily observable from the outside.
        case LogEventLevel.Debug:
          _window.WriteLine(text);
          break;

        // The lifeblood of operational intelligence - things happen.
        case LogEventLevel.Information:
          _window.WriteLine(Yellow, text);
          break;

        // Service is degraded or endangered.
        case LogEventLevel.Warning:
          if (_logWarningsToErrorWindow)
            eWin.WriteLine(Cyan, text);
          else
            _window.WriteLine(Cyan, text);
          break;

        // Functionality is unavailable, invariants are broken or data is lost.
        case LogEventLevel.Error:
          eWin.WriteLine(Red, text);
          eWin.WriteLine("    ---");
          eWin.WriteLine($"    {logEvent?.Exception?.Message}");
          eWin.WriteLine("    ---");
          break;

        // If you have a pager, it goes off when one of these occurs.
        case LogEventLevel.Fatal:
          eWin.WriteLine(Colors.RedOnWhite, text);
          break;

      }

    }
  }

  public static class KonsoleSinkExtensions
  {
    public static LoggerConfiguration KonsoleWindow(
              this LoggerSinkConfiguration loggerConfiguration,
              IConsole window,
              IConsole errorWindow = null,
              bool logWarningsToErrorWindow = true,
              IFormatProvider fmtProvider = null
              )
    {
      return loggerConfiguration.Sink(new KonsoleSink(fmtProvider, window, errorWindow, logWarningsToErrorWindow));
    }
  }
}
