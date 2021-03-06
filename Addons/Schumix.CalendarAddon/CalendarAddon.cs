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
using System.Threading.Tasks;
using System.Collections.Generic;
using Schumix.Irc;
using Schumix.Irc.Commands;
using Schumix.Framework;
using Schumix.Framework.Irc;
using Schumix.Framework.Logger;
using Schumix.Framework.Config;
using Schumix.Framework.Functions;
using Schumix.Framework.Localization;
using Schumix.CalendarAddon.Config;
using Schumix.CalendarAddon.Commands;

namespace Schumix.CalendarAddon
{
	class CalendarAddon : ISchumixAddon
	{
		public static readonly Dictionary<string, Flood> FloodList = new Dictionary<string, Flood>();
		private readonly LocalizationConsole sLConsole = Singleton<LocalizationConsole>.Instance;
		private readonly LocalizationManager sLManager = Singleton<LocalizationManager>.Instance;
		private readonly IrcBase sIrcBase = Singleton<IrcBase>.Instance;
		private CalendarCommand sCalendarCommand;
		private BirthdayCommand sBirthdayCommand;
		private BanCommand sBanCommand;
		private Calendar _calendar;
#pragma warning disable 414
		private AddonConfig _config;
#pragma warning restore 414
		private string _servername;
		private Ban sBan;

		public void Setup(string ServerName, bool LoadConfig = false)
		{
			_servername = ServerName;
			sCalendarCommand = new CalendarCommand(ServerName);
			sBirthdayCommand = new BirthdayCommand(ServerName);
			sBanCommand = new BanCommand(ServerName);
			sBan = new Ban(ServerName);

			if(IRCConfig.List[ServerName].ServerId == 1 || LoadConfig)
				_config = new AddonConfig(Name, ".yml");

			_calendar = new Calendar(ServerName);
			_calendar.Start();

			sIrcBase.Networks[ServerName].IrcRegisterHandler("PRIVMSG", HandlePrivmsg);
			InitIrcCommand();
			SchumixBase.DManager.Update("banned", string.Format("ServerName = '{0}'", ServerName), string.Format("ServerId = '{0}'", IRCConfig.List[ServerName].ServerId));
			SchumixBase.DManager.Update("birthday", string.Format("ServerName = '{0}'", ServerName), string.Format("ServerId = '{0}'", IRCConfig.List[ServerName].ServerId));
			SchumixBase.DManager.Update("calendar", string.Format("ServerName = '{0}'", ServerName), string.Format("ServerId = '{0}'", IRCConfig.List[ServerName].ServerId));

			if(CleanConfig.Database)
			{
				SchumixBase.sCleanManager.CDatabase.CleanTable("banned");
				SchumixBase.sCleanManager.CDatabase.CleanTable("birthday");
				SchumixBase.sCleanManager.CDatabase.CleanTable("calendar");
			}
		}

		public void Destroy()
		{
			_calendar.Stop();
			sIrcBase.Networks[_servername].IrcRemoveHandler("PRIVMSG",  HandlePrivmsg);
			RemoveIrcCommand();
		}

		public int Reload(string RName, bool LoadConfig, string SName = "")
		{
			try
			{
				switch(RName.ToLower())
				{
					case "config":
						if(IRCConfig.List[_servername].ServerId == 1 || LoadConfig)
							_config = new AddonConfig(Name, ".yml");
						return 1;
					case "command":
						InitIrcCommand();
						RemoveIrcCommand();
						return 1;
				}
			}
			catch(Exception e)
			{
				Log.Error("CalendarAddon", sLConsole.GetString("Reload: ") + sLConsole.GetString("Failure details: {0}"), e.Message);
				return 0;
			}

			return -1;
		}

		private void InitIrcCommand()
		{
			sIrcBase.Networks[_servername].SchumixRegisterHandler("ban",      sBanCommand.HandleBan, CommandPermission.Operator);
			sIrcBase.Networks[_servername].SchumixRegisterHandler("unban",    sBanCommand.HandleUnban, CommandPermission.Operator);
			sIrcBase.Networks[_servername].SchumixRegisterHandler("calendar", sCalendarCommand.HandleCalendar);
			sIrcBase.Networks[_servername].SchumixRegisterHandler("birthday", sBirthdayCommand.HandleBirthday, CommandPermission.Operator);
		}

		private void RemoveIrcCommand()
		{
			sIrcBase.Networks[_servername].SchumixRemoveHandler("ban",        sBanCommand.HandleBan);
			sIrcBase.Networks[_servername].SchumixRemoveHandler("unban",      sBanCommand.HandleUnban);
			sIrcBase.Networks[_servername].SchumixRemoveHandler("calendar",   sCalendarCommand.HandleCalendar);
			sIrcBase.Networks[_servername].SchumixRemoveHandler("birthday",   sBirthdayCommand.HandleBirthday);
		}

		private void HandlePrivmsg(IRCMessage sIRCMessage)
		{
			Task.Factory.StartNew(() =>
			{
				var sMyChannelInfo = sIrcBase.Networks[sIRCMessage.ServerName].sMyChannelInfo;
				var sSender = sIrcBase.Networks[sIRCMessage.ServerName].sSender;
				string channel = sIRCMessage.Channel.ToLower();

				if(sMyChannelInfo.FSelect(IFunctions.Antiflood) && sMyChannelInfo.FSelect(IChannelFunctions.Antiflood, channel))
				{
					string nick = sIRCMessage.Nick.ToLower();

					if(nick == "py-ctcp")
						return;

					if(FloodList.ContainsKey(nick) && FloodList[nick].Channel.ContainsKey(channel))
					{
						if(FloodList[nick].Channel[channel].Piece == CalendarConfig.NumberOfFlooding)
						{
							var time = DateTime.Now;
							if(time.Minute < 30)
								sBan.BanName(nick, channel, sLManager.GetWarningText("RecurrentFlooding", channel, sIRCMessage.ServerName), DateTime.Now.Hour, DateTime.Now.Minute+30);
							else if(time.Minute >= 30)
								sBan.BanName(nick, channel, sLManager.GetWarningText("RecurrentFlooding", channel, sIRCMessage.ServerName), DateTime.Now.Hour+1, DateTime.Now.Minute-30);

							FloodList[nick].Channel[channel].Piece = 0;
							return;
						}
						else
						{
							if(FloodList[nick].Channel[channel].Message >= CalendarConfig.NumberOfMessages)
							{
								sSender.Kick(channel, nick, sLManager.GetWarningText("StopFlooding", channel, sIRCMessage.ServerName));
								FloodList[nick].Channel[channel].Message = 0;
								FloodList[nick].Channel[channel].Piece++;
								return;
							}
						}
					}

					if(FloodList.ContainsKey(nick) && FloodList[nick].Channel.ContainsKey(channel))
						FloodList[nick].Channel[channel].Message++;
					else if(FloodList.ContainsKey(nick) && !FloodList[nick].Channel.ContainsKey(channel))
						FloodList[nick].Channel.Add(channel, new FloodChannelParameter());
					else if(!FloodList.ContainsKey(nick))
					{
						FloodList.Add(nick, new Flood());
						FloodList[nick].Channel.Add(channel, new FloodChannelParameter());
					}
				}
			});
		}

		public bool HandleHelp(IRCMessage sIRCMessage)
		{
			return false;
		}

		/// <summary>
		/// Name of the addon
		/// </summary>
		public string Name
		{
			get { return "CalendarAddon"; }
		}

		/// <summary>
		/// Author of the addon.
		/// </summary>
		public string Author
		{
			get { return Consts.SchumixProgrammedBy; }
		}

		/// <summary>
		/// Website where the addon is available.
		/// </summary>
		public string Website
		{
			get { return Consts.SchumixWebsite; }
		}
	}
}