using System.Collections.Generic;
using UnityEngine;

namespace TestHM
{
    public enum LayerOrientation { Horizontal, Vertical }

    [System.Serializable]
    public class LayerDimensions
    {
        [Min(0)] public int rows = 0;
        [Min(0)] public int columns = 0;
    }

    [System.Serializable]
    public class LayerState
    {
        public LayerOrientation Orientation;
        public Queue<Sprite> SpriteBag;
        public readonly List<TileInteraction> Tiles = new List<TileInteraction>();
        public int TilesRemaining;
        public int TotalTiles;
        public int TilesMatched;
        public bool IsUnlocked;
        public bool IsCompleted;
        public int Rows;
        public int Columns;
    }

}
