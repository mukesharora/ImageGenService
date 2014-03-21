using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware.client.messages
{
    public abstract class OmniCalcCommandResultEvent : OmniAPIMessage
    {
        public Guid TransactionID { get; set; }

        public string VisualTagUID { get; set; }

        public bool CommandQueued { get; set; }

        public bool CommandSent { get; set; }

        public bool CommandReceived { get; set; }

        public bool CommandFailed { get; set; }

        public bool CommandRetrying { get; set; }

        public string Information { get; set; }
        
        public override string ToString()
        {
            return String.Format("TransactionID: {0}, VisualTagUID: {1}, CommandQueued: {2}, CommandSent: {3}, CommandReceived: {4}, CommandFailed: {5}, CommandRetrying {6}, Information: {7}",
                TransactionID, VisualTagUID, CommandQueued, CommandSent, CommandReceived, CommandFailed, CommandRetrying, Information);
        }

        #region Pre-baked Command Results

        private const string COMMAND_QUEUED = "Command queued for sending";
        private const string COMMAND_SENDING = "Sending command";
        private const string COMMAND_SUCCESS = "Command was successfully sent";
        private const string COMMAND_RETRYING = "Command retrying";
        private const string COMMAND_QUEUED_FAILED = "Error queuing command";
        private const string COMMAND_FAILED = "Command sending failed";

        internal OmniCalcCommandResultEvent SetCommandQueued()
        {
            CommandQueued = true;
            CommandSent = false;
            CommandReceived = false;
            CommandFailed = false;
            CommandRetrying = false;
            Information = COMMAND_QUEUED;

            return this;
        }

        internal OmniCalcCommandResultEvent SetCommandQueuedFailed()
        {
            CommandQueued = false;
            CommandSent = false;
            CommandReceived = false;
            CommandFailed = false;
            CommandRetrying = false;
            Information = COMMAND_QUEUED_FAILED;

            return this;
        }

        internal OmniCalcCommandResultEvent SetCommandRetrying()
        {
            CommandQueued = true;
            CommandSent = false;
            CommandReceived = false;
            CommandFailed = false;
            CommandRetrying = true;
            Information = COMMAND_RETRYING;

            return this;
        }

        internal OmniCalcCommandResultEvent SetCommandFailed()
        {
            CommandQueued = true;
            CommandSent = true;
            CommandReceived = false;
            CommandFailed = true;
            CommandRetrying = false;
            Information = COMMAND_FAILED;

            return this;
        }

        internal OmniCalcCommandResultEvent SetCommandSending()
        {
            CommandQueued = true;
            CommandSent = true;
            CommandReceived = false;
            CommandFailed = false;
            CommandRetrying = false;
            Information = COMMAND_SENDING;

            return this;
        }

        internal OmniCalcCommandResultEvent SetCommandSuccess()
        {
            CommandQueued = true;
            CommandSent = true;
            CommandReceived = true;
            CommandFailed = false;
            CommandRetrying = false;
            Information = COMMAND_SUCCESS;

            return this;
        }
        
        #endregion

    }
}