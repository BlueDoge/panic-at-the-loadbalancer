// Originally created by Elizabeth Clements
// Copyright and License can be found in the LICENSE file or at the github (https://github.com/BlueDoge/panic-at-the-loadbalancer/)

using System;
using System.Collections.Generic;

namespace BlueDogeTools.panic_at_the_loadbalancer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// go to the actual entry point for this program
			Toolkit toolkit = new Toolkit(args);
		}
	}
}