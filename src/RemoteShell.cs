﻿// Originally created by Elizabeth Clements
// Copyright and License can be found in the LICENSE file or at the github (https://github.com/BlueDoge/panic-at-the-disco/)

using System;
using System.Collections.Generic;

namespace BlueDogeTools.panic_at_the_loadbalancer
{
	public class RemoteShell
	{
		private bool bIsHealthy = false;

		public bool IsHealthy()
		{
			return bIsHealthy;
		}

		public RemoteShell(string ipAddress, string scriptFilepath, bool bAutoRun = false)
		{
		}
	}
}
