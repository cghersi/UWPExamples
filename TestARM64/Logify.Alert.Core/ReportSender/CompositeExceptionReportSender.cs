﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DevExpress.Logify.Core.Internal {
    public class CompositeExceptionReportSender : ExceptionReportSenderSkeleton {
        readonly List<IExceptionReportSender> senders = new List<IExceptionReportSender>();
        public IList<IExceptionReportSender> Senders { get { return senders; } }

        public bool StopWhenFirstSuccess { get; set; }
        public override string ApiKey {
            get { return base.ApiKey; }
            set {
                base.ApiKey = value;
                foreach (IExceptionReportSender sender in Senders)
                    sender.ApiKey = value;
            }
        }
        public override string ServiceUrl {
            get { return base.ServiceUrl; }
            set {
                base.ServiceUrl = value;
                foreach (IExceptionReportSender sender in Senders)
                    sender.ServiceUrl = value;
            }
        }
        public override ICredentials ProxyCredentials {
            get { return base.ProxyCredentials; }
            set {
                base.ProxyCredentials = value;
                foreach (IExceptionReportSender sender in Senders)
                    sender.ProxyCredentials = value;
            }
        }
        public override IWebProxy Proxy {
            get { return base.Proxy; }
            set {
                base.Proxy = value;
                foreach (IExceptionReportSender sender in Senders)
                    sender.Proxy = value;
            }
        }
        public override bool ConfirmSendReport {
            get { return base.ConfirmSendReport; }
            set {
                base.ConfirmSendReport = value;
                foreach (IExceptionReportSender sender in Senders)
                    sender.ConfirmSendReport = value;
            }
        }
        public override int RetryCount {
            get { return base.RetryCount; }
            set {
                base.RetryCount = value;
                foreach (IExceptionReportSender sender in Senders) {
                    ExceptionReportSenderSkeleton instance = sender as ExceptionReportSenderSkeleton;
                    if (instance != null)
                        instance.RetryCount = value;
                }
            }
        }
        /*
        public override string MiniDumpServiceUrl {
            get { return base.MiniDumpServiceUrl; }
            set {
                base.MiniDumpServiceUrl = value;
                foreach (IExceptionReportSender sender in Senders) {
                    ExceptionReportSenderSkeleton instance = sender as ExceptionReportSenderSkeleton;
                    if (instance != null)
                        instance.MiniDumpServiceUrl = value;
                }
            }
        }
        */
        public override int ReportTimeoutMilliseconds {
            get { return base.ReportTimeoutMilliseconds; }
            set {
                base.ReportTimeoutMilliseconds = value;
                foreach (IExceptionReportSender sender in Senders) {
                    ExceptionReportSenderSkeleton instance = sender as ExceptionReportSenderSkeleton;
                    if (instance != null)
                        instance.ReportTimeoutMilliseconds = value;
                }
            }
        }


        public override bool CanSendExceptionReport() {
            int count = Senders.Count;
            for (int i = 0; i < count; i++)
                if (Senders[i].CanSendExceptionReport())
                    return true;
            return false;
        }

        protected override bool SendExceptionReportCore(LogifyClientExceptionReport report) {
            bool result = false;
            int count = Senders.Count;
            for (int i = 0; i < count; i++) {
                try {
                    if (Senders[i].CanSendExceptionReport()) {
                        bool success = Senders[i].SendExceptionReport(report);
                        result = true;
                        if (success && StopWhenFirstSuccess)
                            break;

                    }
                } catch {
                }
            }
            return result;
        }
#if ALLOW_ASYNC
        protected override async Task<bool> SendExceptionReportCoreAsync(LogifyClientExceptionReport report) {
            bool result = false;
            int count = Senders.Count;
            for (int i = 0; i < count; i++) {
                try {
                    if (Senders[i].CanSendExceptionReport()) {
                        bool success = await Senders[i].SendExceptionReportAsync(report);
                        result = true;
                        if (success && StopWhenFirstSuccess)
                            break;

                    }
                } catch {
                }
            }
            return result;
        }
#endif
        public override IExceptionReportSender CreateEmptyClone() {
            return new CompositeExceptionReportSender();
        }
        public override void CopyFrom(IExceptionReportSender instance) {
            base.CopyFrom(instance);


            CompositeExceptionReportSender other = instance as CompositeExceptionReportSender;
            if (other == null)
                return;

            for (int i = 0; i < other.Senders.Count; i++)
                this.Senders.Add(other.Senders[i].Clone());

            this.StopWhenFirstSuccess = other.StopWhenFirstSuccess;
        }
    }

    public class FirstSuccessfullExceptionReportSender : CompositeExceptionReportSender {
        protected override bool SendExceptionReportCore(LogifyClientExceptionReport report) {
            bool result = false;
            int count = Senders.Count;
            for (int i = 0; i < count; i++) {
                try {
                    if (Senders[i].CanSendExceptionReport()) {
                        Senders[i].SendExceptionReport(report);
                        result = true;
                    }
                } catch {
                }
            }
            return result;
        }
    }
}