﻿#if DEBUG
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace DevExpress.Logify.Core.Internal.Tests {
    [TestFixture]
    public class AttachmentsCollectorTests : CollectorTestsBase {
        AttachmentsCollector collector;
        AttachmentCollection attachments;
        AttachmentCollection additionalAttachments;
        [SetUp]
        public void Setup() {
            this.attachments = new AttachmentCollection();
            this.additionalAttachments = new AttachmentCollection();
            this.collector = new AttachmentsCollector(attachments, additionalAttachments);
            SetupCore();

        }
        [TearDown]
        public void TearDown() {
            TearDownCore();
        }
        [Test]
        public void Default() {
            collector.Process(null, Logger);
            string expected = "";
            Assert.AreEqual(expected, Content);
        }
        [Test]
        public void EmptyAttachContent() {
            attachments.Add(new Attachment() { Name = "test", MimeType = "image/png" });
            this.collector = new AttachmentsCollector(attachments, additionalAttachments);
            collector.Process(null, Logger);
            string expected = "\"attachments\":[]\r\n";
            Assert.AreEqual(expected, Content);
        }
        [Test]
        public void EmptyAttachName() {
            attachments.Add(new Attachment() { MimeType = "image/png", Content = new byte[] { 0, 1, 2 } });
            this.collector = new AttachmentsCollector(attachments, additionalAttachments);
            collector.Process(null, Logger);
            string expected = "\"attachments\":[]\r\n";
            Assert.AreEqual(expected, Content);
        }
        [Test]
        public void SimpleAttach() {
            attachments.Add(new Attachment() { Name = "img", MimeType = "image/png", Content = new byte[] { 0, 1, 2 } });
            this.collector = new AttachmentsCollector(attachments, additionalAttachments);
            collector.Process(null, Logger);
            string expected = "\"attachments\":[\r\n{\r\n\"name\":\"img\",\r\n\"mimeType\":\"image/png\",\r\n\"content\":\"H4sIAAAAAAAEAGNgZAIAf4lUCAMAAAA=\",\r\n\"compress\":\"gzip\"\r\n}\r\n]\r\n";
            Assert.AreEqual(expected, Content);
        }
    }
}
#endif