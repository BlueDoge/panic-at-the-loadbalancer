// Originally created by Elizabeth Clements
// Copyright and License can be found in the LICENSE file or at the github (https://github.com/BlueDoge/panic-at-the-loadbalancer/)

using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Security;

namespace BlueDogeTools
{
	public static class CharExtentions
	{
		// is valid keyboard entry
		public static bool IsValidKeyboardCharacter(this char test)
		{
			return test != '\u0000';
		}
	}

	public static class StringExtensions
	{
		public static T UseDecryptedSecureString<T>(this SecureString secureString, Func<string, T> action)
		{
			var length = secureString.Length;
			var sourcePtr = IntPtr.Zero;

			// Create an empty string of the correct size and pin it so the collector can't copy it all over the place.
			var plainString = new string('\0', length);
			var plainStringHandler = GCHandle.Alloc(plainString, GCHandleType.Pinned);

			var insecureStringPointer = plainStringHandler.AddrOfPinnedObject();

			try
			{
				// Create a basic string of the secure string.
				sourcePtr = Marshal.SecureStringToBSTR(secureString);

				// loop through the basic string and populate the managed string
				for (var i = 0; i < secureString.Length; i++)
				{
					var unicodeChar = Marshal.ReadInt16(sourcePtr, i * 2);
					Marshal.WriteInt16(insecureStringPointer, i * 2, unicodeChar);
				}

				return action(plainString);
			}
			finally
			{
				// clear the managed string and then unpin for GC
				Marshal.Copy(new byte[length * 2], 0, insecureStringPointer, length * 2);
				plainStringHandler.Free();

				// free and clear the basic string
				Marshal.ZeroFreeBSTR(sourcePtr);
			}
		}

		public static void UseDecryptedSecureString(this SecureString secureString, Action<string> action)
		{
			UseDecryptedSecureString(secureString, s =>
			{
				action(s);
				return 0;
			});
		}
	}

	public static class ObjectExtentions
	{
		public static void HardError<T>(this object? origin, string message) where T : Exception
		{
			Console.Error.WriteLine(message);
			string exceptionMessage = String.Format("Name: {0}\nMessage: {1}", origin == null ? "null" : origin.GetType().FullName, message).ToString();
			throw (T?)Activator.CreateInstance(typeof(T), new object[] { exceptionMessage }) ?? new Exception(exceptionMessage);
		}
	}
}
