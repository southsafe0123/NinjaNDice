using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Reference tới nhân vật mà camera sẽ theo dõi
    public Transform target;

    // Offset để điều chỉnh vị trí của camera
    public Vector3 offset;

    // Tốc độ di chuyển của camera
    public float smoothSpeed = 0.125f;

    // LateUpdate được sử dụng để đảm bảo camera di chuyển sau khi nhân vật đã được di chuyển
    void LateUpdate()
    {
        // Nếu không có target, không làm gì
        if (target == null)
            return;

        // Tính toán vị trí mới của camera
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Cập nhật vị trí của camera
        transform.position = smoothedPosition;
    }
}

