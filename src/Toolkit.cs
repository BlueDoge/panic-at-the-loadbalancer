// Originally created by Elizabeth Clements
// Copyright and License can be found in the LICENSE file or at the github (https://github.com/BlueDoge/panic-at-the-disco/)

using System;
using System.Collections.Generic;

namespace BlueDogeTools.panic_at_the_loadbalancer
{
	public class Toolkit
	{
		private Credentials? credentials;
		private ECCInstanceFinder? eccInstanceFinder;
		private UserInterface _userInterface;
		private RemoteShell _remoteShell;

		private void RegisterTargetToTargetGroup()
		{
		}

		public Toolkit(string[] args)
		{
			_userInterface = new UserInterface(args, ref credentials);
			// actually run the user interface
			_userInterface.Run(ref credentials, ref eccInstanceFinder);

			// now build the shell
			_remoteShell = new RemoteShell();
		}
	}
}
