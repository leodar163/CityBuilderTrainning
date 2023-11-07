using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ResourceSystem.Categories.UI
{
    public class ResourceCategoryButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        private ResourceCategory _category;

        public UnityEvent onSelected;
        public UnityEvent onUnselected;
        
        public static event Action<ResourceCategory> onSelect;
        public static event Action<ResourceCategory> onUnselect;

        private static ResourceCategoryButton s_selectedButton;
        
        public ResourceCategory Category
        {
            get => _category;
            set
            {
                _category = value;
                UpdateDisplay();
            }
        }

        private void Awake()
        {
            _button.onClick.AddListener(Select);
        }

        private void UpdateDisplay()
        {
            _icon.sprite = _category.icon;
        }

        private void Unselect()
        {
            if (s_selectedButton && s_selectedButton != this) return;
            s_selectedButton = null;
            onUnselected.Invoke();
            onUnselect?.Invoke(null);
        }
        
        public void Select()
        {
            if (s_selectedButton != null)
            {
                if (s_selectedButton != this)
                {
                    s_selectedButton.Unselect();
                }
                else
                {
                    Unselect();
                    return;
                }
            }

            s_selectedButton = this;
            onSelected.Invoke();
            onSelect?.Invoke(_category);
        }
    }
}