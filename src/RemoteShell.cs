// Originally created by Elizabeth Clements
// Copyright and License can be found in the LICENSE file or at the github (https://github.com/BlueDoge/panic-at-the-loadbalancer/)

using System;
using System.Collections.Generic;
using System.Security;
using Renci.SshNet;

namespace BlueDogeTools.panic_at_the_loadbalancer
{
	public class RemoteShell
	{
		string targetIpAddress;
		string targetScriptFilepath;
		private bool bIsHealthy = false;

		public bool IsHealthy()
		{
			return bIsHealthy;
		}

		public RemoteShell(string ipAddress, string scriptFilepath, bool bAutoRun = false)
		{
			targetIpAddress = ipAddress;
			targetScriptFilepath = scriptFilepath;

			if(bAutoRun)
			{
				Run();
			}
		}

		private void AskForUsername(out SecureString username)
		{
			Console.Write("Username ({0}): ");
			username = Utilities.ReadSecureLine(false); // don't hide it at all
		}

		private void AskForPassword(out SecureString password)
		{
			Console.Write("Password ({0}): ");
			password = Utilities.ReadSecureLine();
		}

		public void Run()
		{
			if (IsHealthy()) return;

			SecureString username;
			SecureString password;
			AskForUsername(out username);
			AskForPassword(out password);

			Utilities.WriteLog("SecureShell", String.Format("Connecting to {0}...", targetIpAddress));


			// definitely need to do this differently but this effectively gets what I'm trying to do... so far
			using (var c = new SftpClient(targetIpAddress, username.UseDecryptedSecureString(s => { return s; }), password.UseDecryptedSecureString(s => { return s; })))
			{
				c.KeepAliveInterval = TimeSpan.FromSeconds(60);
				c.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
				c.Connect();
				c.UploadFile(File.OpenRead(targetScriptFilepath), "/tmp/bluedoge.patlb.script.sh");
				c.Disconnect();
			} // c.Dispose()
			using (var c = new SshClient(targetIpAddress, username.UseDecryptedSecureString(s => { return s; }), password.UseDecryptedSecureString(s => { return s; })))
			{
				c.Connect();
				c.RunCommand("cat /tmp/bluedoge.patlb.script.sh | sh > /tmp/bluedoge.patlb.transaction.log");
				c.Disconnect();
			} // c.Dispose()
			username.Dispose();
			password.Dispose();
		}
	}
}
