using System;
using System.Timers;

namespace Phoenix
{
  //// Creates a timer that accepts a `timerCalc` function to perform
  //// calculated timeout retries, such as exponential backoff.
  ////
  //// ## Examples
  ////
  ////    let reconnectTimer = new Timer(() => this.connect(), function(tries){
  ////      return [1000, 5000, 10000][tries - 1] || 10000
  ////    })
  ////    reconnectTimer.setTimeout() // fires after 1000
  ////    reconnectTimer.setTimeout() // fires after 5000
  ////    reconnectTimer.reset()
  ////    reconnectTimer.setTimeout() // fires after 1000
  ////

  public class RetryTimer
  {
    private Action _callback;
    private Func<int, int> _timerCalc;
    private Timer _timer;
    private int _numTries;

    //  constructor(callback, timerCalc){
    //    this.callback  = callback
    //    this.timerCalc = timerCalc
    //    this.timer     = null
    //    this.tries     = 0
    //  }
    public RetryTimer(Action callback, Func<int, int> timerCalc)
    {
      _callback = callback;
      _timerCalc = timerCalc;
      _timer = new Timer()
      {
        AutoReset = false
      };
      _timer.Elapsed += OnElapsed;

      _numTries = 0;
    }

    //  reset(){
    //    this.tries = 0
    //    clearTimeout(this.timer)
    //  }
    public void Reset()
    {
      _numTries = 0;
      _timer.Stop();
    }

    //  // Cancels any previous setTimeout and schedules callback
    //  setTimeout(){
    //    clearTimeout(this.timer)

    //    this.timer = setTimeout(() => {
    //      this.tries = this.tries + 1
    //      this.callback()
    //    }, this.timerCalc(this.tries + 1))
    //  }
    public void SetTimeout()
    {
      _timer.Stop();
      var wait = _timerCalc(_numTries + 1);
      _timer.Interval = wait;
      _timer.Start();
    }

    private void OnElapsed(object sender, ElapsedEventArgs e)
    {
      _numTries += 1;
      _callback();
    }
  }
}