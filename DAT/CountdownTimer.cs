using System;
using System.Diagnostics;
using System.Timers;

namespace DAT
{
    public class CountdownTimer : IDisposable
    {
        public Stopwatch _stpWatch = new Stopwatch();

        public EventHandler TimeChanged;
        public EventHandler CountDownFinished;

        public bool IsRunnign => timer.Enabled;

        private Timer timer = new Timer();
        public int StepMs
        {
            get => (int)timer.Interval;
            set => timer.Interval = value;
        }


        private TimeSpan _max = TimeSpan.FromMilliseconds(30000);

        public TimeSpan TimeLeft => 
            (_max.TotalMilliseconds - _stpWatch.ElapsedMilliseconds) > 0 
            ? TimeSpan.FromMilliseconds(_max.TotalMilliseconds - _stpWatch.ElapsedMilliseconds) 
            : TimeSpan.FromMilliseconds(0);

        private bool _mustStop => (_max.TotalMilliseconds - _stpWatch.ElapsedMilliseconds) < 0;

        public string TimeLeftStr => TimeLeft.ToString(@"m\:ss");


        private void OnTimerElapsed(object sender, EventArgs e)
        {
            TimeChanged.Invoke(TimeLeftStr, e);

            if (_mustStop)
            {
                CountDownFinished.Invoke(sender, e);
                _stpWatch.Stop();
                timer.Enabled = false;
            }
        }

        public CountdownTimer(int min, int sec)
        {
            SetTime(min, sec);
            Init();
        }


        public CountdownTimer()
        {
            Init();
        }

        private void Init()
        {
            StepMs = 1000;
            timer.Elapsed += OnTimerElapsed;
        }

        public void SetTime(TimeSpan ts)
        {
            _max = ts;
        }

        public void SetTime(int min, int sec = 0) => SetTime(TimeSpan.FromSeconds(min * 60 + sec));


        public void Start()
        {
            timer.Start();
            _stpWatch.Start();
        }

        public void Pause()
        {
            timer.Stop();
            _stpWatch.Stop();
        }

        public void Stop()
        {
            Reset();
            Pause();
        }

        public void Reset()
        {
            _stpWatch.Reset();
        }

        public void Restart()
        {
            _stpWatch.Reset();
            timer.Start();
        }

        public void Dispose() => timer.Dispose();
    }
}
