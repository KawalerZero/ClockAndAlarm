using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace ClockAndAlarm
{
	public partial class MainWindow : Window
	{
		private List<DateTime> _listOfAlarms = new List<DateTime>();
		private List<DispatcherTimer> _listOfTimersForAlarm = new List<DispatcherTimer>();
		private DispatcherTimer _timerForDateAndTime;

		public MainWindow()
		{
			InitializeComponent();
			InitializeTimerForDateAndTime();
		}
		private void InitializeTimerForDateAndTime()
		{
			_timerForDateAndTime = new DispatcherTimer();
			_timerForDateAndTime.Tick += new EventHandler(ChangeDateAndTime);
			_timerForDateAndTime.Interval = new TimeSpan(0, 0, 1);
			_timerForDateAndTime.Start();
		}
		private void ChangeDateAndTime(object sender, EventArgs e)
		{
			DateLabel.Content = "Date: " + DateTime.Now.ToShortDateString();
			TimeLabel.Content = "Time: " + DateTime.Now.ToLongTimeString();
		}

		private void AlarmCheck(object sender, EventArgs e)
		{
			if (_listOfAlarms.Count == 0) return;
			var indexOfLastAlarm = _listOfAlarms.Count - 1;
			if (DateTime.Compare(DateTime.Now, _listOfAlarms[indexOfLastAlarm]) < 0) return;
			_listOfAlarms.Remove(_listOfAlarms[indexOfLastAlarm]);
			SortDescendingListOfAlarmsByDate();
			UpdateListAlarm();
			_listOfTimersForAlarm.Remove(_listOfTimersForAlarm[0]);
			var indexofLastDispatcher = _listOfTimersForAlarm.Count - 1;
			if (indexofLastDispatcher >= 0)
			{
				_listOfTimersForAlarm[0].Start();
			}
			MessageBox.Show("Alarm on " + DateTime.Now.ToString(CultureInfo.CurrentCulture));
		}

		private void CreateAlarm(object sender, RoutedEventArgs e)
		{
			DateTime newAlarm;
			var date = TextBoxNewAlarm.Text;
			if (DateTime.TryParse(date, out newAlarm) && newAlarm > DateTime.Now)
			{
				_listOfAlarms.Add(newAlarm);
				SortDescendingListOfAlarmsByDate();
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
			_listOfTimersForAlarm.Add(dispatcherTimer);
			var lastIndexOfDispatcherTimers = _listOfTimersForAlarm.Count;
			if (lastIndexOfDispatcherTimers == 1)
			{
				_listOfTimersForAlarm[lastIndexOfDispatcherTimers - 1].Start();
			}
		}

		private void SortDescendingListOfAlarmsByDate()
		{
			_listOfAlarms = _listOfAlarms.OrderByDescending(x => x.Date).ThenByDescending(x => x.TimeOfDay).ToList();
		}
		private void UpdateListAlarm()
		{
			var content = new StringBuilder();
			foreach (var dateTime in _listOfAlarms)
			{
				content.AppendLine(dateTime.ToString(CultureInfo.CurrentCulture));
			}
			AlarmListLabel.Content = content.ToString();
		}
	}
}

