// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#if WINDOWS10_0_17763_0
using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement.Core;

namespace Microsoft.AppCenter.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        public event EventHandler ApplicationSuspended;
        public event EventHandler ApplicationResuming;
        public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;

        // True if InvokeResuming has been called at least once during the current process
        private static bool _isAppStarted;
        
        // Considered to be suspended until can verify that has started
        private static bool _suspended = true;

        // Singleton instance of ApplicationLifecycleHelper
        private static ApplicationLifecycleHelper _instance;
        public static ApplicationLifecycleHelper Instance
        {
            get { return _instance ?? (_instance = new ApplicationLifecycleHelper()); }

            // Setter for testing
            internal set { _instance = value; }
        }

        /// <summary>
        /// Indicates whether the application is currently in a suspended state. 
        /// </summary>
        public bool IsSuspended => _suspended;

        public ApplicationLifecycleHelper()
        {

            // Subscribe to Resuming and Suspending events.
            CoreApplication.Suspending += InvokeSuspended;
            CoreInputView.GetForCurrentView().OcclusionsChanged += V_OcclusionsChanged;

            // Subscribe to unhandled errors events.
            CoreApplication.UnhandledErrorDetected += (sender, eventArgs) =>
            {
                try
                {
                    // Intentionally propagate exception to get the exception object that crashed the app.
                    eventArgs.UnhandledError.Propagate();
                }
                catch (Exception exception)
                {
                    InvokeUnhandledExceptionOccurred(sender, exception);

                    // Since UnhandledError.Propagate marks the error as Handled, rethrow in order to only Log and not Handle.
                    // Use ExceptionDispatchInfo to avoid changing the stack-trace.
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
            };
        }

        private void V_OcclusionsChanged(CoreInputView sender, CoreInputViewOcclusionsChangedEventArgs args)
        {
            if (_isAppStarted)
            {
                InvokeSuspended(null, EventArgs.Empty);
            } 
            else
            {
                InvokeResuming(null, EventArgs.Empty);
            }
        }

        internal void InvokeUnhandledExceptionOccurred(object sender, Exception exception)
        {
            UnhandledExceptionOccurred?.Invoke(sender, new UnhandledExceptionOccurredEventArgs(exception));
        }

        private void InvokeResuming(object sender, object e)
        {
            _isAppStarted = true;
            _suspended = false;
            ApplicationResuming?.Invoke(sender, EventArgs.Empty);
        }

        private void InvokeSuspended(object sender, object e)
        {
            _suspended = true;
            _isAppStarted = false;
            ApplicationSuspended?.Invoke(sender, EventArgs.Empty);
        }
    }
}
#endif
