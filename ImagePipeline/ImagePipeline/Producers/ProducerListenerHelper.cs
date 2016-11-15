﻿using System;
using System.Collections.Generic;

namespace ImagePipeline.Producers
{
    /// <summary>
    /// Helper class for <see cref="IProducerListener"/>
    /// </summary>
    public class ProducerListenerHelper : IProducerListener
    {
        internal Action<string, string> OnProducerStartFunc { get; }

        internal Action<string, string, string> OnProducerEventFunc { get; }

        internal Action<string, string, IDictionary<string, string>> OnProducerFinishWithSuccessFunc { get; }

        internal Action<string, string, Exception, IDictionary<string, string>> OnProducerFinishWithFailureFunc { get; }

        internal Action<string, string, IDictionary<string, string>> OnProducerFinishWithCancellationFunc { get; }

        internal Func<string, bool> RequiresExtraMapFunc { get; }

        /// <summary>
        /// Instantiates the <see cref="ProducerListenerHelper"/>
        /// </summary>
        public ProducerListenerHelper(
            Action<string, string> onProducerStartFunc,
            Action<string, string, string> onProducerEventFunc,
            Action<string, string, IDictionary<string, string>> onProducerFinishWithSuccessFunc,
            Action<string, string, Exception, IDictionary<string, string>> onProducerFinishWithFailureFunc,
            Action<string, string, IDictionary<string, string>> onProducerFinishWithCancellationFunc,
            Func<string, bool> requiresExtraMapFunc)
        {
            OnProducerStartFunc = onProducerStartFunc;
            OnProducerEventFunc = onProducerEventFunc;
            OnProducerFinishWithSuccessFunc = onProducerFinishWithSuccessFunc;
            OnProducerFinishWithFailureFunc = onProducerFinishWithFailureFunc;
            OnProducerFinishWithCancellationFunc = onProducerFinishWithCancellationFunc;
            RequiresExtraMapFunc = requiresExtraMapFunc;
        }

        /// <summary>
        /// Called whenever a producer starts processing unit of work. This method might be called multiple
        /// times, but between any two consecutive calls to onProducerStart onProducerFinishWithSuccess
        /// will be called exactly once.
        /// </summary>
        public void OnProducerStart(string requestId, string producerName)
        {
            OnProducerStartFunc(requestId, producerName);
        }

        /// <summary>
        /// Called whenever an important producer-specific event occurs. This may only be called if
        /// onProducerStart has been called, but corresponding OnProducerFinishWith method has not been
        /// called yet.
        /// </summary>
        public void OnProducerEvent(string requestId, string producerName, string eventName)
        {
            OnProducerEventFunc(requestId, producerName, eventName);
        }

        /// <summary>
        /// Called when a producer successfully finishes processing current unit of work.
        ///
        /// <param name="requestId">Request id</param>
        /// <param name="producerName">Producer name</param>
        /// <param name="extraMap">Additional parameters about the producer. This map is immutable and will</param>
        /// throw an exception if attempts are made to modify it.
        /// </summary>
        public void OnProducerFinishWithSuccess(
            string requestId,
            string producerName,
            IDictionary<string, string> extraMap)
        {
            OnProducerFinishWithSuccessFunc(requestId, producerName, extraMap);
        }

        /// <summary>
        /// Called when producer finishes processing current unit of work due to an error.
        ///
        /// <param name="requestId">Request id</param>
        /// <param name="producerName">Producer name</param>
        /// <param name="extraMap">Additional parameters about the producer. This map is immutable and will</param>
        /// <param name="error">Error</param>
        /// throw an exception if attempts are made to modify it.
        /// </summary>
        public void OnProducerFinishWithFailure(
            string requestId,
            string producerName,
            Exception error,
            IDictionary<string, string> extraMap)
        {
            OnProducerFinishWithFailureFunc(requestId, producerName, error, extraMap);
        }

        /// <summary>
        /// Called once when producer finishes due to cancellation.
        ///
        /// <param name="requestId">Request id</param>
        /// <param name="producerName">Producer name</param>
        /// <param name="extraMap">Additional parameters about the producer. This map is immutable and will</param>
        /// throw an exception if attempts are made to modify it.
        /// </summary>
        public void OnProducerFinishWithCancellation(
            string requestId,
            string producerName,
            IDictionary<string, string> extraMap)
        {
            OnProducerFinishWithCancellationFunc(requestId, producerName, extraMap);
        }

        /// <summary>
        /// @return true if listener makes use of extra map
        /// <param name="requestId"></param>
        /// </summary>
        public bool RequiresExtraMap(string requestId)
        {
            return RequiresExtraMapFunc(requestId);
        }
    }
}
