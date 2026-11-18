using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "2D/Tiles/Advanced Rule Tile")]
public class AdvancedRuleTile : RuleTile<AdvancedRuleTile.Neighbor>
{
	public class Neighbor : TilingRuleOutput.Neighbor
	{
		public const int NotSpecified = 3;

		public const int Specified = 4;

		public const int Nothing = 5;

		public const int Any = 6;
	}

	public TileBase[] specifiedCompare = new TileBase[0];

	public bool checkSelf = true;

	public override bool RuleMatch(int neighbor, TileBase tile)
	{
		return neighbor switch
		{
			1 => Check_This(tile), 
			2 => Check_NotThis(tile), 
			3 => Check_NotSpecified(tile), 
			4 => Check_Specified(tile), 
			5 => Check_Nothing(tile), 
			6 => Check_Any(tile), 
			_ => base.RuleMatch(neighbor, tile), 
		};
	}

	private bool Check_This(TileBase tile)
	{
		return tile == this;
	}

	private bool Check_NotThis(TileBase tile)
	{
		return tile != this;
	}

	private bool Check_NotSpecified(TileBase tile)
	{
		if (tile != this)
		{
			return !specifiedCompare.Contains(tile);
		}
		return false;
	}

	private bool Check_Specified(TileBase tile)
	{
		if (!checkSelf || !(tile == this))
		{
			return specifiedCompare.Contains(tile);
		}
		return true;
	}

	private bool Check_Nothing(TileBase tile)
	{
		return !tile;
	}

	private bool Check_Any(TileBase tile)
	{
		return tile;
	}
}
