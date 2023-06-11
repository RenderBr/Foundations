using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Foundations.Extensions
{
	public static class ITileExtensions
	{
		/// <summary>
		/// Determines whether a tile is empty.
		/// </summary>
		/// <returns></returns>
		public static bool IsEmpty(this ITile tile)
		{
			return tile == null || ((!tile.active() || !Main.tileSolid[tile.type]) && tile.liquid == 0);
		}
	}
}
