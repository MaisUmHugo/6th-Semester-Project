using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PiGame.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Selectable))]
    public sealed class UIButtonPulse : MonoBehaviour,
        ISelectHandler,
        IDeselectHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        [SerializeField, Min(1f)] private float selectedScale = 1.04f;
        [SerializeField, Range(0f, 0.05f)] private float pulseAmount = 0.012f;
        [SerializeField, Min(0.1f)] private float pulseSpeed = 2f;
        [SerializeField, Min(0.1f)] private float transitionSpeed = 14f;

        private Selectable selectable;
        private Vector3 baseScale;
        private bool isSelected;
        private bool isPointerInside;

        private void Awake()
        {
            selectable = GetComponent<Selectable>();
            baseScale = transform.localScale;
        }

        private void OnEnable()
        {
            selectable ??= GetComponent<Selectable>();
            baseScale = transform.localScale;
            isSelected = EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject;
            isPointerInside = false;
        }

        private void Update()
        {
            bool highlighted = selectable != null && selectable.IsInteractable() && (isSelected || isPointerInside);
            float pulse = highlighted
                ? 1f + Mathf.Sin(Time.unscaledTime * Mathf.PI * pulseSpeed) * pulseAmount
                : 1f;
            float scale = highlighted ? selectedScale * pulse : 1f;
            float interpolation = 1f - Mathf.Exp(-transitionSpeed * Time.unscaledDeltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, baseScale * scale, interpolation);
        }

        private void OnDisable()
        {
            transform.localScale = baseScale;
            isSelected = false;
            isPointerInside = false;
        }

        public void OnSelect(BaseEventData eventData) => isSelected = true;
        public void OnDeselect(BaseEventData eventData) => isSelected = false;
        public void OnPointerEnter(PointerEventData eventData) => isPointerInside = true;
        public void OnPointerExit(PointerEventData eventData) => isPointerInside = false;

#if UNITY_EDITOR
        private void OnValidate()
        {
            selectedScale = Mathf.Max(1f, selectedScale);
            pulseAmount = Mathf.Clamp(pulseAmount, 0f, 0.05f);
            pulseSpeed = Mathf.Max(0.1f, pulseSpeed);
            transitionSpeed = Mathf.Max(0.1f, transitionSpeed);
        }
#endif
    }
}
