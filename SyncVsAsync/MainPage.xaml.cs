using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SyncVsAsync
{
    public sealed partial class MainPage : Page
    {
        public DispatcherTimer clockTimer = new DispatcherTimer();

        public MainPage()
        {
            this.InitializeComponent();

            // I like to control the appearance of my apps
            ApplicationView.PreferredLaunchViewSize = new Size(720, 450);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            // Let's simulate an ultra-responsive UI by having a clock update every 10 ms. This is probably the precision limit of a DispatcherTimer, anyway.
            clockTimer.Tick += clockTimer_tick;
            clockTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            clockTimer.Start();
        }

        /// <summary>
        /// This updates the UI's TimeBlock every 10 milliseconds
        /// </summary>
        private void clockTimer_tick(object sender, object e)
        {
            TimeBlock.Text = DateTime.Now.ToString("hh:mm:ss.ff");
        }

        #region Button Actions
        /// <summary>
        /// This parses the input text and executes the FindPrimeNumber(int n) function.
        /// </summary>
        private void DoSerialSyncTaskButton_click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int x;
            Int32.TryParse(PrimeNumberTextBlock.Text, out x);
            int x1;
            Int32.TryParse(PrimeNumberTextBlock1.Text, out x1);
            int x2;
            Int32.TryParse(PrimeNumberTextBlock2.Text, out x2);
            long nthPrime = FindPrimeNumber(x);
            long nthPrime1 = FindPrimeNumber(x1);
            long nthPrime2 = FindPrimeNumber(x2);
            TaskOutput.Text = nthPrime.ToString();
            TaskOutput1.Text = nthPrime1.ToString();
            TaskOutput2.Text = nthPrime2.ToString();
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            ElapsedBlock1.Text = String.Format("Serial Synchronous computation took {0:00}.{1:00} seconds", ts.Seconds,ts.Milliseconds / 10);
        }

        /// <summary>
        /// This parses the input text and awaits the FindPrimeNumberAsync(int n) function.
        /// </summary>
        private async void DoSerialAsyncTaskButton_click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int x;
            Int32.TryParse(PrimeNumberTextBlock.Text, out x);
            int x1;
            Int32.TryParse(PrimeNumberTextBlock1.Text, out x1);
            int x2;
            Int32.TryParse(PrimeNumberTextBlock2.Text, out x2);

            //var task = FindPrimeNumberAsync(x, TaskOutput);
            //var task1 = FindPrimeNumberAsync(x1, TaskOutput1);
            //var task2 = FindPrimeNumberAsync(x2, TaskOutput2);

            //// Now, we await them all.
            //await Task.WhenAll(task, task1, task2);

            TaskOutput.Text = (await FindPrimeNumberAsync(x, TaskOutput)).ToString();
            TaskOutput1.Text = (await FindPrimeNumberAsync(x1, TaskOutput1)).ToString();
            TaskOutput2.Text = (await FindPrimeNumberAsync(x2, TaskOutput2)).ToString();

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            ElapsedBlock2.Text = String.Format("Serial Asynchronous computation took {0:00}.{1:00} seconds", ts.Seconds, ts.Milliseconds / 10);
        }

        private void DoParallelSyncTaskButton_click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int x;
            Int32.TryParse(PrimeNumberTextBlock.Text, out x);
            int x1;
            Int32.TryParse(PrimeNumberTextBlock1.Text, out x1);
            int x2;
            Int32.TryParse(PrimeNumberTextBlock2.Text, out x2);

            int[] ns = { x, x1, x2 };
            List<PrimePair> result = new List<PrimePair>();

            var outputCollection = new ConcurrentBag<PrimePair>();
            ParallelLoopResult r = Parallel.ForEach(ns, n =>
            {
                outputCollection.Add(FindPrimeNumberPair(n));
            });

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            ElapsedBlock3.Text = String.Format("Parallel Synchronous computation took {0:00}.{1:00} seconds", ts.Seconds, ts.Milliseconds / 10);
        }

        private async void DoParallelAsyncTaskButton_click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int x;
            Int32.TryParse(PrimeNumberTextBlock.Text, out x);
            int x1;
            Int32.TryParse(PrimeNumberTextBlock1.Text, out x1);
            int x2;
            Int32.TryParse(PrimeNumberTextBlock2.Text, out x2);

            int[] ns = { x, x1, x2 };
            var bag = new ConcurrentBag<PrimePair>();

            var tasks = ns.Select(async item =>
            {
                var response = await FindPrimeNumberParallelAsync(item);
                bag.Add(new PrimePair(item, response));
            });
            await Task.WhenAll(tasks);

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            ElapsedBlock4.Text = String.Format("Parallel aynchronous computation took {0:00}.{1:00} seconds", ts.Seconds, ts.Milliseconds / 10);
        }
        #endregion

        #region Functions and Such
        /// <summary>
        /// This function synchronously finds the nth prime number.
        /// </summary>
        /// <param name="n">The prime number we want to find, according to OEIS A000040.</param>
        /// <returns>The nth prime number.</returns>
        public long FindPrimeNumber(int n)
        {
            int count = 0;
            long a = 2;
            while (count < n)
            {
                long b = 2;
                int prime = 1;
                while (b * b <= a)
                {
                    if (a % b == 0)
                    {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if (prime > 0)
                {
                    count++;
                }
                a++;
            }
            return (--a);
        }

        public PrimePair FindPrimeNumberPair(int n)
        {
            int count = 0;
            long a = 2;
            while (count < n)
            {
                long b = 2;
                int prime = 1;
                while (b * b <= a)
                {
                    if (a % b == 0)
                    {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if (prime > 0)
                {
                    count++;
                }
                a++;
            }
            return new PrimePair(n, (--a));
        }

        /// <summary>
        /// This function asynchronously finds the nth prime number.
        /// </summary>
        /// <param name="n">The prime number we want to find, according to OEIS A000040.</param>
        /// <returns>The nth prime number.</returns>
        public async Task<long> FindPrimeNumberAsync(int n, TextBox tb)
        {
            // Code inside an async method will execute synchronously until it arrives at an "await" statement
            tb.Text = "Calculating...";
            // Once we run into the await statement the code executes asynchronously
            return await Task.Run(() =>
            {
                int count = 0;
                long a = 2;
                while (count < n)
                {
                    long b = 2;
                    int prime = 1;
                    while (b * b <= a)
                    {
                        if (a % b == 0)
                        {
                            prime = 0;
                            break;
                        }
                        b++;
                    }
                    if (prime > 0)
                    {
                        count++;
                    }
                    a++;
                }
                // The method returns a Task<long> but our return type within the await statement is just a long.
                return (--a);
            });
        }

        public async Task<long> FindPrimeNumberParallelAsync(int n)
        {
            // Code inside an async method will execute synchronously until it arrives at an "await" statement
            // Once we run into the await statement the code executes asynchronously
            return await Task.Run(() =>
            {
                int count = 0;
                long a = 2;
                while (count < n)
                {
                    long b = 2;
                    int prime = 1;
                    while (b * b <= a)
                    {
                        if (a % b == 0)
                        {
                            prime = 0;
                            break;
                        }
                        b++;
                    }
                    if (prime > 0)
                    {
                        count++;
                    }
                    a++;
                }
                // The method returns a Task<long> but our return type within the await statement is just a long.
                return (--a);
            });
        }
        #endregion

        private void GetRandomNButton_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            PrimeNumberTextBlock.Text = rnd.Next(1, 1000000).ToString();
            PrimeNumberTextBlock1.Text = rnd.Next(1, 1000000).ToString();
            PrimeNumberTextBlock2.Text = rnd.Next(1, 1000000).ToString();
            ElapsedBlock1.Text = "";
            ElapsedBlock2.Text = "";
            ElapsedBlock3.Text = "";
            ElapsedBlock4.Text = "";
        }
    }

    /// <summary>
    /// This class keeps n and p together in case parallelism separates them.
    /// </summary>
    public class PrimePair
    {
        public int N { get; set; }
        public long P { get; set; }

        public PrimePair(int n, long p)
        {
            N = n;
            P = p;
        }
    }
}
