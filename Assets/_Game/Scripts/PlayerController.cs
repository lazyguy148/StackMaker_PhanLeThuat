using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;
    private Vector3 destination;

    [SerializeField]
    private bool detectSwipeOnlyAfterRelease = false;

    [SerializeField]
    private float minDistanceForSwipe = 20f;

    public static event System.Action<SwipeDirection> OnSwipe;

    public enum SwipeDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    public float moveSpeed = 5f;

    private void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPosition = touch.position;
                fingerDownPosition = touch.position;
            }

            if (!detectSwipeOnlyAfterRelease && touch.phase == TouchPhase.Moved)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPosition = touch.position;
                DetectSwipe();
            }
        }
    }

    void DetectSwipe()
    {
        if (SwipeDistanceCheckMet())
        {
            if (IsVerticalSwipe())
            {
                var direction = fingerDownPosition.y - fingerUpPosition.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                Debug.Log("Swiped " + direction);
                
                int brickLayerMask = LayerMask.GetMask("BrickLayer"); // Lấy layer mask của layer "BrickLayer"
                RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, Mathf.Infinity, brickLayerMask);
                //if (Physics.Raycast(transform.position, Vector3.down, out hit, 5f, brickLayerMask))
                //{
                    //Debug.Log("Hit Collider: " + hits.collider.name);
                //}
                // Tìm điểm đến cuối cùng của hướng vuốt trong brickLayer
                for (int i = hits.Length - 1; i >= 0; i--)
                {
                    RaycastHit hit = hits[i];
                    if (hit.collider != null)
                    {
                        destination = hit.point; // Lưu vị trí cần đến từ raycast
                        Debug.Log(hit.collider.name);
                        Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.red,3f); // Vẽ đường ray từ player tới vị trí cần đến
                        break; // Thoát khỏi vòng lặp sau khi tìm thấy điểm đến
                    }
                }
            }
            else
            {
                var direction = fingerDownPosition.x - fingerUpPosition.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                Debug.Log("Swiped " + direction);
                RaycastHit hit;
                int brickLayerMask = LayerMask.GetMask("BrickLayer"); // Lấy layer mask của layer "BrickLayer"

                if (Physics.Raycast(transform.position, Vector3.down, out hit, 5f, brickLayerMask))
                {
                    Debug.Log("Hit Collider: " + hit.collider.name);
                }
            }
            fingerUpPosition = fingerDownPosition;
        }
    }

    void SendSwipe(SwipeDirection direction)
    {
        OnSwipe?.Invoke(direction);
    }

    bool IsVerticalSwipe()
    {
        return VerticalMovementDistance() > HorizontalMovementDistance();
    }

    bool SwipeDistanceCheckMet()
    {
        return VerticalMovementDistance() > minDistanceForSwipe || HorizontalMovementDistance() > minDistanceForSwipe;
    }

    float VerticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.y - fingerUpPosition.y);
    }

    float HorizontalMovementDistance()
    {
        return Mathf.Abs(fingerDownPosition.x - fingerUpPosition.x);
    }

    //private void FixedUpdate()
    //{
        // Di chuyển player tới vị trí cần đến
        //MoveToDestination(destination);
    //}

    //private void MoveToDestination(Vector3 destination)
    //{
        // Di chuyển player tới điểm đến bằng cách sử dụng Vector3.MoveTowards()
        //Debug.DrawLine(transform.position, destination, Color.red); // Vẽ đường dẫn từ player tới vị trí cần đến
        //transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    //}
}


