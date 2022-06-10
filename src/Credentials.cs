// Originally created by Elizabeth Clements
// Copyright and License can be found in the LICENSE file or at the github (https://github.com/BlueDoge/panic-at-the-disco/)

using System;
using System.Security;
using Amazon.Runtime;
using Amazon.Runtime.Credentials;
using Amazon.Runtime.CredentialManagement;

namespace BlueDogeTools.panic_at_the_loadbalancer
{
	public class Credentials
	{
		public AWSCredentials? awsCredentials;
		public SecureString? sshUsername;
		public SecureString? sshPassword;

		public Credentials(SecureString accessKey, SecureString secretKey, SecureString? sessionKey = null)
		{
			if (sessionKey != null)
			{
				awsCredentials = new SessionAWSCredentials(accessKey.ToString(), secretKey.ToString(), sessionKey.ToString());
			}
			else
			{
				awsCredentials = new BasicAWSCredentials(accessKey.ToString(), secretKey.ToString());
			}

			Utilities.WriteLog("Credentials", "Created AWS Credentials.");

			// super ensure these are wiped
			accessKey.Dispose();
			secretKey.Dispose();
			sessionKey?.Dispose();
		}
		public Credentials(string profileName = "default")
		{
			var chain = new CredentialProfileStoreChain();
			if(!chain.TryGetAWSCredentials(profileName, out awsCredentials))
			{
				throw new Exception("Critical Error: Could not find the aws profile specified!");
			}
			else
			{
				Utilities.WriteLog("Credentials", "Created AWS Credentials.");
			}
		}
	}
}
