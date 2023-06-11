namespace MauiBabyKickCounter;

public partial class MainPage : ContentPage
{
	int count = 0;
    DateTime timeStarted;
    IDispatcherTimer timer;
    private string timeRan;
    private TimeSpan ranTimeSpan;

    public MainPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        count = 0;
    }

    private async void OnTimerStartClicked(object sender, EventArgs e)
	{
        if (timer == null)
        {
            this.timer = Application.Current.Dispatcher.CreateTimer();
            this.timer.Interval = TimeSpan.FromSeconds(1);
            this.timer.Tick += (s, e) => this.TimerTicks();
            lblTimer.Text = "0:0:0";
            lblTimer.IsVisible = true;
            this.timeStarted = DateTime.Now;
            this.timer.Start();

            TimerBtn.Text = "Stop Timing";
            TimerBtn.BackgroundColor = Color.FromArgb("#ff4d4d");
            lblInstructions.Text = "Click on the baby above to register a movement.";
            lblMovement.Text = "0";
            lblMovement.IsVisible = true;
        }
        else
        {
            this.timer.Stop();
            TimerBtn.Text = "Start Timing";
            TimerBtn.BackgroundColor = Color.FromArgb("#1aff90");
            lblTimer.IsVisible = false;
            this.timer = null;
            lblInstructions.Text = "Let's count those movements together!";
            lblMovement.IsVisible = false;
            await CheckBabyMovement();
            this.count = 0;
        }
        
    }

    private void TimerTicks()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            //Update view herel
            var timeSpan = DateTime.Now - this.timeStarted;
            this.ranTimeSpan = timeSpan;
            this.timeRan = $"{timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}";
            lblTimer.Text = this.timeRan;
        });
    }

    private async void btnBaby_Clicked(object sender, EventArgs e)
    {
        bool timerAvailable = false;
        if (this.timer != null)
        {
            if (this.timer.IsRunning)
            {
                timerAvailable = true;
                await btnBaby.ScaleTo(1.2, 100);
                count++;
                lblMovement.Text = count.ToString();
                await btnBaby.ScaleTo(1, 100);

            }
        }

        if (!timerAvailable)
        {
            await DisplayAlert("", "You need to start the timer", "OK");
        }
    }

    private async Task CheckBabyMovement()
    {
        if (count > 0)
        {
            var totalSeconds = this.ranTimeSpan.TotalSeconds;
            var movePerSecond = this.count / totalSeconds;
            var movePerMinute = movePerSecond * 60;
            var movePerHour = Math.Round(movePerMinute * 60, 0);

            string time = "minutes";
            if (this.ranTimeSpan.Seconds > 0)
            {
                time = "seconds";
            }
            else if(this.ranTimeSpan.Minutes > 0)
            {
                time = "minutes";
            }
            else if(this.ranTimeSpan.Hours > 0)
            {
                time = "hours";
            }

            await DisplayAlert("", $"Your baby moved {this.count} times over the period of {this.timeRan} {time}. " +
                $"\n Based on this information your baby moves {movePerHour} times on average per hour.", "OK");
        }
    }
}

