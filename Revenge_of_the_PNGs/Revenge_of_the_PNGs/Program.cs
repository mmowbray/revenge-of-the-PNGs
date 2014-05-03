using System;
using System.Collections.Generic;
using System.Linq;

namespace Revenge_of_the_PNGs
{
	static class Program
	{
		private static Game1 game;

		[STAThread]
		static void Main ()
		{
			game = new Game1 ();
			game.Run ();
		}
	}
}