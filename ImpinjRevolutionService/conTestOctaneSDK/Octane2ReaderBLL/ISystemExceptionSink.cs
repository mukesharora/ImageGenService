using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Octane2ReaderBLL
{
    public interface ISystemExceptionSink
    {
        void LogSystemException(string description);
    }
}
