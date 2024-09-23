using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class JoystickController : MonoBehaviour
{
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;
    public Canvas canvas;
    public Camera mainCamera;
    public GameObject player;
    public float moveSpeed = 5f;

    private Vector2 inputVec;
    private bool isJoy;

    void Start()
    {
        joystickBackground.gameObject.SetActive(false);
        joystickHandle.gameObject.SetActive(false);
        isJoy = false;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (!IsTouchingUI(touch))
                {
                    SetJoystickPosition(touch.position);
                    joystickBackground.gameObject.SetActive(true);
                    joystickHandle.gameObject.SetActive(true);
                    isJoy = true;
                }
            }
            else if (isJoy && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary))
            {
                MoveJoystickHandle(touch.position);
                inputVec = (joystickHandle.anchoredPosition / (joystickBackground.sizeDelta.x / 2)).normalized;
            }
            else if (isJoy && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                joystickBackground.gameObject.SetActive(false);
                joystickHandle.gameObject.SetActive(false);
                isJoy = false;
                inputVec = Vector2.zero;
            }
        }
    }

    bool IsTouchingUI(Touch touch)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = touch.position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.name == "Option Button")
            {
                return true;
            }
        }

        return false;
    }

    void SetJoystickPosition(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, screenPosition, canvas.worldCamera, out Vector2 anchoredPosition);
        joystickBackground.anchoredPosition = anchoredPosition;
        joystickHandle.anchoredPosition = Vector2.zero;
    }

    void MoveJoystickHandle(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, screenPosition, canvas.worldCamera, out Vector2 localPoint);

        Vector2 offset = localPoint - joystickBackground.anchoredPosition;
        float radius = joystickBackground.sizeDelta.x / 2;

        if (offset.magnitude > radius)
        {
            offset = offset.normalized * radius;
        }

        joystickHandle.anchoredPosition = offset;
    }

    public Vector2 GetInputVector()
    {
        return inputVec;
    }
}
