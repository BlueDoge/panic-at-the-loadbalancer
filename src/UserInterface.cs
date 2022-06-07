// Originally created by Elizabeth Clements
// Copyright and License can be found in the LICENSE file or at the github (https://github.com/BlueDoge/panic-at-the-disco/)

using System;
using System.Collections.Generic;

namespace BlueDogeTools.panic_at_the_loadbalancer
{
	public class UserInterface
	{
		public UserInterface(string[] args)
		{
			Init(args);
		}

		private void Init(string[] args)
		{
		}

		private void CleanScreen()
        {
			Console.Clear();
        }

		private void AskForAccessKey()
        {
        }

		private void AskForSecretKey()
		{
		}

		private void AskForSessionKey()
		{
		}

		private void AskForRegion()
		{
		}

		private void AskForTargettedMachine()
		{
		}

		private void AskForLoadbalancerInfo()
		{
		}

		public bool Run()
		{
			CleanScreen();
			AskForAccessKey();
			AskForSecretKey();
			AskForSessionKey();
			AskForRegion();
			AskForTargettedMachine();
			AskForLoadbalancerInfo();
			return true;
		}
	}
}
