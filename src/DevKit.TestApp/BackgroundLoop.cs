//----------------------------------------------------------------------- 
// ETP DevKit, 1.2
//
// Copyright 2019 Energistics
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Energistics.Etp
{
    public class BackgroundLoop : IDisposable
    {
        private CancellationTokenSource _loopCancellationTokenSource;
        private Task _loopTask;
        private bool disposedValue;

        public bool IsStarted { get; private set; }

        public Action<CancellationToken> Action { get; private set; }

        public TimeSpan Interval { get; private set; }

        public void Start(Action<CancellationToken> action, TimeSpan interval)
        {
            if (IsStarted)
                return;

            IsStarted = true;

            Action = action;
            Interval = interval;

            _loopCancellationTokenSource = new CancellationTokenSource();
            _loopTask = Task.Factory.StartNew(
                async () => await BackgroundLoopBody(_loopCancellationTokenSource.Token), _loopCancellationTokenSource.Token,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default).Unwrap();
        }

        public void Stop()
        {
            if (!IsStarted)
                return;

            _loopCancellationTokenSource.Cancel();
            try
            {
                _loopTask.Wait();
            }
            catch (OperationCanceledException)
            {
            }
            _loopTask = null;
            _loopCancellationTokenSource.Dispose();
            _loopCancellationTokenSource = null;

            IsStarted = false;
        }

        private async Task BackgroundLoopBody(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    Action(token);
                    await Task.Delay(Interval, token);
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
