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
using System.Collections.Generic;
using Schumix.API;
using Schumix.Framework;
using Schumix.Irc.Commands;
using Schumix.HgRssAddon.Commands;
using Schumix.HgRssAddon.Config;

namespace Schumix.HgRssAddon
{
	public class HgRssAddon : RssCommand, ISchumixAddon
	{
		public static readonly List<HgRss> RssList = new List<HgRss>();

		public void Setup()
		{
			new AddonConfig(Name + ".xml");
			CommandManager.OperatorCRegisterHandler("hg", HandleHg);

			var db = SchumixBase.DManager.Query("SELECT Name, Link, Website FROM hginfo");
			if(db != null)
			{
				for(int i = 0; i < db.Rows.Count; ++i)
				{
					var row = db.Rows[i];
					string nev = row["Name"].ToString();
					string link = row["Link"].ToString();
					string weboldal = row["Website"].ToString();

					var rss = new HgRss(nev, link, weboldal);
					RssList.Add(rss);
				};

				int x = 0;

				foreach(var list in RssList)
				{
					list.Start();
					x++;
				}

				Log.Notice("HgRssAddon", "{0} rss kerult betoltesre.", x);
			}
			else
				Log.Warning("HgRssAddon", "Ures az adatbazis!");
		}

		public void Destroy()
		{
			CommandManager.OperatorCRemoveHandler("hg");

			foreach(var list in RssList)
				list.Stop();
		}

		public void HandlePrivmsg()
		{

		}

		public void HandleNotice()
		{

		}

		public void HandleHelp()
		{
			Help();
		}

		/// <summary>
		/// Name of the addon
		/// </summary>
		public string Name
		{
			get { return "HgRssAddon"; }
		}

		/// <summary>
		/// Author of the addon.
		/// </summary>
		public string Author
		{
			get { return "Megax"; }
		}

		/// <summary>
		/// Website where the addon is available.
		/// </summary>
		public string Website
		{
			get { return "http://www.github.com/megax/Schumix2"; }
		}
	}
}