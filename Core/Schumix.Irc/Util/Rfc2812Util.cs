/*
 * This file is part of Schumix.
 * 
 * Copyright (C) 2002 Aaron Hunter <thresher@sharkbite.org>
 * Copyright (C) 2010-2012 Twl
 * Copyright (C) 2010-2013 Megax <http://megax.yeahunter.hu/>
 * Copyright (C) 2013 Schumix Team <http://schumix.eu/>
 * 
 * Schumix is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Schumix is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Schumix.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Schumix.Framework;
using Schumix.Framework.Irc;
using Schumix.Framework.Config;
using Schumix.Framework.Logger;
using Schumix.Framework.Extensions;
using Schumix.Framework.Localization;

namespace Schumix.Irc.Util
{
	/// <summary>
	/// RFC 2812 Utility methods.
	/// </summary>
	public static class Rfc2812Util
	{
		private static readonly LocalizationConsole sLConsole = Singleton<LocalizationConsole>.Instance;
		// Odd chars that IRC allows in nicknames
		private const string Nick = @"^[" + Special + @"a-zA-Z0-9\-]{0,20}$";
		private const string User = @"(" + Nick+ @")!([\~\w]+)@([\w\.\-]+)";
		private const string Special = @"\[\]\`_\^\{\|\}";
		// Regex that matches the standard IRC 'nick!user@host'
		// Regex that matches a legal IRC nick
		private static readonly Regex NickRegex;
		private static readonly Regex EmailRegex;
		//Regex to create a UserInfo from a string
		private static readonly Regex NameSplitterRegex;
		private const string ChannelPrefix = "#!+&";
		private const string UserModes = "awiorOs";
		private const string ChannelModes = "OohvaimnqpsrtklbeI";

		/// <summary>
		/// Static initializer 
		/// </summary>
		static Rfc2812Util()
		{
			NickRegex = new Regex(Nick); 
			EmailRegex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[A-Z]{2}|com|org|net|hu|gov|mil|biz|info|mobi|name|aero|jobs|museum)\b");
			NameSplitterRegex = new Regex("[!@]", RegexOptions.Compiled | RegexOptions.Singleline);
		}

		//Should never be instantiated
		/// <summary>
		/// Converts the user string sent by the IRC server
		/// into a UserInfo object.
		/// </summary>
		/// <param name="fullUserName">The user in nick!user@host form.</param>
		/// <returns>A UserInfo object.</returns>
		public static UserInfo UserInfoFromString(string fullUserName)
		{
			var parts = ParseUserInfoLine(fullUserName);
			return parts.IsNull() ? UserInfo.Empty : new UserInfo(parts[0], parts[1], parts[2]);
		}

		/// <summary>
		/// Break up an IRC user string into its component
		/// parts. 
		/// </summary>
		/// <param name="fullUserName">The user in nick!user@host form</param>
		/// <returns>A string array with the first item being nick, then user, and then host.</returns>
		public static string[] ParseUserInfoLine(string fullUserName)
		{
			if(fullUserName.IsNull() || fullUserName.Trim().Length == 0)
				return null;

			var match = NameSplitterRegex.Match(fullUserName);
			if(match.Success)
			{
				var parts = NameSplitterRegex.Split(fullUserName);
				return parts;
			}

			return new[] { fullUserName, string.Empty, string.Empty };
		}

		/// <summary>
		/// Using the rules set forth in RFC 2812 determine if
		/// an array of channel names is valid.
		/// </summary>
		/// <returns>True if the channel names are all valid.</returns>
		public static bool IsValidChannelList(string[] channels)
		{
			return !channels.IsNull() && channels.Length != 0 && channels.All(IsValidChannelName);
		}

		/// <summary>
		/// Using the rules set forth in RFC 2812 determine if
		/// the channel name is valid.
		/// </summary>
		/// <returns>True if the channel name is valid.</returns>
		public static bool IsValidChannelName(string channel)
		{
			if(channel.IsNullOrEmpty())
				return false;
			
			if(ContainsSpace(channel))
				return false;

			if(ChannelPrefix.Contains(channel[0]))
			{
				if(channel.Length <= 50)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Using the rules set forth in RFC 2812 determine if
		/// the nickname is valid.
		/// </summary>
		/// <returns>True is the nickname is valid</returns>
		public static bool IsValidNick(string nick)
		{
			if(nick.IsNullOrEmpty())
				return false;

			return !ContainsSpace(nick) && !nick.IsNumber() && !nick.Substring(0, 1).IsNumber() && NickRegex.IsMatch(nick);
		}

		/// <summary>
		/// Check if whether the provided e-mail address is considered valid. 
		/// </summary>
		/// <param name="email">
		/// The e-mail address to check.
		/// </param>
		/// <returns>
		/// True if valid, otherwise False.
		/// </returns>
		public static bool IsValidEmailAddress(string email)
		{
			return EmailRegex.IsMatch(email);
		}

		/// <summary>
		/// Using the rules set forth in RFC 2812 determine if
		/// an array of nicknames names is valid.
		/// </summary>
		/// <returns>True if the channel names are all valid.</returns>
		public static bool IsValidNickList(string[] nicks)
		{
			if(nicks.IsNull() || nicks.Length == 0)
				return false;
			
			return nicks.All(IsValidNick);
		}	

		/// <summary>
		/// Convert a ModeAction into its RFC2812 character.
		/// </summary>
		/// <param name="action">The action enum.</param>
		/// <returns>Either '+' or '-'.</returns>
		public static char ModeActionToChar(ModeAction action)
		{
			return ((byte)action).ToChar(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the char received from the IRC server into
		/// its enum equivalent.
		/// </summary>
		/// <param name="action">Either '+' or '-'.</param>
		/// <returns>An action enum.</returns>
		public static ModeAction CharToModeAction(char action)
		{
			byte b = action.ToByte(CultureInfo.InvariantCulture);
			return (ModeAction)Enum.Parse(typeof(ModeAction), b.ToString(CultureInfo.InvariantCulture), false);
		}

		/// <summary>
		/// Converts a UserMode into its RFC2812 character.
		/// </summary>
		/// <param name="mode">The mode enum.</param>
		/// <returns>The corresponding char.</returns>
		public static char UserModeToChar(UserMode mode)
		{
			return ((byte)mode).ToChar(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Convert a string of UserModes characters to
		/// an array of UserMode enums.
		/// </summary>
		/// <param name="modes">A string of UserMode chars from the IRC server.</param>
		/// <returns>An array of UserMode enums. Charactres that are not from RFC2812 will be droppped.</returns>
		public static UserMode[] UserModesToArray(IEnumerable<char> modes)
		{
			return (from t in modes where IsValidModeChar(t, UserModes) select CharToUserMode(t)).ToArray();
		}

		/// <summary>
		/// Converts the char recived from the IRC server into
		/// its enum equivalent.
		/// </summary>
		/// <param name="mode">One of the IRC mode characters, e.g. 'a','i', etc...</param>
		/// <returns>An mode enum.</returns>
		public static UserMode CharToUserMode(char mode)
		{
			byte b = mode.ToByte(CultureInfo.InvariantCulture);
			return (UserMode)Enum.Parse(typeof(UserMode), b.ToString(CultureInfo.InvariantCulture), false);
		}

		/// <summary>
		/// Convert a string of ChannelModes characters to
		/// an array of ChannelMode enums.
		/// </summary>
		/// <param name="modes">A string of ChannelMode chars from the IRC server.</param>
		/// <returns>An array of ChannelMode enums. Charactres that are not from RFC2812 will be droppped.</returns>
		public static ChannelMode[] ChannelModesToArray(IEnumerable<char> modes)
		{
			return (from t in modes where IsValidModeChar(t, ChannelModes) select CharToChannelMode(t)).ToArray();
		}

		/// <summary>
		/// Converts a ChannelMode into its RFC2812 character.
		/// </summary>
		/// <param name="mode">The mode enum.</param>
		/// <returns>The corresponding char.</returns>
		public static char ChannelModeToChar(ChannelMode mode)
		{
			return ((byte)mode).ToChar(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the char recived from the IRC server into
		/// its enum equivalent.
		/// </summary>
		/// <param name="mode">One of the IRC mode characters, e.g. 'O','i', etc...</param>
		/// <returns>An mode enum.</returns>
		public static ChannelMode CharToChannelMode(char mode)
		{
			byte b = mode.ToByte(CultureInfo.InvariantCulture);
			return (ChannelMode)Enum.Parse(typeof(ChannelMode), b.ToString(CultureInfo.InvariantCulture), false);
		}

		/// <summary>
		/// Converts a StatQuery enum value to its RFC2812 character.
		/// </summary>
		/// <param name="query">The query enum.</param>
		/// <returns>The corresponding char.</returns>
		public static char StatsQueryToChar(StatsQuery query)
		{
			return ((byte)query).ToChar(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the char recived from the IRC server into
		/// its enum equivalent.
		/// </summary>
		/// <param name="queryType">One of the IRC stats query characters, e.g. 'u','l', etc...</param>
		/// <returns>An StatsQuery enum.</returns>
		public static StatsQuery CharToStatsQuery(char queryType)
		{
			byte b = queryType.ToByte(CultureInfo.InvariantCulture);
			return (StatsQuery)Enum.Parse(typeof(StatsQuery), b.ToString(CultureInfo.InvariantCulture), false);
		}

		private static bool IsValidModeChar(char c, string validList)
		{
			return validList.Contains(c);
		}

		private static bool ContainsSpace(string text)
		{
			return text.IndexOf(SchumixBase.Space, 0, text.Length) != -1;
		}

		public static void SetMessageType(this IRCMessage sIRCMessage)
		{
			sIRCMessage.SetMessageType(IRCConfig.List[sIRCMessage.ServerName].MessageType.ToLower());
		}
		
		public static void SetMessageType(this IRCMessage sIRCMessage, string type)
		{
			switch(type.ToLower())
			{
			case "privmsg":
				sIRCMessage.MessageType = MessageType.Privmsg;
				break;
			case "notice":
				sIRCMessage.MessageType = MessageType.Notice;
				break;
			default:
				sIRCMessage.MessageType = MessageType.Privmsg;
				break;
			}
		}

		public static bool IsServ(this string Value)
		{
			foreach(var func in Enum.GetNames(typeof(Serv)))
			{
				if(func == Value)
					return true;
			}

			return false;
		}

		public static bool IsServInLower(this string Value)
		{
			foreach(var func in Enum.GetNames(typeof(Serv)))
			{
				if(func.ToLower() == Value.ToLower())
					return true;
			}

			return false;
		}

		public static bool IsServ(this string Value, Serv serv)
		{
			foreach(var func in Enum.GetNames(typeof(Serv)))
			{
				if(func == Value && Value == serv.ToString())
					return true;
			}
			
			return false;
		}

		public static bool IsCtcp(string message)
		{
			return !message.IsNullOrEmpty() && message.StartsWith("\u0001") && message.EndsWith("\u0001") && !IsAction(message);
		}

		public static string GetCtcp(string message)
		{
			if(!IsCtcp(message))
				Log.Error("Rfc2812Util", sLConsole.GetString("The message is not a CTCP message."));

			return message.Substring(1).Substring(0, message.Length - 2);
		}

		public static bool IsAction(string message)
		{
			return !message.IsNullOrEmpty() && message.StartsWith("\u0001ACTION") && message.EndsWith("\u0001");
		}

		public static string GetAction(string message)
		{
			if(!IsAction(message))
				Log.Error("Rfc2812Util", sLConsole.GetString("The message is not an action."));

			return message.Substring(8).Substring(0, message.Length - 9);
		}
	}
}