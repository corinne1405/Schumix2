/*
 * This file is part of Schumix.
 * 
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
using System.Text;
using Schumix.Irc.Util;
using Schumix.Irc.Commands;
using Schumix.Framework;
using Schumix.Framework.Irc;
using Schumix.Framework.Util;
using Schumix.Framework.Config;
using Schumix.Framework.Platforms;
using Schumix.Framework.Extensions;
using Schumix.Framework.Localization;

namespace Schumix.Irc.Ctcp
{
	public sealed class CtcpSender : CommandInfo
	{
		private readonly LocalizationConsole sLConsole = Singleton<LocalizationConsole>.Instance;
		private readonly Utilities sUtilities = Singleton<Utilities>.Instance;
		private readonly Platform sPlatform = Singleton<Platform>.Instance;
		private readonly IrcBase sIrcBase = Singleton<IrcBase>.Instance;
		private readonly SendMessage sSendMessage;
		private string _clientInfoMessage;
		private string _userInfoMessage;
		private string _versionMessage;
		private string _sourceMessage;
		private string _fingerMessage;

		public CtcpSender(string ServerName) : base(ServerName)
		{
			_userInfoMessage = "Schumix CTCP";		
			_fingerMessage = _userInfoMessage;
			_versionMessage = string.Format("Schumix {0}\n{1} {2}\n{3} {4}", sUtilities.GetVersion(), sLConsole.GetString("Operating System:"), sPlatform.GetOSName(), sLConsole.GetString("Processor:"), sUtilities.GetCpuId());
			_sourceMessage = Consts.SchumixWebsite;
			_clientInfoMessage = sLConsole.GetString("This client supports: UserInfo, Finger, Version, Source, Ping, Time and ClientInfo");
			sSendMessage = sIrcBase.Networks[ServerName].sSendMessage;
		}

		public void CtcpReply(IRCMessage sIRCMessage)
		{
			if(!Rfc2812Util.IsCtcp(sIRCMessage.Args))
				return;

			if(Rfc2812Util.IsValidChannelName(sIRCMessage.Channel))
				return;

			string args = Rfc2812Util.GetCtcp(sIRCMessage.Args);
			string[] split = args.Split(SchumixBase.Space);

			switch(split[0])
			{
				case CtcpUtil.Finger:
					sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, args + SchumixBase.Space + _fingerMessage + sLConsole.GetString(" Idle time ") + FormatIdleTime());
					break;
				case CtcpUtil.Time:
					sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, args + SchumixBase.Space + FormatDateTime());
					break;
				case CtcpUtil.UserInfo:
					sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, args + SchumixBase.Space + _userInfoMessage);
					break;
				case CtcpUtil.Version:
					foreach(var version in _versionMessage.Split(SchumixBase.NewLine))
						sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, args + SchumixBase.Space + version);
					break;
				case CtcpUtil.Source:
					sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, args + SchumixBase.Space + _sourceMessage);
					break;
				case CtcpUtil.ClientInfo:
					sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, args + SchumixBase.Space + _clientInfoMessage);
					break;
				case CtcpUtil.Ping:
					sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, args);
					break;
				default:
					sSendMessage.SendCMCtcpReply(sIRCMessage.Nick, "{0} {1}: {2}", CtcpUtil.ErrorMessage, args, sLConsole.GetString("Is not a supported Ctcp query."));
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
				return _fingerMessage + sLConsole.GetString(" Idle time ") + FormatIdleTime() + SchumixBase.Point;
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
			builder.Append(sSendMessage.IdleTime.Hours + sLConsole.GetString(" Hours, "));
			builder.Append(sSendMessage.IdleTime.Minutes + sLConsole.GetString(" Minutes, "));
			builder.Append(sSendMessage.IdleTime.Seconds + sLConsole.GetString(" Seconds."));
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