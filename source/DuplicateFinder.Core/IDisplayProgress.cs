using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.WindowsAPICodePack.Taskbar;

namespace DuplicateFinder.Core
{
  public interface IDisplayProgress
  {
    void Intermediate();
    void Percent(int currentValue, int maximumValue);
    void Stop();
  }

  class TaskbarProgress : IDisplayProgress
  {
    readonly Action _intermediate = () => { };
    readonly Action<int, int> _percent = (current, max) => { };
    readonly Action _stop = () => { };

    public TaskbarProgress()
    {
      if (!TaskbarManager.IsPlatformSupported)
      {
        return;
      }

      var window = FindConsoleWindow();

      if (window == IntPtr.Zero)
      {
        return;
      }

      _intermediate = () => TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate, window);

      _percent = (current, max) =>
      {
        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal, window);
        TaskbarManager.Instance.SetProgressValue(current, max, window);
      };

      _stop = () => TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress, window);
    }

    public void Intermediate()
    {
      _intermediate();
    }

    public void Percent(int currentValue, int maximumValue)
    {
      _percent(currentValue, maximumValue);
    }

    public void Stop()
    {
      _stop();
    }

    [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
    static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

    static IntPtr FindConsoleWindow()
    {
      try
      {
        var window = Process.GetCurrentProcess().MainWindowHandle;
        if (window == IntPtr.Zero)
        {
          var title = Console.Title;
          var guid = Guid.NewGuid().ToString();
          Console.Title = guid;

          window = FindWindowByCaption(IntPtr.Zero, guid);

          Console.Title = title;
        }

        return window;
      }
      catch (Exception)
      {
        return IntPtr.Zero;
      }
    }
  }
}
