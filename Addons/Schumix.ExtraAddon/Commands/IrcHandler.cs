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
using Schumix.Irc;
using Schumix.Irc.Commands;
using Schumix.Framework;

namespace Schumix.ExtraAddon.Commands
{
	public class IrcHandler : CommandInfo
	{
		private readonly SendMessage sSendMessage = Singleton<SendMessage>.Instance;
		private readonly Sender sSender = Singleton<Sender>.Instance;
		private readonly NickInfo sNickInfo = Singleton<NickInfo>.Instance;
		private readonly Functions sFunctions = Singleton<Functions>.Instance;
		protected bool AutoMode = false;
		protected string ModeChannel;

		/// <summary>
		///     Ha a szobában a köszönés funkció be van kapcsolva,
		///     akkor köszön az éppen belépőnek.
		/// </summary>
		protected void HandleJoin()
		{
			if(Network.IMessage.Nick == sNickInfo.NickStorage)
				return;

			if(sFunctions.AutoKick("join"))
				return;

			string channel = Network.IMessage.Channel;

			if(channel.Substring(0, 1) == ":")
				channel = channel.Remove(0, 1);

			if(Network.sChannelInfo.FSelect("mode") && Network.sChannelInfo.FSelect("mode", channel))
			{
				AutoMode = true;
				ModeChannel = channel;
				sSender.NickServStatus(Network.IMessage.Nick);
			}

			if(Network.sChannelInfo.FSelect("koszones") && Network.sChannelInfo.FSelect("koszones", channel))
			{
				var rand = new Random();
				string Koszones = string.Empty;
				switch(rand.Next(0, 12))
				{
					case 0:
						Koszones = "Hello";
						break;
					case 1:
						Koszones = "Csáó";
						break;
					case 2:
						Koszones = "Hy";
						break;
					case 3:
						Koszones = "Szevasz";
						break;
					case 4:
						Koszones = "Üdv";
						break;
					case 5:
						Koszones = "Szervusz";
						break;
					case 6:
						Koszones = "Aloha";
						break;
					case 7:
						Koszones = "Jó napot";
						break;
					case 8:
						Koszones = "Szia";
						break;
					case 9:
						Koszones = "Hi";
						break;
					case 10:
						Koszones = "Szerbusz";
						break;
					case 11:
						Koszones = "Hali";
						break;
					case 12:
						Koszones = "Szeva";
						break;
				}

				if(DateTime.Now.Hour <= 9)
					sSendMessage.SendCMPrivmsg(channel, "Jó reggelt {0}", Network.IMessage.Nick);
				else if(DateTime.Now.Hour >= 20)
					sSendMessage.SendCMPrivmsg(channel, "Jó estét {0}", Network.IMessage.Nick);
				else
				{
					if(IsAdmin(Network.IMessage.Nick))
						sSendMessage.SendCMPrivmsg(channel, "Üdv főnök");
					else
						sSendMessage.SendCMPrivmsg(channel, "{0} {1}", Koszones, Network.IMessage.Nick);
				}
			}
		}

		/// <summary>
		///     Ha ez a funkció be van kapcsolva, akkor
		///     miután a nick elhagyta a szobát elköszön tőle.
		/// </summary>
		protected void HandleLeft()
		{
			if(Network.IMessage.Nick == sNickInfo.NickStorage)
				return;

			if(Network.sChannelInfo.FSelect("koszones") && Network.sChannelInfo.FSelect("koszones", Network.IMessage.Channel))
			{
				var rand = new Random();
				string elkoszones = string.Empty;
				switch(rand.Next(0, 1))
				{
					case 0:
						elkoszones = "Viszlát";
						break;
					case 1:
						elkoszones = "Bye";
						break;
				}

				if(DateTime.Now.Hour >= 20)
					sSendMessage.SendCMPrivmsg(Network.IMessage.Channel, "Jóét {0}", Network.IMessage.Nick);
				else
					sSendMessage.SendCMPrivmsg(Network.IMessage.Channel, "{0} {1}", elkoszones, Network.IMessage.Nick);
			}
		}

		/// <summary>
		///     Ha engedélyezett a ConsolLog, akkor kiírja a Console-ra ha kickelnek valakit.
		/// </summary>
		protected void HandleKick()
		{
			if(Network.IMessage.Info.Length < 5)
				return;

			if(Network.IMessage.Info[3] == sNickInfo.NickStorage)
			{
				if(Network.sChannelInfo.FSelect("rejoin") && Network.sChannelInfo.FSelect("rejoin", Network.IMessage.Channel))
				{
					foreach(var m_channel in Network.sChannelInfo.CLista)
					{
						if(Network.IMessage.Channel == m_channel.Key)
							sSender.Join(m_channel.Key, m_channel.Value);
					}
				}
			}
			else
			{
				if(Network.sChannelInfo.FSelect("parancsok") && Network.sChannelInfo.FSelect("parancsok", Network.IMessage.Channel))
				{
					if(ConsoleLog.CLog)
					{
						string alomany = string.Empty;
						for(int i = 4; i < Network.IMessage.Info.Length; i++)
							alomany += Network.IMessage.Info[i] + " ";

						if(alomany.Substring(0, 1) == ":")
							alomany = alomany.Remove(0, 1);

						Console.WriteLine("{0} kickelte a következő felhasználot: {1} oka: {2}", Network.IMessage.Nick, Network.IMessage.Info[3], alomany);
					}
				}
			}
		}
	}
}