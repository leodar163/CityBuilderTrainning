using System;
using System.Collections.Generic;
using GridSystem;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Tilemaps;

namespace TerrainSystem
{
    [Serializable]
    public class TerrainType
    {
        [SerializeField] private LocalizedString _terrainName;
        [SerializeField] private string _id;
        public Tile tile;
        public string id => _id;

        public readonly List<CellData> cells = new();

        public string terrainName => _terrainName.GetLocalizedString();
        public string modifierName => terrainName;

        public virtual void OnAddedToCell(CellData cellData)
        {
            if (cells.Contains(cellData)) return;
            cells.Add(cellData);
            GridManager.PaintTilemap(tile,TileMapType.Terrain, cellData.cellCoordinates);
        }

        public virtual void OnRemovedFromCell(CellData cellData)
        {
            if (!cells.Contains(cellData)) return;
            cells.Remove(cellData);
            GridManager.PaintTilemap(null,TileMapType.Terrain,cellData.cellCoordinates);
        }
        
        /*
        public ToolTipMessage ToToolTipMessage()
        {
            return new ToolTipMessage
            {
                title = terrainName,
                message = $"{_typeDescription}\n\n{_effectDescription}"
            };
        }
        */
    }
}