using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SyncVsAsync
{
    public sealed partial class MainPage : Page
    {
        public DispatcherTimer clockTimer = new DispatcherTimer();

        public MainPage()
        {
            this.InitializeComponent();

            // I like to control the appearance of my app
            ApplicationView.PreferredLaunchViewSize = new Size(500, 450);
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

        /// <summary>
        /// This parses the input text and executes the FindPrimeNumber(int n) function.
        /// </summary>
        private void DoSyncTaskButton_click(object sender, RoutedEventArgs e)
        {
            int x;
            Int32.TryParse(PrimeNumberTextBlock.Text, out x);
            long nthPrime = FindPrimeNumber(x);
            TaskOutput.Text = nthPrime.ToString();
        }

        /// <summary>
        /// This parses the input text and awaits the FindPrimeNumberAsync(int n) function.
        /// </summary>
        private async void DoAsyncTaskButton_click(object sender, RoutedEventArgs e)
        {
            int x;
            Int32.TryParse(PrimeNumberTextBlock.Text, out x);
            long nthPrime = await FindPrimeNumberAsync(x);
            TaskOutput.Text = nthPrime.ToString();
        }

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

        /// <summary>
        /// This function asynchronously finds the nth prime number.
        /// </summary>
        /// <param name="n">The prime number we want to find, according to OEIS A000040.</param>
        /// <returns>The nth prime number.</returns>
        public async Task<long> FindPrimeNumberAsync(int n)
        {
            // Code inside an async method will execute synchronously until it arrives at an "await" statement
            TaskOutput.Text = "Calculating...";
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
    }
}
