﻿using System;
using System.IO;

namespace ImagePipelineTest.ImagePipeline.Testing
{
    /// <summary>
    /// Mock MemoryStream class.
    /// </summary>
    class MockMemoryStream : MemoryStream
    {
        private readonly Action<byte[], int, int> _writeFunc;

        /// <summary>
        /// Instantiates the <see cref="MockMemoryStream"/>.
        /// </summary>
        public MockMemoryStream(Action<byte[], int, int> writeFunc)
        {
            _writeFunc = writeFunc;
        }

        /// <summary>
        /// Write delegate.
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _writeFunc(buffer, offset, count);
        }
    }
}
