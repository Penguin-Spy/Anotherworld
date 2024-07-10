/* Anotherworld.cs © Penguin_Spy 2024

  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
  This Source Code Form is "Incompatible With Secondary Licenses", as
  defined by the Mozilla Public License, v. 2.0.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;

namespace Anotherworld {
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class Anotherworld : Mod {

	}

	public class AnotherworldModSystem : ModSystem {
		public static bool JustPressed(Keys key) {
			return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
		}

		public override void PostUpdateWorld() {
			if (JustPressed(Keys.P)) {
				TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
			}
		}

		private void TestMethod(int x, int y) {
			Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.YellowGreen, null);

			// Code to test placed here:
			WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(2, 8), TileID.AmberGemspark);
			//WorldGen.TileRunner(2089,267, 8, 8, 121);
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) {
			//int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
			//tasks.Clear();  // works but breaks world (constant runtime errors cause incredible lag)
			tasks.RemoveAll(genpass =>
				!genpass.Name.Equals("Reset")
				&& !genpass.Name.Equals("Terrain")  // both of these makes the world gen work while still leaving it mostly blank

			//&& !genpass.Name.Equals("Spawn Point")
			//&& !genpass.Name.Equals("Final Cleanup")
			);
			tasks.Add(new AnotherworldGenPass("World Gen Tutorial Ores", 100f));
		}
	}

	public class AnotherworldGenPass : GenPass {
		public AnotherworldGenPass(string name, double loadWeight) : base(name, loadWeight) {
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration) {
			progress.Message = "generating things :)";
			int centerX = Main.maxTilesX / 2;
			int centerY = (int)(Main.maxTilesY * 0.3);
			// spawn room
			GenerateRoom(new Point(centerX, centerY), false);
			progress.Set(0.1f);

			for (int x = 1; x <= 4; x++) {
				progress.Set(0.1f * (x + 1));
				GenerateRoom(new Point(centerX + x * 17, centerY), true);
			}

			Main.spawnTileX = centerX;
			Main.spawnTileY = centerY - 5;
		}

		private static void GenerateRoom(Point p, bool door) {
			WorldUtils.Gen(p, new Shapes.Rectangle(18, 12), new Actions.SetTile(TileID.GrayBrick));
			WorldUtils.Gen(new Point(p.X + 1, p.Y + 1), new Shapes.Rectangle(16, 10), new Actions.ClearTile());
			Main.tile[p.X + 1, p.Y + 6].ResetToType(TileID.Torches);
			Main.tile[p.X + 16, p.Y + 6].ResetToType(TileID.Torches);
			if (door) {
				WorldGen.PlaceDoor(p.X + 17, p.Y + 9, 10, 0);
			}
		}
	}
}
