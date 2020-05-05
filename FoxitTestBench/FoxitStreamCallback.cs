using System.Runtime.InteropServices.WindowsRuntime;
using foxit.common.fxcrt;

namespace FoxitTestBench
{
	public class FoxitStreamCallback : IFileStreamCallback
	{
		private static FoxitStreamCallback s_shared;

		private byte[] m_bytes;
		private int m_pos = 0;

		public IFileStreamCallback Retain()
		{
			if (s_shared == null) 
				s_shared = new FoxitStreamCallback();
			return s_shared;
		}

		public void Release()
		{
			// nothing to do here
		}

		public int GetSize()
		{
			return m_bytes?.Length ?? 0;
		}

		public bool IsEOF()
		{
			return m_pos == GetSize();
		}

		public int GetPosition()
		{
			return m_pos;
		}

		public bool ReadBlock(byte[] buffer, int offset, uint size)
		{
			long maxSize = offset + size;
			if (maxSize >= GetSize())
				return false;
			if ((buffer == null) || (m_bytes == null))
				return false;
			for (int i = 0; i < size; i++)
			{
				buffer[i] = m_bytes[i + offset];
			}

			m_pos = (int)maxSize;

			return true;
		}

		public bool WriteBlock(byte[] buffer, int offset, uint size)
		{
			// shortcut for easy cases:
			if ((m_bytes == null) && (offset == 0))
			{
				m_bytes = buffer;
				return true;
			}

			if ((buffer == null) || (offset < 0))
				return false;
			long maxSize = offset + size;
			if (m_bytes == null)
				m_bytes = new byte[maxSize];
			else if (maxSize >= GetSize())
			{
				byte[] tmp = new byte[maxSize];
				for (int i = 0; i < m_bytes.Length; i++)
				{
					tmp[i] = m_bytes[i];
				}
				m_bytes = tmp;
			}

			for (int i = 0; i < size; i++)
			{
				m_bytes[i + offset] = buffer[i];
			}

			return true;
		}

		public bool Flush()
		{
			return true;
		}

		public byte[] ToByteArray()
		{
			return m_bytes;
		}
	}
}
