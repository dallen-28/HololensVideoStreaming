using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class CTSliceDragController : MonoBehaviour, IMixedRealityPointerHandler
{
    [Header("References")]
    [SerializeField] private UpdateCTSlice sliceController;

    [Header("Drag Settings")]
    [Tooltip("Meters of vertical drag movement = 1 slice step. Lower = more sensitive.")]
    [SerializeField] private float dragSensitivity = 0.003f;

    [Tooltip("Invert up/down drag direction")]
    [SerializeField] private bool invertDirection = false;

    private bool isDragging = false;
    private Vector3 dragStartPosition;
    private int dragStartSliceIndex;

    private void Start()
    {
        if (sliceController == null)
            sliceController = GetComponent<UpdateCTSlice>();
    }

    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);
    }

    private bool IsInteractingWithThisPanel(MixedRealityPointerEventData eventData)
    {
        if (eventData.Pointer?.Result?.CurrentPointerTarget == null)
            return false;

        Transform hit = eventData.Pointer.Result.CurrentPointerTarget.transform;
        return hit == transform || hit.IsChildOf(transform);
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        if (!IsInteractingWithThisPanel(eventData)) return;

        isDragging = true;
        dragStartPosition = eventData.Pointer.Position;
        dragStartSliceIndex = sliceController.CurrentSliceIndex;
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        if (!isDragging || sliceController == null) return;
        if (!IsInteractingWithThisPanel(eventData)) return;

        Vector3 currentPos = eventData.Pointer.Position;
        Vector3 totalDelta = currentPos - dragStartPosition;

        // Project onto the panel's local up direction
        float localDeltaY = Vector3.Dot(transform.InverseTransformDirection(totalDelta), Vector3.up);

        if (invertDirection)
            localDeltaY = -localDeltaY;

        // Map drag distance directly to slice offset
        int sliceOffset = Mathf.RoundToInt(localDeltaY / dragSensitivity);
        int targetIndex = dragStartSliceIndex + sliceOffset;

        sliceController.SetSliceIndex(targetIndex);
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        if (!IsInteractingWithThisPanel(eventData)) return;

        isDragging = false;
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData) { }
}