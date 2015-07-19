using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    class Disposable
    {
        public readonly static IDisposable Empty = new EmptyDisposable();

        class EmptyDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
