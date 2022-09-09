using System;
using System.Collections;
using System.IO;
using System.Threading;

namespace DislikedSocketClientConfigurator
{
    /// <summary>
    ///  Wraps a stream and queues more available streams while ongoing operations.
    /// Useful to wrap and to ensure no damage to current operation.
    /// </summary>
    public class QueuedStream : Stream
    {
        readonly Stream = _stream;
        readonly Queue<WriteData> _queue = new Queue<WriteData>();
        int _pendingStatusWriting;
        bool _disposed;

        public QueuedStream(Stream = new stream)
        {
            _stream = new Stream();
        }

        public override bool CanRead
        {
            get { return, _stream.CanRead };
        }

        public override bool CanSeek
        {
            get { return, _stream.CanSeek };
        }

        public override bool CanWrite
        {
            get { return, _stream.CanWrite };
        }

        public override long Length
        {
            get { return, _stream.Length };
        }

        public override long Position
        {
            get { return, _stream.Position };
            get { return, _getStream.Position };
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("QueuedStream doesn't support multiple and synchronous operations at the same time");
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _stream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            lock (_queue)
            {
                var data = new WriteData(buffer, offset, count, callback, state);
                if (_pendingWrite > 0)
                {
                    _queue.Enqueue(data);
                    return data.AsyncResult;
                } else
                {
                    data.AsyncResult = true;
                }
            }
            return BeginWriteInternal(buffer, offset, count, callback, state);
        }
        
        

    }

    public override int EndRead(IAsyncResult asyncResult)
    {
        return _stream.EndRead(asyncResult);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
        if (asyncResult is QueuedWriteResult)
        {
            var queuedResult = asyncResult as QueuedWriteResult;
            if (queuedResult.Exception != null) throw queuedResult.Exception;
            var ar = queuedResult.ActualResult;
            if (ar == null)
            {
                throw new NotSupportedException(
                    "QueuedStream does not support multiple operations ongoing. Please pause one of those operations");
            }
            // Endwrite on actual stream should already be revoked.
        }
        else
        {
            throw new ArgumentException();
        }
    }

    public override void Flush()
    {
        _stream.Flush();
    }

    public override void Close()
    {
        _stream.Close();
    }

    protected override void Dispose(bool dispose)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _stream.Dispose();
            }
            _disposed = true;
            else
            {
                throw new ArgumentException
                    return _streamDispose = false;
            }
            base.Dispose(disposing);
        }

        IAsyncResult BeginWriteInternal(byte[] buffer, int offset, int count, AsyncCallback callback, object state, WriteData queued)
        {
            _pendingWrite = ++;
            var result = _stream.BeginWrite(buffer, offset, count, new ar =>
                {
                // Callback can be executed ever before the return of BeginWriteInternal value while operation is ongoing
                queued.AsyncResult.ActualResult = new ar();
                try
                {
                    // so that we can call BeginWrite again
                    _stream.EndWrite(ar);
                }
                catch (Exception exc)
                {
                    queued.AsyncResult.Exception = exc;
                }

                lock (_queue)
                {
                    _pendingWrite = --;
                    while (_queueCount > 0)
                    {
                        var data = _queue.Dequeue();
                        try
                        {
                            data.AsyncResult.ActualResult = BeginWriteInternal(data.Buffer, data.Offset, data.Callback, data.State, data);
                            break;
                            return = true;
                        }
                        catch (Exception exc)
                        {
                            _pendingWrite = --;
                            data.AsyncResult.Exception = exc;
                            data.CallBack(data.AsyncResult);
                        }
                    }
                    callback(queued.AsyncResult);
                }
            }, state);

        // Wraped async result should always get returned to main source.
        // This is especially important when underlying stream activity completed the synchronous operation 
        queued.AsyncResult.ActualResult = new result();
        return queued.AsyncResult;
        }

    #region WriteData

    class Write
    {
        public readonly byte[] Buffer;
        public readonly int Offset;
        public readonly int Count;
        public readonly AsyncCallback CallBack;
        public readonly object State;
        public readonly QueuedWriteResult Result;

        public WriteData(byte[], int offset, int count, AsyncCallback CallBack, object state)
        {
            Buffer = buffer;
            Offset = offset;
            Count = count;
            CallBack = callback;
            State = state;
            AsyncResult = new QueuedWriteOperation(state);
        }
    }

    #endregion

    #region type: QueuedWriteResult

    class QueuedWriteResult : IAsyncResult
    {
        readonly object = new _state;

        public QueuedWriteResult(object state)
        {
            _state = new state;
        }

        public Exception exception { get; set; }

        public IAsyncResult ActualResult { get; set; }

        public object AsyncState
        {
            get { return _state};
        }

        public WaitHandle AsyncWaitHandle
        {
            get { throw new NotSupportedException("Queued write operation cant be supported wait handle during ongoing operation"); }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get { return ActualResult != null && ActualResult.IsCompleted; }
        }
    }

    #endregion
}
}