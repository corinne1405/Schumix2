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
using System.Xml;
using System.Threading;
using Schumix.Irc;
using Schumix.Framework;
using Schumix.Framework.Extensions;
using Schumix.Framework.Localization;
using Schumix.MantisBTRssAddon.Config;
using Schumix.MantisBTRssAddon.Localization;

namespace Schumix.MantisBTRssAddon
{
	public sealed class MantisBTRss
	{
		private readonly LocalizationManager sLManager = Singleton<LocalizationManager>.Instance;
		private readonly PLocalization sLocalization = Singleton<PLocalization>.Instance;
		private readonly ChannelInfo sChannelInfo = Singleton<ChannelInfo>.Instance;
		private readonly SendMessage sSendMessage = Singleton<SendMessage>.Instance;
		private Thread _thread;
		private readonly string _name;
		private readonly string _url;
		private string _oldbug;
		private string _title;
		private string _link;
		public bool Started { get; private set; }

		public MantisBTRss(string name, string url)
		{
			_name = name;
			_url = url;
			Init();
		}

		private void Init()
		{
			_title = "rss/channel/item/title";
			_link = "rss/channel/item/link";
		}

		public string Name
		{
			get { return _name; }
		}

		public void Start()
		{
			Started = true;
			_thread = new Thread(Update);
			_thread.Start();
		}

		public void Stop()
		{
			Started = false;
			_thread.Abort();
		}

		public void Reload()
		{
			_thread.Abort();
			_thread = new Thread(Update);
			_thread.Start();
		}

		private void Update()
		{
			try
			{
				XmlDocument url;
				string newbug;
				string title;
				string link;

				url = GetUrl();
				if(!url.IsNull())
					_oldbug = BugCode(url);

				while(true)
				{
					try
					{
						if(sChannelInfo.FSelect("mantisbt"))
						{
							url = GetUrl();
							if(url.IsNull())
								continue;

							newbug = BugCode(url);
							if(newbug == "no text")
								continue;

							if(_oldbug != newbug)
							{
								title = Title(url);
								if(title == "no text")
									continue;

								link = Link(url);
								if(link == "no text")
									continue;

								Informations(newbug, title, link);
								_oldbug = newbug;
							}

							Thread.Sleep(RssConfig.QueryTime*1000);
						}
						else 
							Thread.Sleep(1000);
					}
					catch(Exception e)
					{
						Log.Error("MantisBTRss", sLocalization.Exception("Error"), _name, e.Message);
					}
				}
			}
			catch(Exception e)
			{
				Log.Error("MantisBTRss", sLocalization.Exception("Error2"), _name, e.Message);
				Update();
			}
		}

		private XmlDocument GetUrl()
		{
			try
			{
				var rss = new XmlDocument();
				rss.Load(_url);
				return rss;
			}
			catch(Exception e)
			{
				Log.Error("MantisBTRss", sLocalization.Exception("Error"), _name, e.Message);
			}

			return null;
		}

		private string Title(XmlDocument rss)
		{
			var title = rss.SelectSingleNode(_title);
			return title.IsNull() ? "no text" : title.InnerText;
		}

		private string Link(XmlDocument rss)
		{
			var link = rss.SelectSingleNode(_link);
			return link.IsNull() ? "no text" : link.InnerText;
		}

		private string BugCode(XmlDocument rss)
		{
			var title = rss.SelectSingleNode(_title);
			if(title.IsNull())
				return "no text";

			return title.InnerText.Substring(0, title.InnerText.IndexOf(SchumixBase.Colon));
		}

		private void Informations(string bugcode, string title, string link)
		{
			var db = SchumixBase.DManager.QueryFirstRow("SELECT Channel FROM mantisbt WHERE Name = '{0}'", _name);
			if(!db.IsNull())
			{
				string[] channel = db["Channel"].ToString().Split(SchumixBase.Comma);

				foreach(var chan in channel)
				{
					string language = sLManager.GetChannelLocalization(chan);

					if(title.Contains(SchumixBase.Colon.ToString()))
					{
						sSendMessage.SendCMPrivmsg(chan, sLocalization.MantisBTRss("Text", language), _name, bugcode, link);
						sSendMessage.SendCMPrivmsg(chan, sLocalization.MantisBTRss("Text2", language), _name, title.Substring(title.IndexOf(SchumixBase.Colon)+1));
					}

					Thread.Sleep(1000);
				}
			}
		}
	}
}