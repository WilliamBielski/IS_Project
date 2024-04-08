using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace IS_Project.Models
{
    public class Sprites : Constants
    {
        //Texture2D sprites;
        public Texture2D hedgeTile; //48x48 tile
        public Texture2D pathTile; //48x48 tile
        public Texture2D topWallTile;
        public Texture2D bottomWallTile;
        public Texture2D leftWallTile;
        public Texture2D rightWallTile;
        public Texture2D redTile;
        public Texture2D redPlayerPiece;
        public Texture2D blueTile;
        public Texture2D bluePlayerPiece;
        public Texture2D minotaur;

        public SpriteFont basicFont;

        public Texture2D redIndicator;
        public Texture2D blueIndicator;

        public Texture2D chokepointNorth;
        public Texture2D chokepointSouth;
        public Texture2D chokepointEast;
        public Texture2D chokepointWest;

        public Texture2D minotaurRangeTile;

        public Sprites()
        {

            // TODO: use this.Content to load your game content here
            hedgeTile = Globals.Content.Load<Texture2D>("hedge-tile");
            pathTile = Globals.Content.Load<Texture2D>("path-tile");
            topWallTile = Globals.Content.Load<Texture2D>("wall-tile-top");
            bottomWallTile = Globals.Content.Load<Texture2D>("wall-tile-bottom");
            leftWallTile = Globals.Content.Load<Texture2D>("wall-tile-left");
            rightWallTile = Globals.Content.Load<Texture2D>("wall-tile-right");
            redTile = Globals.Content.Load<Texture2D>("red-player-tile");
            redPlayerPiece = Globals.Content.Load<Texture2D>("red-player-piece");
            blueTile = Globals.Content.Load<Texture2D>("blue-player-tile");
            bluePlayerPiece = Globals.Content.Load<Texture2D>("blue-player-piece");
            minotaur = Globals.Content.Load<Texture2D>("minotuar-tile");

            basicFont = Globals.Content.Load<SpriteFont>("galleryFont");

            redIndicator = Globals.Content.Load<Texture2D>("red_path_indicator");
            blueIndicator = Globals.Content.Load<Texture2D>("blue_path_indicator");

            chokepointNorth = Globals.Content.Load<Texture2D>("Chokepoint_North");
            chokepointSouth = Globals.Content.Load<Texture2D>("Chokepoint_South");
            chokepointEast = Globals.Content.Load<Texture2D>("Chokepoint_East");
            chokepointWest = Globals.Content.Load<Texture2D>("Chokepoint_West");

            minotaurRangeTile = Globals.Content.Load<Texture2D>("Minorange_tile");
        }
    }
}
