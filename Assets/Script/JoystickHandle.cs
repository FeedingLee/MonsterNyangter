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
            canvas.transform as RectTransform, screenPosition, null, out Vector2 anchoredPosition);

        joystickBackground.anchoredPosition = anchoredPosition;
        joystickHandle.anchoredPosition = Vector2.zero; // 핸들을 배경의 중앙에 위치시킴        
    }




    void MoveJoystickHandle(Vector2 screenPosition)
    {
        // 터치 위치를 로컬 포인트로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, screenPosition, null, out Vector2 localPoint);

        // 배경의 앵커 포지션과 터치 위치 간의 오프셋 계산
        Vector2 offset = localPoint - joystickBackground.anchoredPosition;

        // 조이스틱 배경의 반지름 계산
        float radius = joystickBackground.sizeDelta.x / 2;

        // 오프셋의 크기가 반지름을 넘으면, 정규화하여 반지름까지 조정
        if (offset.magnitude > radius)
        {
            offset = offset.normalized * radius;
        }

        // 핸들 위치 설정
        joystickHandle.anchoredPosition = offset;
    }


    public Vector2 GetInputVector()
    {
        return inputVec;
    }
}
