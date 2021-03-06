﻿using System;
using System.Collections.Generic;

namespace Exceptionless.Core.Pipeline {
    public interface IPipelineAction<TContext> where TContext : IPipelineContext {
        /// <summary>
        /// Processes this action using the specified pipeline context.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        void Process(TContext context);

        /// <summary>
        /// Processes this action using the specified pipeline context.
        /// </summary>
        /// <param name="contexts">The pipeline context.</param>
        void ProcessBatch(ICollection<TContext> contexts);

        /// <summary>
        /// Handle exceptions thrown by this action.
        /// </summary>
        /// <param name="exception">The exception that occurred while processing the action.</param>
        /// <param name="context">The pipeline context.</param>
        /// <returns>Return true if processing should continue or false if processing should halt.</returns>
        bool HandleError(Exception exception, TContext context);
    }

    /// <summary>
    /// The base class for pipeline actions
    /// </summary>
    /// <typeparam name="TContext">The type of the pipeline context.</typeparam>
    public abstract class PipelineActionBase<TContext> : IPipelineAction<TContext> where TContext : class, IPipelineContext {
        protected virtual bool ContinueOnError { get { return false; } }

        /// <summary>
        /// Processes this action using the specified pipeline context.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        public abstract void Process(TContext context);

        /// <summary>
        /// Processes this action using the specified pipeline context.
        /// </summary>
        /// <param name="contexts">The pipeline context.</param>
        public virtual void ProcessBatch(ICollection<TContext> contexts) {
            foreach (var ctx in contexts) {
                try {
                    Process(ctx);
                } catch (Exception ex) {
                    bool cont = false;
                    try {
                        cont = HandleError(ex, ctx);
                    } catch { }

                    if (!cont)
                        ctx.SetError(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Handle exceptions thrown by this action.
        /// </summary>
        /// <param name="exception">The exception that occurred while processing the action.</param>
        /// <param name="context">The pipeline context.</param>
        /// <returns>Return true if processing should continue or false if processing should halt.</returns>
        public virtual bool HandleError(Exception exception, TContext context) {
            return ContinueOnError;
        }
    }
}