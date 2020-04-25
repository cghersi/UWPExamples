//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

#define PROFILE

// ReSharper disable UnusedParameter.Local : it is used when PROFILE is active
#if PROFILE
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
#endif

namespace FoxitTestBench
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
	public class LTProfiler
	{
#if PROFILE
		public const LTProfilerLevel DEFAULT_LEVEL = LTProfilerLevel.Medium;

		private readonly Stopwatch m_sw;
		private readonly string m_name;
		private readonly Dictionary<string, double> m_times;
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
		public LTProfiler(string name, LTProfilerLevel level = LTProfilerLevel.Low)
		{
#if PROFILE
			m_level = level;
			if (m_level < DEFAULT_LEVEL)
				return;

			m_name = name;
			m_sw = Stopwatch.StartNew();
			m_times = new Dictionary<string, double>();
			m_count = 0;
#endif
		}

		public void Restart(bool resetCounter = true)
		{
#if PROFILE
			if (m_level < DEFAULT_LEVEL)
				return;

			m_sw.Restart();
			if (resetCounter)
				m_count = 0;
#endif
		}

		public void ReadTime(string name)
		{
#if PROFILE
			if (m_level < DEFAULT_LEVEL)
				return;

			m_sw.Stop();

			double time = m_sw.Elapsed.TotalMilliseconds;
			if (m_times.ContainsKey(name))
				m_times[name] += time;
			else
				m_times.Add(name, time);

			m_sw.Restart();
#endif
		}

		public void CollectResults(string name, bool printResults = true)
		{
#if PROFILE
			if (m_level < DEFAULT_LEVEL)
				return;

			ReadTime(name);
			m_count++;
			m_sw.Stop();

			if (!printResults)
				return;

			Debug.WriteLine(PrintResults());
#endif
		}

		public string PrintResults()
		{
#if PROFILE
			if (m_level < DEFAULT_LEVEL)
				return "Too Low Level for Profiler";

			StringBuilder sb = new StringBuilder("PROF ").Append(m_name).Append(": ");
			double total = 0;
			foreach (KeyValuePair<string, double> pair in m_times)
			{
				total += pair.Value;
				sb.Append(pair.Key).AppendFormat(" => {0:####.000}ms", pair.Value).Append("; ");
			}

			sb.AppendFormat("TOTAL => {0:####.000}ms; ", total);

			if (m_count > 1)
			{
				foreach (KeyValuePair<string, double> pair in m_times)
				{
					sb.Append("AVERAGE: ").Append(pair.Key).AppendFormat(" => {0:####.000}ms", pair.Value / m_count).Append("; ");
				}
				sb.AppendFormat("TOTAL AVG => {0:####.000}ms", total / m_count);
			}

			return sb.ToString();
#else
			return "Profiler not active";
#endif
		}
	}

	public enum LTProfilerLevel
	{
		High = 100,
		Medium = 10,
		Low = 1
	}
}
