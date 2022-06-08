// Originally created by Elizabeth Clements
// Copyright and License can be found in the LICENSE file or at the github (https://github.com/BlueDoge/panic-at-the-disco/)

using Amazon.EC2;
using Amazon.EC2.Model;

namespace BlueDogeTools.panic_at_the_loadbalancer
{
	public class ECCInstanceFinder
	{
		private AmazonEC2Client _Client;
		public ECCInstanceFinder(ref Credentials? credentials, string? Region)
		{
			if(credentials != null)
			{
				_Client = new AmazonEC2Client(credentials.awsCredentials, Utilities.GetRegionEndpointFromString(Region ?? "uswest2"));
			}
			else
			{
				_Client = new AmazonEC2Client(Utilities.GetRegionEndpointFromString(Region ?? "uswest2"));
			}
		}
		public ECCInstanceFinder(ref Credentials? credentials, Amazon.RegionEndpoint Region)
		{
			if (credentials != null)
			{
				_Client = new AmazonEC2Client(credentials.awsCredentials, Region);
			}
			else
			{
				_Client = new AmazonEC2Client(Region);
			}
		}
		public string GetInstanceIDFromPublicIPAddress(string ipAddress)
		{
			//aws ec2 describe-instances --filter Name=ip-address,Values=ipAddress
			var descResponse = _Client.DescribeInstancesAsync(new DescribeInstancesRequest() {
				Filters =
				{
					new Filter("ip-address")
					{
						Values = { ipAddress } // we're only looking for ip addresses
					}
				}
			}).GetAwaiter().GetResult();

			// we're only going to want the first of each of these arrays, only should have one result
			return descResponse.Reservations[0].Instances[0].InstanceId;
		}
		public string GetInstanceIDFromPrivateIPAddress(string ipAddress)
		{
			//aws ec2 describe-instances --filter Name=ip-address,Values=ipAddress
			var descResponse = _Client.DescribeInstancesAsync(new DescribeInstancesRequest()
			{
				Filters =
				{
					new Filter("private-ip-address")
					{
						Values = { ipAddress } // we're only looking for ip addresses
					}
				}
			}).GetAwaiter().GetResult();

			// we're only going to want the first of each of these arrays, only should have one result
			return descResponse.Reservations[0].Instances[0].InstanceId;
		}
		public string GetPublicIPAddressFromInstanceID(string instanceID)
		{
			var descResponse = _Client.DescribeInstancesAsync(new DescribeInstancesRequest() {
				InstanceIds = { instanceID }
			}).GetAwaiter().GetResult();

			return descResponse.Reservations[0].Instances[0].PublicIpAddress;
		}
		public string GetPrivateIPAddressFromInstanceID(string instanceID)
		{
			var descResponse = _Client.DescribeInstancesAsync(new DescribeInstancesRequest()
			{
				InstanceIds = { instanceID }
			}).GetAwaiter().GetResult();

			return descResponse.Reservations[0].Instances[0].PrivateIpAddress;
		}
	}
}
