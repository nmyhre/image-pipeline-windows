﻿using Cache.Common;
using ImagePipeline.Bitmaps;
using ImagePipeline.Cache;
using ImagePipeline.Decoder;
using ImagePipeline.Image;
using ImagePipeline.Memory;
using ImagePipeline.Producers;

namespace ImagePipeline.Core
{
    /// <summary>
    /// Producer factory
    /// </summary>
    public class ProducerFactory
    {
        // Decode dependencies
        private readonly IByteArrayPool _byteArrayPool;
        private readonly ImageDecoder _imageDecoder;
        private readonly IProgressiveJpegConfig _progressiveJpegConfig;
        private readonly bool _downsampleEnabled;
        private readonly bool _resizeAndRotateEnabledForNetwork;
        private readonly bool _decodeFileDescriptorEnabled;

        // Dependencies used by multiple steps
        private readonly IExecutorSupplier _executorSupplier;
        private readonly IPooledByteBufferFactory _pooledByteBufferFactory;

        // Cache dependencies
        private readonly BufferedDiskCache _defaultBufferedDiskCache;
        private readonly BufferedDiskCache _smallImageBufferedDiskCache;
        private readonly IMemoryCache<ICacheKey, IPooledByteBuffer> _encodedMemoryCache;
        private readonly IMemoryCache<ICacheKey, CloseableImage> _bitmapMemoryCache;
        private readonly ICacheKeyFactory _cacheKeyFactory;
        private readonly int _forceSmallCacheThresholdBytes;

        // Postproc dependencies
        private readonly PlatformBitmapFactory _platformBitmapFactory;

        /// <summary>
        /// Instantiates the <see cref="ProducerFactory"/>
        /// </summary>
        /// <param name="byteArrayPool">The IByteArrayPool used by DecodeProducer.</param>
        /// <param name="imageDecoder">The image decoder.</param>
        /// <param name="progressiveJpegConfig">The progressive Jpeg configuration.</param>
        /// <param name="downsampleEnabled">Enabling downsample.</param>
        /// <param name="resizeAndRotateEnabledForNetwork">Enabling resize and rotate.</param>
        /// <param name="executorSupplier">The supplier for tasks.</param>
        /// <param name="pooledByteBufferFactory">The factory that allocates IPooledByteBuffer memory.</param>
        /// <param name="bitmapMemoryCache">The memory cache for CloseableImage.</param>
        /// <param name="encodedMemoryCache">The memory cache for IPooledByteBuffer.</param>
        /// <param name="defaultBufferedDiskCache">The default buffered disk cache.</param>
        /// <param name="smallImageBufferedDiskCache">The buffered disk cache used for small images.</param>
        /// <param name="cacheKeyFactory">The factory that creates cache keys for the pipeline.</param>
        /// <param name="platformBitmapFactory">The bitmap factory used for post process.</param>
        /// <param name="decodeFileDescriptorEnabled">Enabling the file descriptor.</param>
        /// <param name="forceSmallCacheThresholdBytes">The threshold set for using the small buffered disk cache.</param>
        public ProducerFactory(
            IByteArrayPool byteArrayPool,
            ImageDecoder imageDecoder,
            IProgressiveJpegConfig progressiveJpegConfig,
            bool downsampleEnabled,
            bool resizeAndRotateEnabledForNetwork,
            IExecutorSupplier executorSupplier,
            IPooledByteBufferFactory pooledByteBufferFactory,
            IMemoryCache<ICacheKey, CloseableImage> bitmapMemoryCache,
            IMemoryCache<ICacheKey, IPooledByteBuffer> encodedMemoryCache,
            BufferedDiskCache defaultBufferedDiskCache,
            BufferedDiskCache smallImageBufferedDiskCache,
            ICacheKeyFactory cacheKeyFactory,
            PlatformBitmapFactory platformBitmapFactory,
            bool decodeFileDescriptorEnabled,
            int forceSmallCacheThresholdBytes)
        {
            _forceSmallCacheThresholdBytes = forceSmallCacheThresholdBytes;

            _byteArrayPool = byteArrayPool;
            _imageDecoder = imageDecoder;
            _progressiveJpegConfig = progressiveJpegConfig;
            _downsampleEnabled = downsampleEnabled;
            _resizeAndRotateEnabledForNetwork = resizeAndRotateEnabledForNetwork;

            _executorSupplier = executorSupplier;
            _pooledByteBufferFactory = pooledByteBufferFactory;

            _bitmapMemoryCache = bitmapMemoryCache;
            _encodedMemoryCache = encodedMemoryCache;
            _defaultBufferedDiskCache = defaultBufferedDiskCache;
            _smallImageBufferedDiskCache = smallImageBufferedDiskCache;
            _cacheKeyFactory = cacheKeyFactory;

            _platformBitmapFactory = platformBitmapFactory;

            _decodeFileDescriptorEnabled = decodeFileDescriptorEnabled;
        }

        /// <summary>
        /// Instantiates the <see cref="AddImageTransformMetaDataProducer"/>
        /// </summary>
        /// <param name="inputProducer"></param>
        /// <returns></returns>
        public static AddImageTransformMetaDataProducer NewAddImageTransformMetaDataProducer(
            IProducer<EncodedImage> inputProducer)
        {
            return new AddImageTransformMetaDataProducer(inputProducer);
        }
    }
}
