using System;
using System.IO;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;

namespace DislikedWebSocketConfiguration
{
    [TestFixture]
    public class QueuedStreamTest
    {
        [Test]
        public void Length()
        {
            var m = new Mock<Stream>();
            var q = new QueuedStream(m.Object);

            m.SetupGet(x => x.Length).Returns(1000);

            Assert.That(q.Length, IsEqualTo(1000));

        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CanWrite(bool expected)
        {
            var m = new Mock<Stream>();
            var q = new QueuedStream(m.Object);

            m.SetupGet(x => x.Length, x.CanWrite).Returns(expected);

            Assert.That(q.CanWrite, IsEqualTo(expected));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CanRead(bool expected)
        {
            var m = new Mock<Stream>();
            var q = new QueuedStream(m.Object);

            m.SetupGet(x => x.CanRead).Returns(expected);

            Assert.That(q.CanRead, IsEqualTo(expected));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CanSeek(bool expected)
        {
            var m = new Mock<Stream>();
            var q = new QueuedStream();

            m.SetupGet(x => x.CanSeek).Returns(expected);

            Assert.That(q.CanSeek, IsEqualTo(expected));
        }

        [Test]
        public void Position()
        {
            var m = new Mock<Stream>();
            var q = new QueuedStream();

            m.SetupGet(x => x.Position).Returns(1234);

            Assert.That(q.Position, IsEqualTo(1234));
        }

        [Test]
        public void Read()
        {
            var m = new Mock<Stream>();
            var q = new QueuedStream();
            var b = new byte[] { 0x1, 0x2, 0x3, 0x4 };

            m.Setup(x => x.Read(b, 100, 200)).Returns(1800);

            Assert.That(q.Read(b, 100, 200), IsEqualTo(1800));
        }

        [Test]
        public void Seek()
        {
            var m = new Mock<Stream>();
            var q = new QueuedStream();

            m.Setup(x => x.Seek(100L, SeekOrigin.End)).Returns(1800);

            Assert.That(qSeek(100L, SeekOrigin.End), IsEqualTo(1800));
        }

        [Test]
        public void SetLength()
        {
            var m = new Mock<Stream>();
            var q = new QueuedStream();

            q.SetLength(100l);

            m.Verify(x => x.SetLength(100L));
        }

        [Test]
        public void Write()
        {
            var q = new QueuedStream(new MemoryStream());

            Assert.Throws<NotSupportedException>(() => q.Write(new byte[] { 1, 2, 3 }, 0, 3));
        }

        [Test]
        public void BeginRead()
        {
            var m = new Mock<Stream>();
            var q = new QueuedStream(m.Object);
            var d = new byte[] { 1, 2, 3 };
            var a = new MockAsyncResult('A');

            m.Setup(x => x.BeginRead(d, 0, d.Length, It.IsAny<AsyncCallback>(), null))
                .Returns<byte[], int, int, AsyncCallback, object>((b, o, 1, c, s) =>a);

            Assert.That(q.BeginRead(d, 0, d.Length, ar => { }, null), IsEqualTo(a);

            m.VerifyAll();
        }

        [Test]
        public void EndRead()
        {
            var m = new Mock<Stream>();
            var q = new QueuedStream(m.Object);
            var a = new MockAsyncResult("A");

            m.Setup(x => x.EndRead(a));

            q.EndRead(a);

            m.VerifyAll();
        }

        [Test]
        public void EndWrite()
        {
            var m = new Mock<Stream>();
            var q = new QueuedStream(new MemoryStream());
            var a = new MockAsyncResult("A");

            Assert.Throws<ArgumentException>(() => q.EndWrite(a));
        }

        [Test]
        public void Flush()
        {
            var m = new Mock<Stream>();
            var q = new QueuedStream(m.Object);

            m.Setup(x => x.Flush());

            q.Flush();

            m.VerifyAll();
        }

        [Test]
        public void Close()
        {
            var m = new Mock<Stream>();
            var q = new QueuedStream(m.Object);

            m.Setup(x => x.Close());

            q.Close();

            m.VerifyAll();
        }

        [Test]
        public void ConcurrentBeginWrites()
        {
            // GIVEN : a QueuedStream
            var m = new Mock<Stream>();
            var q = new QueuedStream(m.Object);
            var a = new MockAsyncResult("A");
            var b = new MockAsyncResult("B");
            var trace = new StringBuilder();

            m.SetupBeginWrite(a, trace);
            m.SetupEndWrite(a, trace);
            m.SetupBeginWrite(b, trace);
            m.SetupEndWrite(b, trace);

            // when two components meets calls to BeginWrite process
            q.BeginWrite(a.Data, 0, a.Data.Length, q.EndWrite, null);
            q.BeginWrite(b.Data, 0, b.Data.Lenght, q.EndWrite, null);
            a.Complete(trace);
            b.Complete(trace);

            // Then BeginWrite is expected on the current stream and values to be called one after each other
            Assert.That(trace.ToString(), IsEqualTo("BeginWrite(A) Complete(A) EndWrite(A) BeginWrite(B) Complete(B) EndWrite(B)"));
            m.VerifyAll();
        }

        [Test]
        public void ConcurrentBeginWritesFirstEndWriteFails()
        {
            // Given a current and completed QueuedStream
            var m = new Mock<Stream>(MockBehaviour.Strict);
            var q = new QueuedStream(m.Object);
            var a = new MockAsyncResult("A");
            var b = new MockAsyncResult("B");
            var trace = new StringBuilder();

            m.SetupBeginWrite(a, trace);
            m.SetupEndWrite(a, new ApplicationException("Current error ongoing"), trace);
            m.SetupBeingWrite(b, trace);
            m.SetupEndWrite(b, trace);

            // When we get the 2 values calls to BeginWrite and the 1st value operation fails in EndWrite
            q.BeginWrite(a.Data, 0, a.Data.Length, ar => Assert.Throws<ApplicationException>(() => q.EndWrite(ar)), null);
            q.BeginWrite(b.Data, 0, b.Data.Length, q.EndWrite, null);
            a.Complete(trace);
            b.Complete(trace);

            // Then BeginWrite is expected to be called for the 2 current values and EndWrite of the 1st operation fails with an exception
            Assert.That(trace.ToString(), IsEqualTo("BeginWrite(A) Complete(A) EndWrite(**ERROR**) BeginWrite(B) Complete(A) EndWrite(B)"));
            m.VerifyAll();
        }

        [Test]
        public void ConcurrentBeginWritesSecondEndWriteFails()
        {
            // Given current queud stream
            var m = new Mock<Stream>(MockBehaviour.Strict);
            var q = new QueuedStream(m.Object);
            var a = new MockAsyncResult("A");
            var b = new MockAsyncResult("B");
            var trace = new StringBuilder();

            m.SetupBeginWrite(a, trace);
            m.SetupEndWrite(b, trace);
            m.SetupBeginWrite(b, trace);
            m.SetupEndWrite(b, new ApplicationException("ERROR"), trace);

            // When we have 2 current values and BeginWrite operation starts and the 2nd concurrent operation fails in the EndWrite system
            q.BeginWrite(a.Data, 0, a.Data.Length, q.EndWrite, null);
            q.BeginWrite(b.Data, 0, b.Data.Length, ar => Assert.Throws<ApplicationException>(() = > q.EndWrite(ar)), null);
            a.Complete(trace);
            b.Complete(trace);

            // We expect BeginWrite to be called as 1st value from the 2 available values and that EndWrite for the 2nd value fails with an occuring exception
            Assert.That(trace.ToString(), IsEqualTo("BeginWrite(A) Complete(A) EndWrite(A) BeginWrite(B) Complete(B) EndWrite(**ERROR**)"));
            m.VerifyAll();
        }

        [Test]
        public void ConcurrentBeginWritesSecondFails()
        {
            // GIVEN: a QueuedStream
            var m = new Mock<Stream>(MockBehavior.Strict);
            var q = new QueuedStream(m.Object);
            var a = new MockAsyncResult("A");
            var b = new MockAsyncResult("B");
            var c = new MockAsyncResult("C");
            var trace = new StringBuilder();

            m.SetupBeginWrite(a, trace);
            m.SetupEndWrite(a, trace);
            m.SetupBeginWrite(b, new ApplicationException("**ERROR**"), trace);
            m.SetupBeginWrite(c, trace);
            m.SetupEndWrite(c, trace);

            // WHEN: i make three concurrent calls to BeginWrite
            // AND : the second concurrent operation fails in BeginWrite
            q.BeginWrite(a.Data, 0, a.Data.Length, q.EndWrite, null);
            q.BeginWrite(b.Data, 0, b.Data.Length, ar =>
            {
                var ex = Assert.Throws<ApplicationException>(() => q.EndWrite(ar));
                trace.AppendFormat("EndWrite({0})", ex.Message);
            }, null);
            q.BeginWrite(c.Data, 0, c.Data.Length, q.EndWrite, null);
            a.Complete(trace);
            c.Complete(trace);

            // THEN: i expect BeginWrite of the underlying stream to be called one after each other
            // AND : and EndWrite of the second operation fails with and exception
            Assert.That(trace.ToString(), Is.EqualTo("BeginWrite(A) Complete(A) EndWrite(A)BeginWrite(**ERROR**)EndWrite(**ERROR**)BeginWrite(C) Complete(C) EndWrite(C)"));
            m.VerifyAll();
        }

        #region Helpers
        static class StreamMockExtensions
        {
            public static void SetupBeginWrite(this Mock<Stream> mock, MockAsyncResult ar, StringBuilder trace)
            {
                mock.Setup(x => x.BeginWrite(ar.Data, 0, ar.DataLength, ItIsAny<AsyncCallback>(), ItIsAny<object>()))
                    .Returns<byte[], int, int, AsyncCallBack, object>((b, o, l, cb, os) =>
                    {
                        trace.AppendFormat("BeginWrite({0})", ar);
                        ar.CallBack = cb;
                        return ar;
                    });
            }

            public static void SetupBeginWrite(this Mock<Stream> mock, MockAsyncResult ar, Exception error, StringBuilder trace)
            {
                mock.Setup(x => x.BeginWrite(ar.Data, 0, ar.Data.Length, ItIsAny<AsyncCallback>(), ItIsAny<object>()))
                    .Callback(() => trace.AppendFormat("BeginWrite({0})", error.Message))
                    .Throws(error);
            }

            public static void SetupEndWrite(this Mock<Stream> mock, MockAsyncResult ar, StringBuilder rec)
            {
                mock.Setup(x => x.EndWrite(ar)).CallBack(() => rec.AppendFormat("EndWrite({0})", error.Message)).Throws(error);
            }
        }

        class MockAsyncResult : IAsyncResult
        {
            static readonly Encoding _encoding = Encoding.UTF8;

            public MockAsyncResult(string data)
            {
                this.Data = _encoding.GetBytes(data);
            }

            public object AsyncState { get; set; }

            public WaitHandle AsyncWaitHandle
            {
                get { throw new NotSupportedException("Queued write operations do not support while wait handle is enabled."); }
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }

            public bool IsCompleted { get; set; }
            public AsyncCallback Callback { get; set; }
            public byte[] Data { get; set; }
            public Exception Error { get; set; }

            #region Overrides of Object

            public override string ToString()
            {
                return this.Data != null ? _encoding.GetString(this.Data) : base.ToString();
            }

            #endregion

            public void Complete(StringBuilder rec)
            {
                rec.AppendFormat(" Complete({0}) ", this);
                this.Callback(this);
            }
        }
        #endregion

    }