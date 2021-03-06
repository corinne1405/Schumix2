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
using System.IO;
using Schumix.Config.Util;
using Schumix.Config.Logger;
using Schumix.Config.CopyTo;

namespace Schumix.Config
{
	sealed class ConfigBase
	{
		private readonly Utilities sUtilities = Singleton<Utilities>.Instance;

		public ConfigBase()
		{
			Log.Notice("ConfigBase", "Config started.");
		}

		~ConfigBase()
		{
			Log.Debug("ConfigBase", "~ConfigBase()");
		}

		public void Clean(string Schumix2Dir, string AddonsDir, string ConfigDir)
		{
			Log.Notice("ConfigBase", "Copy new files.");
			new Copy(Schumix2Dir, AddonsDir, ConfigDir);

			if(Directory.Exists(Schumix2Dir))
			{
				Log.Notice("ConfigBase", "Clean directorys.");
				sUtilities.ClearAttributes(Schumix2Dir);
				Directory.Delete(Schumix2Dir, true);
			}
		}
	}
}