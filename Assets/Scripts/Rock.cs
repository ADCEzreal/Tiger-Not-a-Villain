using UnityEngine;
using Controller; 
public class Rock : MonoBehaviour
{
    public float speed = 8f; // 총알 이동 속력
    private Rigidbody rockRigidbody; // 이동에 사용할 리지드바디 컴포넌트

    void Start() {
        // 게임 오브젝트에서 Rigidbody 컴포넌트를 찾아 bulletRigidbody에 할당
        rockRigidbody = GetComponent<Rigidbody>();
        // 리지드바디의 속도 = 앞쪽 방향 * 이동 속력
        rockRigidbody.linearVelocity = transform.forward * speed;

        // 3초 뒤에 자신의 게임 오브젝트 파괴
        Destroy(gameObject, 5f);
    }

    
    void OnCollisionEnter(Collision collision) {
        // Player 태그를 가진 오브젝트와 충돌했는지 확인
        if (collision.collider.CompareTag("Player"))
        {
            // PlayerController 가져오기
            PlayerController playerController = collision.collider.GetComponent<PlayerController>();
            // Rigidbody 가져오기
            Rigidbody playerRb = collision.collider.GetComponent<Rigidbody>();

            if (playerController != null && playerRb != null)
            {
                // 체력 감소
                GameManager.Instance.TakeDamage(1);
                Destroy(gameObject);
            }
        }
    }
}
