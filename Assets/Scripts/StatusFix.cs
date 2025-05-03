using UnityEngine;

public class StatusFix : MonoBehaviour
{
     void LateUpdate()
    {
        if (Camera.main != null)
        {
            // 카메라 방향으로 UI가 항상 회전하도록
            transform.forward = Camera.main.transform.forward;
        }
    }
}
