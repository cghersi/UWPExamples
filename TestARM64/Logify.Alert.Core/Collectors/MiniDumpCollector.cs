using System;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace DevExpress.Logify.Core.Internal {
    public class MiniDumpCollector : IInfoCollector {
        public const string DumpGuidKey = "miniDumpGuid";
        public const string DumpFileNameKey = "miniDumpFileName";
        public const string DumpAttachKey = "miniDumpAttach";

        [HandleProcessCorruptedStateExceptions]
        public virtual void Process(Exception ex, ILogger logger) {
            try {
                Guid dumpGuid = Guid.NewGuid();
                string fileName = dumpGuid.ToString() + ".dmp";
                fileName = Path.Combine(Path.GetTempPath(), fileName);
                if (MiniDumpWriter.Write(fileName, MiniDumpType.Normal)) {
                    byte[] content = File.ReadAllBytes(fileName);
                    try {
                        File.Delete(fileName);
                    }
                    catch {
                    }

                    Attachment attach = new Attachment();
                    attach.Content = content;
                    attach.MimeType = "application/vnd.tcpdump.pcap";
                    attach.Name = "miniDump.dmp";

                    logger.Data[DumpAttachKey] = attach;
                }
            }
            catch {
            }
        }
        void WriteMiniDumpExternally(Exception ex, ILogger logger) {
            logger.BeginWriteObject("miniDump");
            try {
                Guid dumpGuid = Guid.NewGuid();
                string fileName = dumpGuid.ToString() + ".dmp";
                fileName = Path.Combine(Path.GetTempPath(), fileName);
                if (MiniDumpWriter.Write(fileName, MiniDumpType.Normal)) {
                    logger.WriteValue("dumpGuid", dumpGuid.ToString());
                    logger.Data[DumpGuidKey] = dumpGuid.ToString();
                    logger.Data[DumpFileNameKey] = fileName;
                }
            }
            catch {
            }
            finally {
                logger.EndWriteObject("miniDump");
            }
        }
    }
    public class DeferredMiniDumpCollector : IInfoCollector {
        public virtual void Process(Exception ex, ILogger logger) {
            object attachObject;
            if (!logger.Data.TryGetValue(MiniDumpCollector.DumpAttachKey, out attachObject))
                return;
            Attachment attach = attachObject as Attachment;
            if (attach == null)
                return;

            AttachmentCollector collector = new AttachmentCollector(attach, 0, Int32.MaxValue, "miniDump");
            collector.PerformProcess(ex, logger);
        }
    }

    [Flags]
    enum MiniDumpType : uint {
        Normal = 0x00000000,
        WithDataSegs = 0x00000001,
        WithFullMemory = 0x00000002,
        WithHandleData = 0x00000004,
        FilterMemory = 0x00000008,
        ScanMemory = 0x00000010,
        WithUnloadedModules = 0x00000020,
        WithIndirectlyReferencedMemory = 0x00000040,
        FilterModulePaths = 0x00000080,
        WithProcessThreadData = 0x00000100,
        WithPrivateReadWriteMemory = 0x00000200,
        WithoutOptionalData = 0x00000400,
        WithFullMemoryInfo = 0x00000800,
        WithThreadInfo = 0x00001000,
        WithCodeSegs = 0x00002000,
        WithoutAuxiliaryState = 0x00004000,
        WithFullAuxiliaryState = 0x00008000,
        WithPrivateWriteCopyMemory = 0x00010000,
        IgnoreInaccessibleMemory = 0x00020000,
        ValidTypeFlags = 0x0003ffff,
    };
    class MiniDumpWriter {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]  // Pack=4 is important! So it works also for x64!
        struct MiniDumpExceptionInformation {
            public uint ThreadId;
            public IntPtr ExceptionPointers;
            public int ClientPointers;
        }

        [DllImport("dbghelp.dll",
          EntryPoint = "MiniDumpWriteDump",
          CallingConvention = CallingConvention.StdCall,
          CharSet = CharSet.Unicode,
          ExactSpelling = true, SetLastError = true)]
        static extern bool MiniDumpWriteDump(
          IntPtr hProcess,
          uint processId,
          IntPtr hFile,
          uint dumpType,
          ref MiniDumpExceptionInformation expParam,
          IntPtr userStreamParam,
          IntPtr callbackParam);

        [DllImport("kernel32.dll", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        static extern uint GetCurrentThreadId();
        [DllImport("kernel32.dll", EntryPoint = "GetCurrentProcess", ExactSpelling = true)]
        static extern IntPtr GetCurrentProcess();
        [DllImport("kernel32.dll", EntryPoint = "GetCurrentProcessId", ExactSpelling = true)]
        static extern uint GetCurrentProcessId();

        [HandleProcessCorruptedStateExceptions]
        public static bool Write(string fileName, MiniDumpType dumpType) {
            try {
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    MiniDumpExceptionInformation exp;
                    exp.ThreadId = GetCurrentThreadId();
                    exp.ClientPointers = 0;
                    exp.ExceptionPointers = GetExceptionPointers();
                    return MiniDumpWriteDump(
                      GetCurrentProcess(),
                      GetCurrentProcessId(),
                      fs.SafeFileHandle.DangerousGetHandle(),
                      (uint)dumpType,
                      ref exp,
                      IntPtr.Zero,
                      IntPtr.Zero);
                }
            }
            catch {
                return false;
            }
        }
#if NETSTANDARD
        [HandleProcessCorruptedStateExceptions]
        static IntPtr GetExceptionPointers() {
            // https://github.com/dotnet/coreclr/pull/11125
            try {
                MethodInfo methodGetExceptionPointers = typeof(Marshal).GetMethod("GetExceptionPointers");
                if (methodGetExceptionPointers != null)
                    return (IntPtr)methodGetExceptionPointers.Invoke(null, null);
                else
                    return IntPtr.Zero;
            }
            catch {
                return IntPtr.Zero;
            }
        }
#else
        static IntPtr GetExceptionPointers() {
            return System.Runtime.InteropServices.Marshal.GetExceptionPointers();
        }
#endif
    }
}