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
using System.IO;
using System.Text;
using System.Collections.Generic;
using Schumix.Framework;
using Schumix.Framework.Config;
using Schumix.Irc;

namespace Schumix
{
	class MainClass
	{
		private static readonly Sender sSender = Singleton<Sender>.Instance;

        /// <summary>
        ///     A Main f�ggv�ny. Itt indul el a debug.
        /// </summary>
        /// <remarks>
        ///     Schumix IRC bot
        ///     <para>
        ///         K�sz�tette Megaxxx �s Jackneill.
        ///     </para>
        /// </remarks>
		private static void Main(string[] args)
		{
			Console.Title = SchumixBase.Title;
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine("[Schumix2]");
			Console.WriteLine("A program leallitasahoz hasznald a <Ctrl+C> parancsot vagy <kikapcs>\n");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine("Keszitette Megax, Jackneill. Schumix Verzio: {0} http://megaxx.info", Verzio.SchumixVerzio);
			Console.WriteLine("==============================================================================");
			Console.WriteLine("");
			Log.Notice("Main", "Rendszer indul...");
			Console.WriteLine("");

			new SchumixBot();
			Console.CancelKeyPress += (sender, e) => { sSender.Quit("Daemon killed."); SchumixBase.time.SaveUptime(); };
		}
	}
}