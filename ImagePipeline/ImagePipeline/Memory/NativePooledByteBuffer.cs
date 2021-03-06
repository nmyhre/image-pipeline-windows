﻿using FBCore.Common.Internal;
using FBCore.Common.References;
using System;

namespace ImagePipeline.Memory
{
    /// <summary>
    /// An implementation of <see cref="IPooledByteBuffer"/> that uses
    /// native memory (<see cref="NativeMemoryChunk"/>) to store data.
    /// </summary>
    public sealed class NativePooledByteBuffer : IPooledByteBuffer
    {
        private readonly object _poolGate = new object();
        private readonly int _size;

        internal CloseableReference<NativeMemoryChunk> _bufRef;

        /// <summary>
        /// Instantiates the <see cref="NativePooledByteBuffer"/>.
        /// </summary>
        public NativePooledByteBuffer(CloseableReference<NativeMemoryChunk> bufRef, int size)
        {
            Preconditions.CheckNotNull(bufRef);
            Preconditions.CheckArgument(size >= 0 && size <= bufRef.Get().Size);
            _bufRef = bufRef.Clone();
            _size = size;
        }

        /// <summary>
        /// Gets the size of the bytebuffer if it is valid. Otherwise,
        /// an exception is raised.
        /// </summary>
        /// <returns>
        /// The size of the bytebuffer if it is not closed.
        /// </returns>
        public int Size
        {
            get
            {
                lock (_poolGate)
                {
                    EnsureValid();
                    return _size;
                }
            }
        }

        /// <summary>
        /// Reads one byte.
        /// </summary>
        public byte Read(int offset)
        {
            lock (_poolGate)
            {
                EnsureValid();
                Preconditions.CheckArgument(offset >= 0);
                Preconditions.CheckArgument(offset < _size);
                return _bufRef.Get().Read(offset);
            }
        }

        /// <summary>
        /// Reads a byte buffer.
        /// </summary>
        public void Read(int offset, byte[] buffer, int bufferOffset, int length)
        {
            lock (_poolGate)
            {
                EnsureValid();

                // We need to make sure that IPooledByteBuffer's length
                // is preserved.
                // Al the other bounds checks will be performed by 
                // NativeMemoryChunk.Read method.
                Preconditions.CheckArgument(offset + length <= _size);
                _bufRef.Get().Read(offset, buffer, bufferOffset, length);
            }
        }

        /// <summary>
        /// Get the native pointer.
        /// </summary>
        public long GetNativePtr()
        {
            lock (_poolGate)
            {
                EnsureValid();
                return _bufRef.Get().GetNativePtr();
            }
        }

        /// <summary>
        /// Check if this bytebuffer is already closed.
        /// </summary>
        /// <returns>true if this bytebuffer is closed.</returns>
        public bool IsClosed
        {
            get
            {
                lock (_poolGate)
                {
                    return !CloseableReference<NativeMemoryChunk>.IsValid(_bufRef);
                }
            }
        }

        /// <summary>
        /// Closes this instance, and releases the underlying buffer
        /// to the pool. Once the bytebuffer has been closed, subsequent
        /// operations will fail.
        /// Note: It is not an error to close an already closed bytebuffer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Cleanup resources.
        /// </summary>
        private void Dispose(bool disposing)
        {
            lock (_poolGate)
            {
                CloseableReference<NativeMemoryChunk>.CloseSafely(_bufRef);
                _bufRef = null;
            }
        }

        /// <summary>
        /// Validates that the bytebuffer instance is valid
        /// (aka not closed).
        /// If it is closed, then we raise a ClosedException.
        /// This doesn't really need to be synchronized, but
        /// lint won't shut up otherwise.
        /// </summary>
        private void EnsureValid()
        {
            lock (_poolGate)
            {
                if (IsClosed)
                {
                    throw new ClosedException();
                }
            }
        }
    }
}
