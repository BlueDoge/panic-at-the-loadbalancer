// Originally created by Elizabeth Clements
// Copyright and License can be found in the LICENSE file or at the github (https://github.com/BlueDoge/panic-at-the-loadbalancer/)

using System;
using System.Collections.Generic;

namespace BlueDogeTools.panic_at_the_loadbalancer
{
	public class Toolkit
	{
		private Credentials? credentials;
		private ECCInstanceFinder? eccInstanceFinder;
		private UserInterface userInterface;
		private RemoteShell shell;
		private ELBController lbController;

		private void RegisterTargetToTargetGroup()
		{
		}

		public Toolkit(string[] args)
		{
			userInterface = new UserInterface(args, ref credentials, ref eccInstanceFinder, true);

			// now build the shell and autorun the "health check" or provisioner
			shell = new RemoteShell(userInterface.GetIp(), userInterface.GetScriptFilepath(), true);

			if (!shell.IsHealthy())
			{
				throw new Exception("Error: the ssh instance claims it never achieved a healthy state. This situation may require human intervention.");
			}

			// access lb and add it back
			lbController = new ELBController(ref credentials);
		}
	}
}
