// Originally created by Elizabeth Clements
// Copyright and License can be found in the LICENSE file or at the github (https://github.com/BlueDoge/panic-at-the-disco/)

using System.Reflection;
using System.Security;

namespace BlueDogeTools.panic_at_the_loadbalancer
{
	public static class Utilities
	{
		public static void WritePrompt(string Message, int embeddedDepth = 1, char embeddedPadding = '>')
		{
			// PadLeft with the Length of the message + the depth of the embedding. Starting with 1
			Console.Write(Message.PadLeft(embeddedDepth + Message.Length, embeddedPadding));
		}

		public static void WriteList(string[] OrderedList)
		{
			Int32 iter = 1;
			foreach(var element in OrderedList)
			{
				Console.WriteLine("{0}. {1}", iter++, element);
			}
		}

		public static void WriteLog(string Tag, string Message)
		{
			Console.WriteLine("[{0}] {1}", Tag, Message);
		}

		public static SecureString ReadSecureLine(bool bOverride = true, bool bAddChars = true, char charToAdd = '*')
		{
			SecureString secureLine = new SecureString();
			var keyData = Console.ReadKey(bOverride);
			while(keyData.Key != ConsoleKey.Enter)
			{
				secureLine.AppendChar(keyData.KeyChar);
				keyData = Console.ReadKey(bOverride);
				if(bAddChars && bOverride)
				{
					Console.Write(charToAdd); // would add *'s if defaulted
				}
			}
			// the new line was consumed.
			Console.Write('\n');

			return secureLine;
		}

		public static Amazon.RegionEndpoint GetRegionEndpointFromString(string HumanReadableRegion, bool verbose = false)
		{
			Type type = typeof(Amazon.RegionEndpoint);
			foreach (var f in type.GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				var v = f.GetValue(null);
				if (v == null) continue;
				var rep = (Amazon.RegionEndpoint)v;

				if (verbose)
					Console.WriteLine("[{0}] {1} <{2}>", f.Name, rep.SystemName, rep.DisplayName);

				if (rep.SystemName.ToLower() == HumanReadableRegion.ToLower())
				{
					// oh thank goodness its easy
					return rep;
				}
				else if (rep.SystemName.ToLower().Replace("-", "") == HumanReadableRegion.ToLower().Replace("-", ""))
				{
					// super easy
					return rep;
				}
				else
				{
					if (rep.DisplayName
						.ToLower()
						.Replace("(", "").Replace(")", "") // remove the ( ) from the region
						.Contains(HumanReadableRegion.ToLower().Replace("(", "").Replace(")", "")))
					{
						return rep;
					}
				}
			}

			// defaults: US West (Oregon)
			return Amazon.RegionEndpoint.USWest2;
		}
	}
}
