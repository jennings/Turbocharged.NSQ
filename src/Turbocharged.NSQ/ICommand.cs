using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    interface ICommand
    {
        byte[] ToByteArray();
    }

    interface ICommandWithResponse : ICommand
    {
        void Complete(byte[] data);
    }

    interface ICommand<T> : ICommandWithResponse
    {
        Task<T> Task { get; }
    }
}
