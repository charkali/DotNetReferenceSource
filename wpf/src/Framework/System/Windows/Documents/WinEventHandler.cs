//---------------------------------------------------------------------------
//
// <copyright file=WinEventHandler.cs company=Microsoft>
//    Copyright (C) Microsoft Corporation.  All rights reserved.
// </copyright>
// 
//
// Description: WinEventHandler implementation.
//
// History:
//  02/04/2005 : yutakas - created.
//
//---------------------------------------------------------------------------

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Windows.Threading;
using MS.Win32;

using MS.Internal;

namespace System.Windows.Documents
{
    internal class WinEventHandler : IDisposable
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------
 
        #region Constructors

        // ctor that takes a range of events
        /// <SecurityNote>
        /// Critical - as this calls the setter for _winEventProc.Value.
        /// Safe - as this doesn't allow an arbitrary value to be set.
        /// </SecurityNote>
        [SecurityCritical, SecurityTreatAsSafe]
        internal WinEventHandler(int eventMin, int eventMax)
        {
            _eventMin = eventMin;
            _eventMax = eventMax;

            _winEventProc.Value = new NativeMethods.WinEventProcDef(WinEventDefaultProc);
            // Keep the garbage collector from moving things around
            _gchThis = GCHandle.Alloc(_winEventProc.Value);

            // Workaround for bug 150666.
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            if (dispatcher != null)
            {
                dispatcher.ShutdownFinished += new EventHandler(OnDispatcherShutDown);
            }
        }

        ~WinEventHandler()
        {
            Clear();
        }

        #endregion Constructors

        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------
 
        #region Internal Methods

        public void Dispose()
        {
            // no need to call this finalizer.

            GC.SuppressFinalize(this);
            Clear();
        }

        // callback for the class inherited from this.
        internal virtual void WinEventProc(int eventId, IntPtr hwnd)
        {
        }

        /// <SecurityNote>
        ///     Critical: This code frees the gchandle and is critical because it calls GCHandle
        ///     TreatAsSafe: Ok to call free on a handle that we have allocated
        /// </SecurityNote>
        [SecurityCritical,SecurityTreatAsSafe]
        internal void Clear()
        {
            // Make sure that the hooks is uninitialzied.
            Stop();

            // free GC handle.
            if (_gchThis.IsAllocated)
            {
                _gchThis.Free();
            }
        }

        // install WinEvent hook and start getting the callback.
        /// <SecurityNote>
        /// Critical - as this calls SetWinEventHook which has a SUC on it and also calls
        ///            the setter for _hHook.
        /// Safe - as this does not allow an arbitrary method to be set up as a hook and
        ///        also doesn't allow an aribtrary value to be set on _hHook.Value.
        /// </SecurityNote>
        [SecurityCritical, SecurityTreatAsSafe]
        internal void Start()
        {
            if (_gchThis.IsAllocated)
            {
                _hHook.Value = UnsafeNativeMethods.SetWinEventHook(_eventMin, _eventMax, IntPtr.Zero, _winEventProc.Value,
                                                             0, 0, NativeMethods.WINEVENT_OUTOFCONTEXT);
                if (_hHook.Value == IntPtr.Zero )
                {
                    Stop();
                }
            }
        }

        // uninstall WinEvent hook.
        /// <SecurityNote>
        /// Critical - as this calls UnhookWinEvent which has a SUC on it and also
        ///            calls the setter for _hHook.Value.
        /// Safe - as this does not allow an arbitrary hook to be removed and sets
        ///        _hHook.Value to IntPtr.Zero which is safe.
        /// </SecurityNote>
        [SecurityCritical, SecurityTreatAsSafe]
        internal void Stop()
        {
            if (_hHook.Value != IntPtr.Zero )
            {
                UnsafeNativeMethods.UnhookWinEvent(_hHook.Value);
                _hHook.Value = IntPtr.Zero ;
            }
        }

        #endregion Internal Methods

        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------
 
        #region Private Methods

        private void WinEventDefaultProc(int winEventHook, int eventId, IntPtr hwnd, int idObject, int idChild, int eventThread, int eventTime)
        {
            WinEventProc(eventId , hwnd);
        }

        // Workaround for bug 150666.
        private void OnDispatcherShutDown(object sender, EventArgs args)
        {
            Stop();
        }

        #endregion Private Methods


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------
 
        #region Private Fields

        // min WinEvent.
        private int _eventMin;

        // max WinEvent.
        private int _eventMax;

        // hook handle
        private SecurityCriticalDataForSet<IntPtr> _hHook;

        // the callback.
        private SecurityCriticalDataForSet<NativeMethods.WinEventProcDef> _winEventProc;

        // GCHandle to keep the garbage collector from moving things around
        private GCHandle _gchThis;

        #endregion Private Fields
    }
}
