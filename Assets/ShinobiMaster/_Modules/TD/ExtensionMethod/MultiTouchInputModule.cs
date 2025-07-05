using UnityEngine;
using UnityEngine.EventSystems;

public class MultiTouchInputModule : StandaloneInputModule
{
    [SerializeField] protected int m_TouchLimitation = 10;
    // Dictionary to store the PointerEventData for each touch ID
    private PointerEventData[] _pointerEventDatas = new PointerEventData[10]; // assuming max of 10 touch points

    public override void Process()
    {
        // Process standard input events (mouse, keyboard, etc.)
        base.Process();

        // Process multi-touch events
        ProcessMultiTouch();
    }

    // Method to process multi-touch inputs
    private void ProcessMultiTouch()
    {
        // Iterate through all active touches
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            // Get or create PointerEventData for the current touch ID
            PointerEventData touchEventData = GetTouchPointerEventData(touch);

            // Process touch press and release events
            ProcessMultipleTouchPress(touchEventData, touch.phase == TouchPhase.Began, touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled);

            // Process movement and drag events
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
            {
                ProcessMove(touchEventData);
                ProcessDrag(touchEventData);
            }

            // Clean up pointer data if the touch has ended
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                RemovePointerData(touchEventData);
            }
        }
    }

    // Get or create PointerEventData for the touch
    private PointerEventData GetTouchPointerEventData(Touch touch)
    {
        if(_pointerEventDatas == null)
        {
            _pointerEventDatas = new PointerEventData[m_TouchLimitation];
        }
        // Ensure that each touch ID has its own PointerEventData
        if (_pointerEventDatas[touch.fingerId] == null)
        {
            _pointerEventDatas[touch.fingerId] = new PointerEventData(eventSystem);
        }

        PointerEventData data = _pointerEventDatas[touch.fingerId];
        data.Reset();
        data.position = touch.position;
        data.delta = touch.deltaPosition;
        data.pointerId = touch.fingerId;
        data.button = PointerEventData.InputButton.Left;

        // Raycast to find the GameObject under the touch position
        eventSystem.RaycastAll(data, m_RaycastResultCache);
        data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        m_RaycastResultCache.Clear();

        return data;
    }

    // Method to process touch press and release
    protected void ProcessMultipleTouchPress(PointerEventData data, bool pressed, bool released)
    {
        GameObject currentOverGo = data.pointerCurrentRaycast.gameObject;

        if (pressed)
        {
            data.eligibleForClick = true;
            data.delta = Vector2.zero;
            data.dragging = false;
            data.useDragThreshold = true;
            data.pressPosition = data.position;
            data.pointerPressRaycast = data.pointerCurrentRaycast;

            DeselectIfSelectionChanged(currentOverGo, data);

            if (data.pointerEnter != currentOverGo)
            {
                HandlePointerExitAndEnter(data, currentOverGo);
                data.pointerEnter = currentOverGo;
            }

            // Execute pointer down event
            GameObject newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, data, ExecuteEvents.pointerDownHandler);

            if (newPressed == null)
            {
                newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
            }

            data.pointerPress = newPressed;
            data.rawPointerPress = currentOverGo;
            data.clickTime = Time.unscaledTime;
            data.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

            if (data.pointerDrag != null)
            {
                ExecuteEvents.Execute(data.pointerDrag, data, ExecuteEvents.initializePotentialDrag);
            }
        }

        if (released)
        {
            // Execute pointer up event
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);

            // Check if it was a click
            GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(data.pointerCurrentRaycast.gameObject);
            if (data.pointerPress == pointerUpHandler && data.eligibleForClick)
            {
                ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);
            }

            data.eligibleForClick = false;
            data.pointerPress = null;
            data.rawPointerPress = null;

            if (data.pointerDrag != null && data.dragging)
            {
                ExecuteEvents.Execute(data.pointerDrag, data, ExecuteEvents.endDragHandler);
            }

            data.dragging = false;
            data.pointerDrag = null;

            // Handle pointer exit
            if (data.pointerEnter != data.pointerCurrentRaycast.gameObject)
            {
                HandlePointerExitAndEnter(data, null);
                data.pointerEnter = null;
            }
        }
    }
}
