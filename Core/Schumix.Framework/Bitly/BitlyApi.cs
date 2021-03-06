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
using System.Web;
using System.Linq;
using System.Xml.Linq;
using Schumix.Framework.Logger;
using Schumix.Framework.Config;
using Schumix.Framework.Extensions;
using Schumix.Framework.Localization;

namespace Schumix.Framework.Bitly
{
	public static class BitlyApi
	{
		private static readonly LocalizationConsole sLConsole = Singleton<LocalizationConsole>.Instance;

		public static BitlyResults ShortenUrl(string longUrl)
		{
			if(ShortUrlConfig.Name.IsNullOrEmpty())
			{
				Log.Error("BitlyApi", sLConsole.GetString("Username missing!"));
				return null;
			}

			if(ShortUrlConfig.ApiKey.IsNullOrEmpty())
			{
				Log.Error("BitlyApi", sLConsole.GetString("Api key missing!"));
				return null;
			}

			string url = string.Format("http://api.bit.ly/shorten?format=xml&version=2.0.1&longUrl={0}&login={1}&apiKey={2}",
			                           HttpUtility.UrlEncode(longUrl), ShortUrlConfig.Name, ShortUrlConfig.ApiKey);

			var resultXml = XDocument.Load(url);
			var x = (from result in resultXml.Descendants("nodeKeyVal") select new BitlyResults
			{
				UserHash = result.Element("userHash").Value,
				ShortUrl = result.Element("shortUrl").Value
			});

			return x.Single();
		}
	}
}