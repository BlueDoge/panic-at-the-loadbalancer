// Originally created by Elizabeth Clements
// Copyright and License can be found in the LICENSE file or at the github (https://github.com/BlueDoge/panic-at-the-loadbalancer/)

using System;
using System.Runtime.InteropServices;
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
			foreach (var element in OrderedList)
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
			while (keyData.Key != ConsoleKey.Enter)
			{
				bool bIsValidChar = keyData.KeyChar.IsValidKeyboardCharacter();
				if (keyData.Key == ConsoleKey.Backspace)
				{
					secureLine.RemoveAt(secureLine.Length - 1);
					// if we're not overriding, or we are and adding characters...
					// we should remove the character added
					if (!bOverride || (bOverride && bAddChars))
					{
						Console.Write("\b \b");
					}
				}
				if (bIsValidChar)
				{
					if (bAddChars && bOverride)
					{
						Console.Write(charToAdd); // would add *'s if defaulted
					}
					secureLine.AppendChar(keyData.KeyChar);
				}
				keyData = Console.ReadKey(bOverride);
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

		public static void ClearScreen()
		{
			Console.Clear();
		}

		public static void WriteHeader()
		{
			ClearScreen();
			Console.WriteLine("Secure Shell Example");
			Console.WriteLine("Created by Liz Clements in 2022");
			Console.Write("\n\n");
		}

		public static void HardError<T>(object? origin, string message) where T : Exception
		{
			Console.Error.WriteLine(message);
			string exceptionMessage = String.Format("Name: {0}\nMessage: {1}", origin == null ? "null" : origin.GetType().FullName, message).ToString();
			throw (T)Activator.CreateInstance(typeof(T), new object[] { exceptionMessage }) ?? new Exception(exceptionMessage);
		}
	}
}