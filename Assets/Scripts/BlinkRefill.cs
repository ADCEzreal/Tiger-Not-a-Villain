using UnityEngine;

public class BlinkRefill : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Controller.PlayerController pController = other.GetComponent<Controller.PlayerController>();
            if (pController != null)
            {
                pController.RefillBlink();
                Destroy(gameObject); // 먹고 사라지기
            }
        }
    }
}