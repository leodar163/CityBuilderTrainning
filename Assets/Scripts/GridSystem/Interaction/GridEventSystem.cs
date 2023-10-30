using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace GridSystem.Interaction
{
    public class GridEventSystem : Singleton<GridEventSystem>
    {
        [Header("Cursor")]
        [SerializeField] private Transform _cursor;
        private Material _cursorMat;
        public Vector3 cursorPosition => _cursor.position;
        
        //Hover
        private bool _hoveringActivated = true;
        public static bool hoveringActivated { get => Instance._hoveringActivated; set => Instance._hoveringActivated = value;}
        
        
        private static CellData s_hoveredCell;
        public static CellData HoveredCell => s_hoveredCell;
        private static CellData s_lastHoveredCell;
        [Header("Interactors")]
        [SerializeField] private List<MonoBehaviour> _gridInteractors;
        private static IGridInteractor[] s_gridInteractors;
        private static IGridInteractor s_currentInteractor;
        public static IGridInteractor CurrentInteractor => s_currentInteractor;
        [SerializeField] private GridInteractorType _defaultInteractorType;


        private void OnValidate()
        {
            foreach (var ob in _gridInteractors.ToArray())
            {
                if (ob is not IGridInteractor)
                {
                    _gridInteractors.Remove(ob);
                }
            }
        }

        private void Awake()
        {
            s_gridInteractors = new IGridInteractor[_gridInteractors.Count];

            for (int i = 0; i < _gridInteractors.Count; i++)
            {
                if (_gridInteractors[i] is IGridInteractor interactor)
                {
                    s_gridInteractors[i] = interactor;
                }
            }

            _gridInteractors = null;
            
            if (_cursor.TryGetComponent(out SpriteRenderer meshRend))
            {
                _cursorMat = meshRend.material;
            }
        }

        private void Start()
        {
            SwitchInteractor(GridInteractorType.Default);
        }

        private void Update()
        {
            if (_hoveringActivated && GridManager.TryGetCellDataFromMousePosition(out s_hoveredCell))
            {
                _cursor.gameObject.SetActive(true);
                Vector3 newCursorPosition = s_hoveredCell.position;
                newCursorPosition.y = 0.0001f;
                _cursor.position = newCursorPosition;
            }
            else
            {
                s_hoveredCell = null;
                _cursor.gameObject.SetActive(false);
            }
            
            if (s_hoveredCell != null)
            {
                if (s_lastHoveredCell == null || s_lastHoveredCell != s_hoveredCell)
                {
                    s_currentInteractor?.OnHoveredCellChanged(s_hoveredCell);
                }
            }
            else
            {
                if (s_lastHoveredCell != null)
                {
                    s_currentInteractor?.OnHoveredCellChanged(null);
                }
            }

            s_lastHoveredCell = s_hoveredCell;
        }

        private void LateUpdate()
        {
            if (s_currentInteractor is { isActive: false })
            {
                SwitchInteractor(GridInteractorType.Default);
            }
        }

        public static void SwitchInteractor(GridInteractorType type)
        {
            if (!TryGetInteractor(type, out IGridInteractor interactor)) return;

            if (s_currentInteractor != null)
            {
                if (s_currentInteractor == interactor)
                    return;
                s_currentInteractor.Deactivate();

            }
            s_currentInteractor = interactor;
            s_currentInteractor.Activate();
        }

        public static void SwitchDefaultInteractor(GridInteractorType type, bool switchNow = true)
        {
            if (type == GridInteractorType.Default) return;
            
            if (switchNow && s_currentInteractor.type != type) SwitchInteractor(type);

            Instance._defaultInteractorType = type;
        }

        private static bool TryGetInteractor(GridInteractorType type, out IGridInteractor gridInteractor)
        {
            if (type == GridInteractorType.Default) type = Instance._defaultInteractorType;
            
            foreach (var interactor in s_gridInteractors)
            {
                if (interactor.type == type)
                {
                    gridInteractor = interactor;
                    return true;
                } 
            }

            gridInteractor = null;
            return false;
        }

        public static void PaintCursor(Color color)
        {
            Instance._cursorMat.color = color;
        }
    }
}