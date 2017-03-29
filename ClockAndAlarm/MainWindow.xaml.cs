using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace ClockAndAlarm
{
	public partial class MainWindow : Window
	{
		private List<DateTime> _listOfAlarms = new List<DateTime>();
		private List<DispatcherTimer> _listOfDispatcherTimerForAlarm = new List<DispatcherTimer>();
		private DispatcherTimer _dispatcherTimerForDateAndTime;

		public MainWindow()
		{
			InitializeComponent();
			_dispatcherTimerForDateAndTime = new DispatcherTimer();
			_dispatcherTimerForDateAndTime.Tick += new EventHandler(DispatcherTimer_Tick);
			_dispatcherTimerForDateAndTime.Interval = new TimeSpan(0, 0, 1);
			_dispatcherTimerForDateAndTime.Start();
		}

		private void DispatcherTimer_Tick(object sender, EventArgs e)
		{
			DataLabel.Content = "Data: " + DateTime.Now.ToShortDateString();
			TimeLabel.Content = "Time: " + DateTime.Now.ToLongTimeString();
		}

		private void AlarmCheck(object sender, EventArgs e)
		{
			if (_listOfAlarms.Count != 0)
			{
				var indexOfLastAlarm = _listOfAlarms.Count - 1;
				if (DateTime.Now.Equals(_listOfAlarms[indexOfLastAlarm]))
				{
					_listOfAlarms.Remove(_listOfAlarms[indexOfLastAlarm]);
					SortDate();
					UpdateListAlarm();
					_listOfDispatcherTimerForAlarm.Remove(_listOfDispatcherTimerForAlarm[0]);
					var indexofLastDispatcher = _listOfDispatcherTimerForAlarm.Count - 1;
					if (indexofLastDispatcher >= 0)
					{
						_listOfDispatcherTimerForAlarm[0].Start();
					}
					MessageBox.Show("Alarm on " + DateTime.Now.ToString());
				}
			}
		}

		private void CreateAlarm(object sender, RoutedEventArgs e)
		{
			DateTime newAlarm;
			var date = TextBoxNewAlarm.Text;
			if (DateTime.TryParse(date, out newAlarm) && newAlarm > DateTime.Now)
			{
				_listOfAlarms.Add(newAlarm);
				SortDate();
				UpdateListAlarm();
				CreateNewDispatcherTimer();
			}
			else
			{
				MessageBox.Show("Invalid format of date or invalid date.\nYou cannot possible create alarm for past.");
			}
		}

		private void CreateNewDispatcherTimer()
		{
			var dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Tick += new EventHandler(AlarmCheck);
			dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
			_listOfDispatcherTimerForAlarm.Add(dispatcherTimer);
			var lastIndexOfDispatcherTimers = _listOfDispatcherTimerForAlarm.Count;
			if (lastIndexOfDispatcherTimers == 1)
			{
				_listOfDispatcherTimerForAlarm[lastIndexOfDispatcherTimers - 1].Start();
			}
		}

		private void SortDate()
		{
			_listOfAlarms = _listOfAlarms.OrderByDescending(x => x.Date).ThenByDescending(x => x.TimeOfDay).ToList();
		}
		private void UpdateListAlarm()
		{
			StringBuilder content = new StringBuilder();
			foreach (DateTime dateTime in _listOfAlarms)
			{
				content.AppendLine(dateTime.ToString());
			}
			AlarmListLabel.Content = content.ToString();
		}
	}
}

