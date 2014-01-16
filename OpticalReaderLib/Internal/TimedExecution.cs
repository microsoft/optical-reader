/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using System;
using System.Threading.Tasks;

namespace OpticalReaderLib.Internal
{
    class TimedExecution
    {
        private TimeSpan _executionTime = new TimeSpan(0);

        public TimeSpan ExecutionTime
        {
            get
            {
                var t = _executionTime;

                _executionTime = new TimeSpan(0);

                return t;
            }

            private set
            {
                _executionTime = value;
            }
        }

        public async Task<T> ExecuteAsync<T>(Windows.Foundation.IAsyncOperation<T> op)
        {
            var startTime = DateTime.Now;
            var result = await op;
            var endTime = DateTime.Now;

            ExecutionTime = endTime - startTime;

            return result;
        }
    }
}
