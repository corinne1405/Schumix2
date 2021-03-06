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
using System.Data;
using System.Collections.Generic;
using Schumix.Framework;
using Schumix.Framework.Util;
using Schumix.Framework.Config;
using Schumix.Framework.Extensions;

namespace Schumix.Irc.Ignore
{
	public sealed class IgnoreIrcCommand
	{
		private readonly Utilities sUtilities = Singleton<Utilities>.Instance;
		private readonly List<string> _ignorelist = new List<string>();
		private string _servername;

		public IgnoreIrcCommand(string ServerName)
		{
			_servername = ServerName;
		}

		public bool IsIgnore(string Name)
		{
			return Contains(Name);
		}

		public void LoadSql()
		{
			var db = SchumixBase.DManager.Query("SELECT Command FROM ignore_irc_commands WHERE ServerName = '{0}'", _servername);
			if(!db.IsNull())
			{
				foreach(DataRow row in db.Rows)
				{
					string name = row["Command"].ToString();
					
					if(!Contains(name))
						_ignorelist.Add(name.ToLower());
				}
			}
		}

		public void Add(string Name)
		{
			if(Name.IsNullOrEmpty())
				return;

			var db = SchumixBase.DManager.QueryFirstRow("SELECT 1 FROM ignore_irc_commands WHERE Command = '{0}' And ServerName = '{1}'", sUtilities.SqlEscape(Name.ToLower()), _servername);
			if(!db.IsNull())
				return;

			_ignorelist.Add(Name.ToLower());
			SchumixBase.DManager.Insert("`ignore_irc_commands`(ServerId, ServerName, Command)", IRCConfig.List[_servername].ServerId, _servername, sUtilities.SqlEscape(Name.ToLower()));
		}

		public void Remove(string Name)
		{
			if(Name.IsNullOrEmpty())
				return;

			var db = SchumixBase.DManager.QueryFirstRow("SELECT 1 FROM ignore_irc_commands WHERE Command = '{0}' And ServerName = '{1}'", sUtilities.SqlEscape(Name.ToLower()), _servername);
			if(db.IsNull())
				return;

			_ignorelist.Remove(Name.ToLower());
			SchumixBase.DManager.Delete("ignore_irc_commands", string.Format("Command = '{0}' And ServerName = '{1}'", sUtilities.SqlEscape(Name.ToLower()), _servername));
		}

		public bool Contains(string Name)
		{
			if(Name.IsNullOrEmpty())
				return false;

			return _ignorelist.Contains(Name.ToLower());
		}

		public void Clean()
		{
			_ignorelist.Clear();
		}
	}
}