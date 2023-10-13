using System;
using GridSystem;
using ToolTipSystem;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Tilemaps;

namespace TerrainSystem
{
    [Serializable]
    public class TerrainType : IToolTipSpeaker
    {
        [SerializeField] private LocalizedString _terrainName;
        [SerializeField] private string _id;
        public Tile tile;
        public string id => _id;
        
        public CellData cell { get; private set; }

        [Header("Description")] 
        [SerializeField] private string _typeDescription = "This is a terrain";
        [SerializeField] private string _effectDescription = "No effect";
        
        public string terrainName => _terrainName.GetLocalizedString();
        public string modifierName => terrainName;

        public virtual void OnAddedToCell(CellData cellData)
        {
            cell = cellData;
            GridManager.PaintTilemap(tile,GridManager.TileMapType.Terrain, cellData.cellCoordinates);
        }

        public virtual void OnRemovedFromCell(CellData cellData)
        {
            if (cell != cellData) return;
            GridManager.PaintTilemap(null,GridManager.TileMapType.Terrain,cellData.cellCoordinates);
            cell = null;
        }

        public void OnRemovedFromCell()
        {
            if (cell != null)
            {
                OnRemovedFromCell(cell);
            }
        }

        public ToolTipMessage ToToolTipMessage()
        {
            return new ToolTipMessage
            {
                title = terrainName,
                message = $"{_typeDescription}\n\n{_effectDescription}"
            };
        }
    }
}