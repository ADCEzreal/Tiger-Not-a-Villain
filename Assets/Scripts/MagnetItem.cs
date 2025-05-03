using UnityEngine;

public class MagnetItem : MonoBehaviour
{
    private float lifeTime = 10f;

    void Start()
    {
        Destroy(gameObject, lifeTime); // 자동 사라짐
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Controller.PlayerController playerController = other.GetComponent<Controller.PlayerController>();
            if (playerController != null)
            {
                playerController.ActiveMagnet();
                Destroy(gameObject);
            }
        }
    }
}