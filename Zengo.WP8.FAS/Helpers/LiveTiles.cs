
#region Usings

using Microsoft.Phone.Shell;
using System;
using System.Linq;

#endregion

namespace Zengo.WP8.FAS.Helpers
{
    public class LiveTiles
    {
        public static void Set(int remainingVotes, int votesCast)
        {
            // Application Tile is always the first Tile, even if it is not pinned to Start.
            ShellTile primaryTile = ShellTile.ActiveTiles.First();

            if (primaryTile != null)
            {
                // set the properties to update for the Application Tile
                // Empty strings for the text values and URIs will result in the property being cleared.
                StandardTileData tileData = new StandardTileData
                {
                    Title = "Fans All Star",
                    BackgroundImage = new Uri("Images/LiveTile/LargeTileIcon.png", UriKind.Relative),
                    Count = 0,

                    BackTitle = "Fans All Star",
                    BackBackgroundImage = new Uri(remainingVotes <= 0 ? "Images/LiveTile/Red.png" : "Images/LiveTile/Green.png", UriKind.Relative),
                    BackContent = votesCast + " votes cast " + remainingVotes + " votes left"
                };

                // Update the Application Tile
                primaryTile.Update(tileData);
            }
        }

        public static void Reset()
        {
            ShellTile primaryTile = ShellTile.ActiveTiles.First();

            if (primaryTile != null)
            {
                StandardTileData tileData = new StandardTileData
                {
                    Title = "Fans All Star",
                    BackgroundImage = new Uri("Images/LiveTile/LargeTileIcon.png", UriKind.Relative),
                    Count = 0,

                    BackBackgroundImage = new Uri("IDontExist", UriKind.Relative),
                    BackContent = string.Empty,
                    BackTitle = string.Empty
                };

                primaryTile.Update(tileData);
            }
        }
    }
}
