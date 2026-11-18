using System;
using UnityEngine;
using UnityEngine.Tilemaps;

// The export's weighted random Tile type is absent from the installed Tilemap Extras package.
public sealed class ImportedWeightedRandomTile : Tile
{
    [Serializable]
    public struct Entry
    {
        public Sprite Sprite;
        public int Weight;
    }

    public Entry[] Sprites;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        int total = 0;
        foreach (var entry in Sprites) total += Mathf.Max(0, entry.Weight);
        if (total == 0) return;
        uint hash = unchecked((uint)(position.x * 73856093 ^ position.y * 19349663 ^ position.z * 83492791));
        int choice = (int)(hash % (uint)total);
        foreach (var entry in Sprites)
        {
            choice -= Mathf.Max(0, entry.Weight);
            if (choice >= 0) continue;
            tileData.sprite = entry.Sprite;
            return;
        }
    }
}
