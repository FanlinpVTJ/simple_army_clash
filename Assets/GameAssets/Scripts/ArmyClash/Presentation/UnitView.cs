using System;
using PoolsUtility;
using SimpleArmyClash.Domain;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleArmyClash.Presentation
{
    public class UnitView : PooledObject, IUnitView, IPointerClickHandler
    {
        private const float MINIMUM_DIRECTION_SQUARED = 0.000001f;
        private static readonly int BASE_COLOR_PROPERTY = Shader.PropertyToID("_BaseColor");
        private static readonly int COLOR_PROPERTY = Shader.PropertyToID("_Color");

        public event Action<int> OnSelected;

        [SerializeField, Tooltip("Required renderers that display the statistics color of this unit.")]
        private Renderer[] _bodyRenderers = Array.Empty<Renderer>();

        [SerializeField, Tooltip("Required renderers for a separate marker showing army allegiance.")]
        private Renderer[] _teamMarkerRenderers = Array.Empty<Renderer>();

        [SerializeField, Tooltip("Required child transform containing the body; used for attack animation.")]
        private Transform _visualRoot;

        [SerializeField, Tooltip("Required selection highlight object. Kept inactive outside formation editing.")]
        private GameObject _selectionMarker;

        private Transform _cachedTransform;
        private MaterialPropertyBlock _materialProperties;
        private Vector3 _originalVisualScale;

        public int Identifier { get; private set; }

        private void Awake()
        {
            _cachedTransform = transform;
            _materialProperties = new MaterialPropertyBlock();
            _originalVisualScale = _visualRoot.localScale;
        }

        public virtual void Configure(UnitState state, Color bodyColor, Color teamColor)
        {
            Identifier = state.Identifier;
            _cachedTransform.position = state.Position;
            _cachedTransform.localScale = Vector3.one * state.Definition.Scale;
            _cachedTransform.rotation = Quaternion.LookRotation(state.ArmyIndex == 0 ? Vector3.forward : Vector3.back);
            _visualRoot.localScale = _originalVisualScale;
            _selectionMarker.SetActive(false);
            ApplyColor(_bodyRenderers, bodyColor);
            ApplyColor(_teamMarkerRenderers, teamColor);
        }

        public virtual void Synchronize(Vector3 position, float visualScale)
        {
            Vector3 direction = position - _cachedTransform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > MINIMUM_DIRECTION_SQUARED)
            {
                _cachedTransform.rotation = Quaternion.LookRotation(direction);
            }

            _cachedTransform.position = position;
            _visualRoot.localScale = _originalVisualScale * visualScale;
        }

        public virtual void PlayAttack(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - _cachedTransform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > MINIMUM_DIRECTION_SQUARED)
            {
                _cachedTransform.rotation = Quaternion.LookRotation(direction);
            }
        }

        public void SetSelected(bool selected)
        {
            if (_selectionMarker.activeSelf != selected)
            {
                _selectionMarker.SetActive(selected);
            }
        }

        public virtual void ResetForPool()
        {
            OnSelected = null;
            _visualRoot.localScale = _originalVisualScale;
            _selectionMarker.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnSelected?.Invoke(Identifier);
            }
        }

        private void ApplyColor(Renderer[] renderers, Color color)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer bodyRenderer = renderers[i];
                bodyRenderer.GetPropertyBlock(_materialProperties);
                _materialProperties.SetColor(BASE_COLOR_PROPERTY, color);
                _materialProperties.SetColor(COLOR_PROPERTY, color);
                bodyRenderer.SetPropertyBlock(_materialProperties);
                _materialProperties.Clear();
            }
        }
    }
}
