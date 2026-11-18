# Sephiria tiles

The project contains 228 imported Tile assets, including 120 Advanced Rule Tiles and three weighted random Tiles.

51 rule tiles that are not referenced by another tile's `specifiedCompare` list are organized for selection:

- `Assets/Art/GroundTiles`: 36 rule tiles with neither `Wall` nor `Roof` in their names.
- `Assets/Art/WallTiles`: 13 rule tiles with `Wall` in their names.
- `Assets/Art/RoofTiles`: 2 rule tiles with `Roof` and no `Wall` in their names.

The remaining 177 assets stay in `Tiles`. Comparison components, mutually referenced rule groups, regular Tiles and weighted random Tiles were not moved. In particular, `Cliff-Badland` is in `GroundTiles`, while all 50 entries in its comparison list remain in their original location. Asset GUIDs and rule contents are preserved. Drag the desired Tile assets into your Tile Palette to paint them onto a scene Tilemap.

Required dependencies are retained:

- `Textures`: five sprite sheets with the original rectangles, pivots, borders and physics shapes; 16 pixels per unit and point filtering. Some restored tiles also use the project's existing sprite sheet under `Assets/Art/Sprites/Tiles`.
- `Prefabs/FakeTileGizmo`: referenced by `Ground-GrasslandWater-Fake`.
- `Prefabs/HiddenWallCollider`: referenced by `Wall-DeepCaveHidden`.
- `Assets/Scripts/Tiles/AdvancedRuleTile.cs`: custom neighbor matching.
- `Assets/Scripts/Tiles/ImportedWeightedRandomTile.cs`: deterministic sprite placement using the imported weights.
- `Assets/Scripts/Tiles/FakeTileGizmo.cs`: component used by the referenced gizmo prefab.
- The project's installed Tilemap Extras package supplies the standard Rule Tile implementation and editor.

The source AssetRipper export omitted fields from 77 native Tiles. 35 were restored from cached scene sprites and 21 from complete definitions in the earlier project. Cached sprite recovery uses identity transforms, white tint and no collision; set collision behavior for your gameplay as needed.

The remaining 21 retain their asset GUIDs and comparison references but have no sprite. `NativeTileRecovery.json` lists restored and unresolved tiles. Some may be intentional invisible markers; no visual data was invented for them.
