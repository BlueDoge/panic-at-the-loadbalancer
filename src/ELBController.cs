// Originally created by Elizabeth Clements
// Copyright and License can be found in the LICENSE file or at the github (https://github.com/BlueDoge/panic-at-the-loadbalancer/)

using System;
using System.Collections.Generic;
using Amazon.Runtime;
using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;

namespace BlueDogeTools.panic_at_the_loadbalancer
{
	public class ELBController
	{
		public ELBController(ref Credentials? awsCredentials)
		{
			if (awsCredentials == null) throw new ArgumentNullException("Error: invalid AWS Creds");
		}
	}
}
