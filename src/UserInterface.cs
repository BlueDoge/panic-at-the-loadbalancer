// Originally created by Elizabeth Clements
// Copyright and License can be found in the LICENSE file or at the github (https://github.com/BlueDoge/panic-at-the-disco/)

using System;
using System.Collections.Generic;
using System.Security;
using Amazon.Runtime;

namespace BlueDogeTools.panic_at_the_loadbalancer
{
	public class UserInterface
	{
		private bool bIsVerbose = false;
		private Amazon.RegionEndpoint? GivenRegion;
		private string? GivenTargetId;
		private string? GivenTargetIpAddress;
		private string? GivenTargetPrivateIpAddress;
		private string? GivenLoadbalancerTargetGroupArn;

		public Amazon.RegionEndpoint GetRegion()
		{
			return GivenRegion ?? Amazon.RegionEndpoint.USWest2;
		}

		public string GetTargetInstanceId()
		{
			if(GivenTargetId == null)
			{
				throw new Exception("Error: instance id is not yet resolved.");
			}
			return GivenTargetId;
		}

		public string GetLoadbalancerTargetGroupArn()
		{
			if(GivenLoadbalancerTargetGroupArn == null)
			{
				throw new Exception("Error: ELB Target Group Arn not yet resolved.");
			}
			return GivenLoadbalancerTargetGroupArn;
		}

		public string GetIp()
		{
			if (GivenTargetIpAddress == null)
			{
				throw new Exception("Error: ip address not yet resolved.");
			}
			return GivenTargetIpAddress;
		}

		public string GetPrivateIp()
		{
			if (GivenTargetPrivateIpAddress == null)
			{
				throw new Exception("Error: private ip address not yet resolved.");
			}
			return GivenTargetPrivateIpAddress;
		}

		public UserInterface(string[] args, ref Credentials? awsCredentials)
		{
			Init(args, ref awsCredentials);
		}

		private void Init(string[] args, ref Credentials? awsCredentials)
		{
		}

		private void CleanScreen()
		{
			Console.Clear();
		}

		private bool AskAboutAWSProfiles(ref Credentials? awsCredentials)
		{
			// the method should ask if we want to select the AWS Profile
			// then use the profile provided to load up Credentials using the SDK
			// return true if we're using AWS profiles and have asked which one, so we'd skip asking about access/secret/session key info.
			Console.Write("Use default profile? [Y/n] ");
			string? enteredData = Console.ReadLine();
			if (enteredData == null || enteredData.Trim() == "" || enteredData.ToLower().Substring(0, 1) == "y")
			{
				Console.Write("Which profile? [default] ");
				enteredData = Console.ReadLine();
				if(enteredData == null || enteredData.Trim() == "")
				{
					awsCredentials = new Credentials();
				}
				else
				{
					awsCredentials = new Credentials(enteredData);
				}
				return true;
			}
			return false;
		}

		private void AskForCredentials(ref Credentials? awsCredentials)
		{
			if (!AskAboutAWSProfiles(ref awsCredentials))
			{
				SecureString? accessKey;
				SecureString? secretKey;
				SecureString? sessionKey;
				AskForAccessKey(out accessKey);
				AskForSecretKey(out secretKey);
				AskForSessionKey(out sessionKey);

				// sessionKey can be null and the underlying class handles it appropriately
				awsCredentials = new Credentials(accessKey, secretKey, sessionKey);
			}
		}

		private void AskForAccessKey(out SecureString accessKey)
		{
			Console.Write("Enter access key: ");
			accessKey = Utilities.ReadSecureLine();
		}

		private void AskForSecretKey(out SecureString secretKey)
		{
			Console.Write("Enter secret key: ");
			secretKey = Utilities.ReadSecureLine();
		}

		private void AskForSessionKey(out SecureString? sessionKey)
		{
			Console.Write("Are we using a session key? [Y/n] ");
			string? enteredData = Console.ReadLine();
			if (enteredData == null || enteredData.Trim() == "" || enteredData.ToLower().Substring(0, 1) == "y")
			{
				SecureString secureData = Utilities.ReadSecureLine();
				if(enteredData == null)
				{
					throw new Exception("Error: invalid session key provided.");
				}
				sessionKey = secureData;
			}
			else
			{
				sessionKey = null;
			}
		}

		private void AskForRegion()
		{
			// ask user for region endpoint they want to target
			// target it using BlueDogeTools.panic_at_the_loadbalancer.GetRegionEndpointFromString(string region)
			Console.Write("Region [US-West-2]: ");
			string Data = Console.ReadLine() ?? "";
			if (Data.Trim() != "")
			{
				GivenRegion = Utilities.GetRegionEndpointFromString(Data, bIsVerbose);
			}
			else
			{
				// oregon, best dc
				GivenRegion = Amazon.RegionEndpoint.USWest2;
			}

			if (bIsVerbose)
			{
				Console.WriteLine("Selected region {0} <{1}>", GivenRegion.DisplayName, GivenRegion.SystemName);
			}
		}

		private void AskForTargettedMachine(ref Credentials? awsCredentials, ref ECCInstanceFinder? eccInstanceFinder)
		{
			// load up the EC2 client
			eccInstanceFinder = new ECCInstanceFinder(ref awsCredentials, GivenRegion ?? Amazon.RegionEndpoint.USWest2);

			// grab the instance ID from the provided IPAddress
			Console.WriteLine("(You can select instances by Public IPv4, Private IPv4, or Instance ID)");
			Console.WriteLine("(Acceptable responses: public ip, private ip, id, instance id)");
			Console.Write("Instance Selection Mode [Public IPv4]: ");
			string Data = (Console.ReadLine() ?? "Public IPv4").ToLower();
			if (Data == "public ip")
			{
				Console.Write("Provide the Public IPv4: ");
				string? IP = Console.ReadLine();
				if(IP == null) // also check if it even is a valid ip address
				{
					throw new Exception("Invalid ip address provided.");
				}
				GivenTargetId = eccInstanceFinder.GetInstanceIDFromPublicIPAddress(IP);
				GivenTargetIpAddress = IP;
			}
			else if (Data == "private ip")
			{
				Console.Write("Provide the Private IPv4: ");
				string? IP = Console.ReadLine();
				if(IP == null) // also check if it even is a valid ip address
				{
					throw new Exception("Invalid ip address provided.");
				}
				GivenTargetId = eccInstanceFinder.GetInstanceIDFromPrivateIPAddress(IP);
				GivenTargetPrivateIpAddress = IP;
			}
			else if (Data == "id" || Data == "instance id")
			{
				Console.Write("Provide the Instance ID: ");
				string? ID = Console.ReadLine();
				if(ID == null) // also check if it even is a valid instance id [won't be perfect]
				{
					throw new Exception("Potentially invalid instance id provided");
				}
				GivenTargetId = ID;
			}
		}

		private void AskForLoadbalancerInfo()
		{
			Console.Write("Load balancer Target Group Arn: ");
			string? Data = Console.ReadLine();
			if (Data == null || Data.Trim() == "")
			{
				throw new Exception("Potentially invalid Target Group Arn provided.");
			}
			else
			{
				GivenLoadbalancerTargetGroupArn = Data;
			}
		}

		public bool Run(ref Credentials? awsCredentials, ref ECCInstanceFinder? eccInstanceFinder)
		{
			CleanScreen();
			AskForCredentials(ref awsCredentials);
			AskForRegion();
			AskForTargettedMachine(ref awsCredentials, ref eccInstanceFinder);
			AskForLoadbalancerInfo();
			// return false if there isn't at least an accesskey and secretkey
			if (awsCredentials == null)
			{
				return false;
			}

			return true;
		}
	}
}
