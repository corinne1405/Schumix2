/*
 * This file is part of Schumix.
 * 
 * Copyright (C) 2010-2011 Megax <http://www.megaxx.info/>
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
using System.Text;
using Schumix.API;
using Schumix.Framework;
using Schumix.Framework.Extensions;
using Schumix.Framework.Localization;

namespace Schumix.Irc.Ctcp
{
	public sealed class CtcpSender
	{
		private readonly LocalizationConsole sLConsole = Singleton<LocalizationConsole>.Instance;
		private readonly SendMessage sSendMessage = Singleton<SendMessage>.Instance;
		private readonly Utilities sUtilities = Singleton<Utilities>.Instance;
		private string _fingerMessage;
		private string _userInfoMessage;
		private string _versionMessage;
		private string _sourceMessage;
		private string _clientInfoMessage;

		private CtcpSender()
		{
			_userInfoMessage = "Schumix CTCP";		
			_fingerMessage = _userInfoMessage;
			_versionMessage = string.Format("Schumix {0}\n{1} {2}\n{3} {4}", sUtilities.GetVersion(), sLConsole.CtcpSender("Text7"), sUtilities.GetOSName(), sLConsole.CtcpSender("Text8"), sUtilities.GetCpuId());
			_sourceMessage = "https://github.com/megax/Schumix2";
			_clientInfoMessage = sLConsole.CtcpSender("Text");
		}

		public void CtcpReply(IRCMessage sIRCMessage)
		{
			string args = sIRCMessage.Args;

			if(!args.Contains(""))
				return;

			args = args.Remove(0, 1, "");
			args = args.Substring(0, args.IndexOf(""));
			var command = args.Split(SchumixBase.NewLine);

			switch(command[0])
			{
				case CtcpUtil.Finger:
					sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, _fingerMessage + sLConsole.CtcpSender("Text5") + FormatIdleTime());
					break;
				case CtcpUtil.Time:
					sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, FormatDateTime());
					break;
				case CtcpUtil.UserInfo:
					sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, _userInfoMessage);
					break;
				case CtcpUtil.Version:
					foreach(var version in _versionMessage.Split(SchumixBase.NewLine))
						sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, version);
					break;
				case CtcpUtil.Source:
					sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, _sourceMessage);
					break;
				case CtcpUtil.ClientInfo:
					sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, _clientInfoMessage);
					break;
				case CtcpUtil.Ping:
					sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, args);
					break;
				default:
					sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, sLConsole.CtcpSender("Text6"), command[0]);
					break;
			}
		}

		/// <summary>
		/// Finger responses normally consist of a message
		/// and the idle time.
		/// </summary>
		/// <value>The Idle time will be automatically appended
		/// to the finger response. This default to the UserInfo message.</value>
		public string FingerResponse
		{
			get
			{
				return _fingerMessage + sLConsole.CtcpSender("Text5") + FormatIdleTime() + ".";
			}
			set
			{
				_fingerMessage = value;
			}
		}

		/// <summary>
		/// A message about the user.
		/// </summary>
		/// <value>Any string which does not exceed the IRC max length.
		/// This defaults to "Thresher Auto-Responder".</value>
		public string UserInfoResponse
		{
			get
			{
				return _userInfoMessage;
			}
			set
			{
				_userInfoMessage = value;
			}
		}

		/// <summary>
		/// The version of the client software.
		/// </summary>
		/// <value>This defaults to "Thresher IRC library 1.0".</value>
		public string VersionResponse
		{
			get
			{
				return _versionMessage;
			}
			set
			{
				_versionMessage = value;
			}
		}

		/// <summary>
		/// Tell others what CTCP commands this client supports.
		/// </summary>
		/// <value>By default it sends a list of all the CTCP commands.</value>
		public string ClientInfoResponse
		{
			get
			{
				return _clientInfoMessage;
			}
			set
			{
				_clientInfoMessage = value;
			}
		}

		/// <summary>
		/// Where to get this client.
		/// </summary>
		/// <value>This can be a complex set of FTP instructions or just a
		/// URL to the client's homepage.</value>
		public string SourceResponse
		{
			get
			{
				return _sourceMessage;
			}
			set
			{
				_sourceMessage = value;
			}
		}

		/// <summary>
		/// For a TimeSpan to show only hours, minutes, and seconds.
		/// </summary>
		/// <returns>A beautified TimeSpan</returns>
		private string FormatIdleTime() 
		{
			var builder = new StringBuilder();
			builder.Append(sSendMessage.IdleTime.Hours + sLConsole.CtcpSender("Text2"));
			builder.Append(sSendMessage.IdleTime.Minutes + sLConsole.CtcpSender("Text3"));
			builder.Append(sSendMessage.IdleTime.Seconds + sLConsole.CtcpSender("Text4"));
			return builder.ToString();
		}

		/// <summary>
		/// Format the current date into date, time, and time zone. Used
		/// by Time replies.
		/// </summary>
		/// <returns>A beautified DateTime</returns>
		private string FormatDateTime() 
		{
			var time = DateTime.Now;
			var builder = new StringBuilder();
			builder.Append(time.ToLongDateString() + SchumixBase.Space);
			builder.Append(time.ToLongTimeString() + SchumixBase.Space);
			builder.Append("(" + TimeZone.CurrentTimeZone.StandardName + ")");
			return builder.ToString();
		}
	}
}