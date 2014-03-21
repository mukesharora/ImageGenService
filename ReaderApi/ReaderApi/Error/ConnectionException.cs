using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReaderApi.Error
{
    public class ConnectionException : System.Exception
    {
        /// <summary>Initializes a new instance of the <see cref="T:ReaderApi.Exception.ConnectionException"/> class.
		/// </summary>
		public ConnectionException() { 			
 		}

        /// <summary>Initializes a new instance of the <see cref="T:ReaderApi.Exception.ConnectionException"/> class
		/// with a specified error message.</summary>
		/// <param name="message">The message that describes the error. </param>
        public ConnectionException(string message) : base(message)
        { 			
 		}
    }
}
