// Originally created by Elizabeth Clements
// Copyright and License can be found in the LICENSE file or at the github (https://github.com/BlueDoge/panic-at-the-disco/)

using System;
using System.Collections.Generic;
using Amazon.EC2;
using Amazon.ElasticLoadBalancingV2;

namespace BlueDogeTools.panic_at_the_loadbalancer
{
	public class Toolkit
	{
		private UserInterface _userInterface;
		private RemoteShell _remoteShell;
		public Toolkit(string[] args)
		{
			_userInterface = new UserInterface(args);
			// actually run the user interface
			_userInterface.Run();

			// now build the shell
			_remoteShell = new RemoteShell();
		}
	}
}
