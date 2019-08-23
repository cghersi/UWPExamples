//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

#define PROFILE

// ReSharper disable UnusedParameter.Local : it is used when PROFILE is active

using System.Diagnostics;
using System.Text;
#if PROFILE
using System.Collections.Generic;

#endif

namespace InputManagerWithCoreIndependentInputSource
{
	/// <summary>
	/// Very lightweight collector of time measurements for performance analysis. <br/>
	/// If PROFILE pre-compilation constant is defined, it allocates a Stopwatch and collects times + at the end of the method
	/// logs the results. <br/>
	/// Otherwise it just doesn't perform any action. <br/>
	/// Therefore it could be left also in the production code without penalties in the performance
	/// (commenting the PROFILE pre-compilation constant). <br/>
	/// Logs of results is also piloted by the level of the profiler, together with the DEFAULT_LEVEL. <br/>
	/// If the level specified in the constructor is >= DEFAULT_LEVEL, then the results are collected and logged. <br/>
	/// </summary>
	public class LTClassProfiler
	{
#if PROFILE
		public const LTProfilerLevel DEFAULT_LEVEL = LTProfilerLevel.High;

		private readonly string m_name;
		private readonly Dictionary<int, Stopwatch> m_timers;
		private readonly List<double> m_times;
		private int m_count;
		private readonly LTProfilerLevel m_level;
#endif

		/// <summary>
		/// Sets up a new profiler instance.
		/// It also starts the Stopwatch for the time measurement.
		/// </summary>
		/// <param name="name">Name that will used in the log at the end of the collection.</param>
		/// <param name="level">level of profiling. Determines whether the results will be collected and written onto the log.
		/// If this level is >= DEFAULT_LEVEL, then the results are collected and logged.</param>
		public LTClassProfiler(string name, LTProfilerLevel level = LTProfilerLevel.Low)
		{
#if PROFILE
			m_name = name;
			m_timers = new Dictionary<int, Stopwatch>();
			m_times = new List<double>();
			m_level = level;
			m_count = 0;
#endif
		}

		public int Start()
		{
			int pointer = m_count++;
#if PROFILE
			if (m_level < DEFAULT_LEVEL)
				return pointer;

			while (m_timers.ContainsKey(pointer))
			{
				pointer = m_count++;
			}
			m_timers.Add(pointer, Stopwatch.StartNew());
#endif
			return pointer;
		}

		public void CollectResults(int pointer)
		{
#if PROFILE
			if (m_level < DEFAULT_LEVEL)
				return;

			if (m_timers.ContainsKey(pointer))
			{
				m_timers[pointer].Stop();
				m_times.Add(m_timers[pointer].Elapsed.TotalMilliseconds);
			}
#endif
		}

		public string PrintResults()
		{
			double totalTime = 0;
			foreach (double t in m_times)
			{
				totalTime += t;
			}
			m_timers.Clear();

			StringBuilder sb = new StringBuilder("PROFILER ").Append(m_name).Append(": ");
			sb.AppendFormat("TOTAL => {0:####.000}ms for {1} samples; ", totalTime, m_times.Count);

			if (m_times.Count > 0)
				sb.AppendFormat(" AVG => {0:####.000}ms", totalTime / m_times.Count);

			m_times.Clear();

			return sb.ToString();
		}
	}

	public enum LTProfilerLevel
	{
		High = 100,
		Medium = 10,
		Low = 1
	}
}
