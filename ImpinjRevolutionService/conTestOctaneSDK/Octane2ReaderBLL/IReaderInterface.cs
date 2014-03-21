using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Impinj.OctaneSdk;

namespace Octane2ReaderBLL
{
	public interface IReaderInterface
	{
		List<ImpinjReader> GetReaders();
		ImpinjReader GetReader(string deviceID);

		void Start();
		void Stop();
		void Restart();
		void TryReconnectToDisconnectedReaders();
		
		void SetGPO(ImpinjReader reader, int port, bool portState);
		void StartStopReader(ImpinjReader reader, bool start);
	}
}
