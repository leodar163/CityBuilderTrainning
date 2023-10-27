using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GridSystem.UI
{
    public class MapFilterButton : MonoBehaviour
    {
        [SerializeField] private TileMapType _filter;
        [SerializeField] private Button _button;
        [SerializeField] private Color _selectedColor;
        private Color _defaultColor;
        

        protected void Awake()
        {
            _defaultColor = _button.targetGraphic.color;
            _button.onClick.AddListener(() =>
                MapFilter.ShowMapFilter(_filter == MapFilter.CurrentFilter ? TileMapType.Terrain : _filter));
        }

        private void Update()
        {
            _button.targetGraphic.color = MapFilter.CurrentFilter == _filter ? _selectedColor : _defaultColor;
        }

        
    }
}