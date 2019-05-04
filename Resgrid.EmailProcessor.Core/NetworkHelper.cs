using System;
using System.Net.NetworkInformation;

namespace Resgrid.EmailProcessor.Core
{
	public static class NetworkHelper
	{
		public static bool IsPortAvailable(int port)
		{
			// https://stackoverflow.com/questions/570098/in-c-how-to-check-if-a-tcp-port-is-available/570461
			bool isAvailable = true;

			try
			{
				// Evaluate current system tcp connections. This is the same information provided
				// by the netstat command line application, just in .Net strongly-typed object
				// form.  We will look through the list, and if our port we would like to use
				// in our TcpClient is occupied, we will set isAvailable to false.
				IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
				TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

				foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
				{
					if (tcpi.LocalEndPoint.Port == port)
					{
						isAvailable = false;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				isAvailable = false;
			}

			return isAvailable;
		}
	}
}
