using SF2022UserDanisLib;

[TestClass]
public class UnitTest1
{
	Calculations calc = new Calculations();

	private struct TestParams
	{
		public TimeSpan[] startTimes;
		public int[] durations;
		public TimeSpan beginWorkingTime;
		public TimeSpan endWorkingTime;
		public int consultationTime;
	}

	// Тест 1: Базовый случай - несколько занятых периодов внутри рабочего дня
	[TestMethod]
	public void TestMethod1()
	{
		universalTest(new TestParams
		{
			startTimes = new TimeSpan[] { ts(10, 0), ts(11, 0), ts(15, 0), ts(15, 30), ts(16, 50) },
			durations = new int[] { 60, 30, 10, 10, 40 },
			beginWorkingTime = ts(8, 0),
			endWorkingTime = ts(18, 0),
			consultationTime = 30
		}, new string[] { "08:00-08:30", "08:30-09:00", "09:00-09:30", "09:30-10:00", "11:30-12:00", "12:00-12:30", "12:30-13:00", "13:00-13:30", "13:30-14:00", "14:00-14:30", "14:30-15:00", "15:40-16:10", "16:10-16:40", "17:30-18:00" });
	}

	// Тест 2: Короткий рабочий день без занятых периодов
	[TestMethod]
	public void TestMethod2()
	{
		universalTest(new TestParams
		{
			startTimes = new TimeSpan[] { },
			durations = new int[] { },
			beginWorkingTime = ts(8, 0),
			endWorkingTime = ts(8, 6),
			consultationTime = 2
		}, new string[] { "08:00-08:02", "08:02-08:04", "08:04-08:06" });
	}

	// Тест 3: Занятый период в начале рабочего дня
	[TestMethod]
	public void TestMethod3()
	{
		universalTest(new TestParams
		{
			startTimes = new TimeSpan[] { ts(8, 0), ts(9, 0) },
			durations = new int[] { 60, 30 },
			beginWorkingTime = ts(8, 0),
			endWorkingTime = ts(12, 0),
			consultationTime = 30
		}, new string[] { "09:30-10:00", "10:00-10:30", "10:30-11:00", "11:00-11:30", "11:30-12:00" });
	}

	// Тест 4: Занятый период в конце рабочего дня
	[TestMethod]
	public void TestMethod4()
	{
		universalTest(new TestParams
		{
			startTimes = new TimeSpan[] { ts(11, 30), ts(11, 45) },
			durations = new int[] { 15, 30 },
			beginWorkingTime = ts(9, 0),
			endWorkingTime = ts(12, 0),
			consultationTime = 15
		}, new string[] { "09:00-09:15", "09:15-09:30", "09:30-09:45", "09:45-10:00", "10:00-10:15", "10:15-10:30", "10:30-10:45", "10:45-11:00", "11:00-11:15", "11:15-11:30" });
	}

	// Тест 5: Занятые периоды перекрывают весь рабочий день
	[TestMethod]
	public void TestMethod5()
	{
		universalTest(new TestParams
		{
			startTimes = new TimeSpan[] { ts(9, 0), ts(13, 0) },
			durations = new int[] { 240, 60 }, // 4 часа и 1 час
			beginWorkingTime = ts(9, 0),
			endWorkingTime = ts(15, 0),
			consultationTime = 60
		}, new string[] { "14:00-15:00" });
	}

	// Тест 6: Занятые периоды с небольшими промежутками между ними
	[TestMethod]
	public void TestMethod6()
	{
		universalTest(new TestParams
		{
			startTimes = new TimeSpan[] { ts(9, 0), ts(9, 45), ts(10, 30) },
			durations = new int[] { 30, 15, 45 },
			beginWorkingTime = ts(9, 0),
			endWorkingTime = ts(12, 0),
			consultationTime = 20
		}, new string[] { "10:00-10:20", "11:15-11:35", "11:35-11:55" });
	}

	// Тест 7: Консультация длиннее доступных промежутков
	[TestMethod]
	public void TestMethod7()
	{
		universalTest(new TestParams
		{
			startTimes = new TimeSpan[] { ts(9, 0), ts(9, 30), ts(10, 0), ts(10, 15) },
			durations = new int[] { 20, 10, 10, 45 },
			beginWorkingTime = ts(9, 0),
			endWorkingTime = ts(12, 0),
			consultationTime = 30
		}, new string[] { "11:00-11:30", "11:30-12:00" });
	}

	// Тест 8: Граничные случаи - занятые периоды на границах рабочего дня
	[TestMethod]
	public void TestMethod8()
	{
		universalTest(new TestParams
		{
			startTimes = new TimeSpan[] { ts(8, 0), ts(11, 30) },
			durations = new int[] { 30, 90 }, // 0.5 ч и 1.5 ч
			beginWorkingTime = ts(8, 0),
			endWorkingTime = ts(13, 0),
			consultationTime = 45
		}, new string[] { "08:30-09:15", "09:15-10:00", "10:00-10:45", "10:45-11:30" });
	}

	// Тест 9: Несколько последовательных свободных окон
	[TestMethod]
	public void TestMethod9()
	{
		universalTest(new TestParams
		{
			startTimes = new TimeSpan[] { ts(10, 0), ts(10, 40), ts(11, 20) },
			durations = new int[] { 30, 30, 30 },
			beginWorkingTime = ts(9, 0),
			endWorkingTime = ts(13, 0),
			consultationTime = 20
		}, new string[] { "09:00-09:20", "09:20-09:40", "09:40-10:00", "11:50-12:10", "12:10-12:30", "12:30-12:50" });
	}

	// Тест 10: Пересекающиеся занятые периоды (если система поддерживает)
	[TestMethod]
	public void TestMethod10()
	{
		universalTest(new TestParams
		{
			startTimes = new TimeSpan[] { ts(9, 0), ts(9, 45), ts(10, 0) },
			durations = new int[] { 60, 30, 45 },
			beginWorkingTime = ts(9, 0),
			endWorkingTime = ts(13, 0),
			consultationTime = 25
		}, new string[] { "10:45-11:10", "11:10-11:35", "11:35-12:00", "12:00-12:25", "12:25-12:50" });
	}

	// Тест 11: Исключение - несовпадающие длины массивов
	[TestMethod]
	[ExpectedException(typeof(ArgumentException))]
	public void TestMethod11()
	{
		universalTestException(new TestParams
		{
			startTimes = new TimeSpan[] { },
			durations = new int[] { 1 },
			beginWorkingTime = ts(8, 0),
			endWorkingTime = ts(8, 6),
			consultationTime = 2
		});
	}

	// Тест 12: Консультация равна 0 минут
	[TestMethod]
	[ExpectedException(typeof(ArgumentException))]
	public void TestMethod12()
	{
		universalTestException(new TestParams
		{
			startTimes = new TimeSpan[] { ts(10, 0), ts(11, 0) },
			durations = new int[] { 30, 45 },
			beginWorkingTime = ts(9, 0),
			endWorkingTime = ts(12, 0),
			consultationTime = 0
		}
            // Каждая минута доступна для 0-минутной консультации, что некорректно
        );
	}

	private void universalTest(TestParams testParams, string[] expectedResult)
	{
		string[] result = calc.AvailablePeriods(testParams.startTimes, testParams.durations, testParams.beginWorkingTime, testParams.endWorkingTime, testParams.consultationTime);
		CollectionAssert.AreEqual(expectedResult, result);
	}

	private void universalTestException(TestParams testParams)
	{
		calc.AvailablePeriods(testParams.startTimes, testParams.durations, testParams.beginWorkingTime, testParams.endWorkingTime, testParams.consultationTime);
	}

	private TimeSpan ts(int hour, int min)
	{
		return new TimeSpan(hour, min, 0);
	}
}