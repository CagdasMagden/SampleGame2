using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTarget;
    public Vector3 cameraOffSet;

    public float smoothSpeed = 0.125f;

    void LateUpdate() //nedir ��ren
    {
        Vector3 desiredPosition = playerTarget.position + cameraOffSet;

        transform.position = desiredPosition;
        //transform.LookAt(playerTarget);
        //Vector3 velocity = Vector3.zero;
        //Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position,
        //                                              desiredPosition,
        //                                              ref velocity,
        //                                              smoothSpeed);
    }
    // karakterin tipi sa�a sola d�n�nce flip att��� i�in
    // kamerada k���k bir kayma oluyor her d�n��te bir bakars�n
}
