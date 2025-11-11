using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewTile", menuName = "2D/Tiles/Universal Rule Tile")]
public class UniversalRuleTile : RuleTile
{
    private const int DefaultEmptyState = 2;

    // Overrides the default neighbor matching behavior to smoothly blend all tiles together
    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        // Match empty spaces only if they are in the default empty state
        if (tile == null)
        {
            return neighbor == DefaultEmptyState;
        }

        // Match based on the neighbor's relation to this tile
        switch (neighbor)
        {
            // Match if the neighbor is the same as this tile
            case TilingRuleOutput.Neighbor.This: return true;
            // Do not match if the neighbor is different
            case TilingRuleOutput.Neighbor.NotThis: return false;
        }

        // For other cases, fallback to the default rule match
        return base.RuleMatch(neighbor, tile);
    }
}
