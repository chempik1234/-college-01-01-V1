namespace SF2022UserDanisLib
{
	public class Calculations
	{
		public string[] AvailablePeriods(TimeSpan[] startTimes, int[] durations, TimeSpan beginWorkingTime, TimeSpan endWorkingTime, int consultationTime)
		{
			if (startTimes.Length != durations.Length)
			{
				throw new ArgumentException("startTimes.Length != durations.Length");
			}
			if (consultationTime <= 0)
			{
				throw new ArgumentException("consultationTime <= 0");
			}
			if (endWorkingTime < beginWorkingTime)
			{
				throw new ArgumentException("endWorkingTime < beginWorkingTime");
			}

			if (endWorkingTime == beginWorkingTime)
			{
				return new string[0];
			}

			List<string> result = new List<string>();

			int nextSleepIndex = 0;
			TimeSpan nextSleepEnd = new();

			TimeSpan currentTime = beginWorkingTime;

			if (startTimes.Length > 0)
			{
				if (startTimes[0] < beginWorkingTime)
				{
					currentTime = startTimes[0] + new TimeSpan(0, durations[0], 0);
					nextSleepIndex++;
				}
				if (nextSleepIndex < startTimes.Length)
					nextSleepEnd = startTimes[nextSleepIndex] + new TimeSpan(0, durations[nextSleepIndex], 0);
			}

			TimeSpan consultationTimeSpan = new TimeSpan(0, minutes: consultationTime, 0);

			while (currentTime.Add(consultationTimeSpan) <= endWorkingTime)
			{
				var currentConsultationEnd = currentTime.Add(consultationTimeSpan);

				if (durations.Length > 0 && nextSleepIndex < durations.Length && Overlap(currentTime, currentConsultationEnd, startTimes[nextSleepIndex], nextSleepEnd))
				{
					currentTime = nextSleepEnd;
					nextSleepIndex++;
					while (nextSleepIndex < startTimes.Length && startTimes[nextSleepIndex] + new TimeSpan(0, durations[nextSleepIndex], 0) < nextSleepEnd)
					{
						nextSleepIndex++;
					}
					if (nextSleepIndex < startTimes.Length)
						nextSleepEnd = startTimes[nextSleepIndex] + new TimeSpan(0, durations[nextSleepIndex], 0);

					continue;
				}

				result.Add(s2(currentTime.Hours) + ":" + s2(currentTime.Minutes) + "-" + s2(currentConsultationEnd.Hours) + ":" + s2(currentConsultationEnd.Minutes));
				currentTime = currentConsultationEnd;
			}

			return result.ToArray();
		}

		private bool Overlap(TimeSpan workStart, TimeSpan workEnd, TimeSpan sleepStart, TimeSpan sleepEnd) { 
			return !(workEnd <= sleepStart || workStart >= sleepEnd);
		}
		///
		/// ||||||||========== OK
		/// ========|||||||||| OK
		/// 
		/// |||||||||||||||||||||||||||||||| NO
		///            ============
		/// 
		/// =============================== NO
		///        |||||||||||||||
		/// 
		///            
		/// ||||||||||||||||||    NO
		///         ===============
		///         
		/// ==========          NO
		///       ||||||||
		///
		
		private string s2 (int n)
		{
			if (n >= 10)
				return n.ToString();
			return "0" + n;
		}
	}
}
