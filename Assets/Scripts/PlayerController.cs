using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Controller
{
    [RequireComponent(typeof(CreatureMover))]
    public class PlayerController : MonoBehaviour {

        [Header("Character")]
        [SerializeField]
        private string m_HorizontalAxis = "Horizontal";
        [SerializeField]
        private string m_VerticalAxis = "Vertical";
        [SerializeField]
        private KeyCode m_RunKey = KeyCode.LeftShift;
        public PlayerHPUI healthUI; // HP 로직  
        private CreatureMover m_Mover;
        private Rigidbody rb;
        private Vector2 m_Axis;
        private bool m_IsRun;
     
        [Header("Magnet")]
        public float magnetRadius = 5f; // 자석 기능 로직   
        public float magnetPower = 30f;
        private float magnetDuration = 5f;
        private float magnetTimer = 0f;
        private bool isMagnetActive = false; 
        public GameObject magnetAura; // 자석 아우라 
        private Vector3 m_Target;
        private bool isStun = false;
        private float stunDuration = 1.5f; // 기절 지속 시간
        private float stunTimer; // 현재 기절 상태 시간 확인  
        
        [Header("Blink")]
        public float blinkDistance = 5f; // 앞으로 순간이동 로직    
        public int maxBlinkCount = 1;
        private int currentBlinkCount;

        [Header("Cleanse")]
        public GameObject cleanseEffectPrefab; // 이펙트 프리팹
        public AudioClip cleanseSFX;           // 사운드 클립
        public float cleanseEffectYOffset = 0.1f; // 효과 위치 조절
        private AudioSource audioSource; // 소리 재생용
        public float radius = 5f; // 클린즈 범위 
        private int cleanseSkillCount = 0; 
        private int garlicConsumed = 0;
        public Image cleanseIcon;
        public TextMeshProUGUI cleanseCountText;
        private const int maxCleanseCount = 2;
        
        private void Awake()
        {
            m_Mover = GetComponent<CreatureMover>();
        }

        void Start() {
            rb = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();
            currentBlinkCount = maxBlinkCount; // 처음엔 1회 가능
            healthUI.ShowBlinkIcon(true); // 처음에는 보이게
        }

        
        public void ApplyStunEffect() // 가짜마늘 맞았을 때 기절하는 메소드
        {
            isStun = true;
            stunTimer = stunDuration;

            // 기절 표시 
            healthUI.ShowStun(true);
            Debug.Log("호랑이 기절 시작! " + stunDuration + "초 동안 못 움직임");
        }
        public void ActiveMagnet() // 자석 아이템을 발동시키는 메소드 
        {
            isMagnetActive = true;
            magnetTimer = magnetDuration;

             // 자석 오라 활성화
            if (magnetAura != null)
                magnetAura.SetActive(true);

            Debug.Log("자석 활성화!");
        }
        void Update() {
            // 정화 스킬 사용 로직 (언제든 사용 가능)   
            if (Input.GetKeyDown(KeyCode.R) && cleanseSkillCount > 0)
            {
                UseCleanseSkill();
                cleanseSkillCount--;
                Debug.Log("🌀 정화 스킬 사용! 남은 스킬 수: " + cleanseSkillCount);
            }

            //  기절 상태일 경우 입력을 막고 리턴
            if (isStun)
            {
                stunTimer -= Time.deltaTime; // 기절 타이머 시작    

                // 물리 정지
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                if (!rb.IsSleeping()) // 물리가 멈추지 않았다면
                    rb.Sleep();     // 멈춤

                //  입력도 완전히 제거
                m_Axis = Vector2.zero;
                m_IsRun = false;

                //  CreatureMover에 '정지 입력' 전달해서 끊어줌
                Vector3 forwardTarget = Vector3.forward * 10f;
                m_Mover.SetInput(in m_Axis, in forwardTarget, in m_IsRun);

                if (stunTimer <= 0f) // 기절이 풀렸다면 
                {
                    isStun = false;
                    // 기절 표시 끄기
                    healthUI.ShowStun(false);
                    rb.WakeUp(); // 깨움움
                }
                return;
            }
            // 자석 기능 로직 
            if (isMagnetActive)
            {
                magnetTimer -= Time.deltaTime; // 자석 타이머 시작  
                if (magnetTimer <= 0f) // 자석 제한 시간이 끝났다면 
                {
                    isMagnetActive = false;
                    // 오라 비활성화
                    if (magnetAura != null)
                        magnetAura.SetActive(false);
                    Debug.Log("자석 비활성화!");
                }

                AttractGarlics(); 
            }

            // 순간이동 로직    
            if (!isStun && (Input.GetKeyDown(KeyCode.Space)||Input.GetKeyDown(KeyCode.Return)) && currentBlinkCount > 0)
            {
                BlinkForward();
                currentBlinkCount--;

                // 아이콘 숨기기
                if (currentBlinkCount == 0)
                    healthUI.ShowBlinkIcon(false);
            }

            GatherInput();
            SetInput();
        }
        void AttractGarlics() // 마늘 자석 로직 
        {
            // 플레이어를 중심으로 magnetRadius 범위 내의 모든 콜라이더를 가져옴
            Collider[] garlics = Physics.OverlapSphere(transform.position, magnetRadius);
            // 감지된 콜라이더들 중에서 하나씩 반복
            foreach (var col in garlics)
            {
                if (col.tag == "Garlic") // 태그가 Garlic 이라면
                {
                   Rigidbody rb = col.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        // 기존 속도 제거
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;

                        // 직선 방향으로 강하게 밀기
                        Vector3 dir = (transform.position - col.transform.position).normalized;
                        rb.AddForce(dir * magnetPower, ForceMode.VelocityChange);
                    }
                }
            }
        }
        public void RefillBlink() // 순간이동 획득 시   
        {
            if (currentBlinkCount < maxBlinkCount)
            {
                currentBlinkCount++;
                healthUI.ShowBlinkIcon(true);
                healthUI.ShowBlinkMessage("순간이동 충전");
                Debug.Log("순간이동 충전!");
            }
            else
            {
                healthUI.ShowBlinkMessage("이미 충전 상태!");
                Debug.Log("이미 최대 충전 상태!");
            }
        }
        void BlinkForward() // 순간이동 로직    
        {
            // 플레이어의 정면 방향
            Vector3 forward = transform.forward;
            // 이동 위치 계산
            Vector3 blinkTarget = transform.position + forward * blinkDistance;

            // Raycast 충돌 관련은 tag가 아닌 Layer를 사용해야 함.
            RaycastHit hit;
            // 벽을 감지할 레이어 설정 (Wall이라는 레이어만 감지)
            int wallLayerMask = LayerMask.GetMask("Wall");
            // 현재 위치에서 blinkTarget까지 Raycast
            if (Physics.Raycast(transform.position, forward, out hit, blinkDistance, wallLayerMask))
            {
                // 벽에 막혀 있으면, 벽 앞까지만 이동
                blinkTarget = hit.point - forward * 0.3f; // 약간 여유 두기
                Debug.Log("벽에 막힘, 벽 앞까지만 순간이동");
            }
            // 순간이동
            transform.position = blinkTarget;

            Debug.Log("순간이동 발생!");
        }
        public void OnEatGarlicForSkill() // 마늘을 통해 스킬 제한   -> GameManager에서 호출    
        {
            garlicConsumed++;

            // 20개마다 1회 충전, 최대 2회 충전으로 제한
            if (garlicConsumed >= 20)
            {
                garlicConsumed -= 20;
                if (cleanseSkillCount < 2)
                {
                    cleanseSkillCount++;
                    Debug.Log("정화 스킬 충전! 현재 스킬 수: " + cleanseSkillCount);
                }
            }
            UpdateCleanseUI();
        }   
        void UseCleanseSkill() // 주변 클린즈 로직   
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius);

            foreach (var c in hits)
            {
                GameObject obj = c.gameObject;
                if (obj.tag == "FakeGarlic" || obj.tag == "Rock")
                {
                    Destroy(obj);
                }
            }
            // 이펙트 생성
            if (cleanseEffectPrefab != null)
            {
                Vector3 effectPos = transform.position + Vector3.up * cleanseEffectYOffset;
                Instantiate(cleanseEffectPrefab, effectPos, Quaternion.identity);
            }

            // 소리 재생
            if (cleanseSFX != null && audioSource != null)
            {
                audioSource.PlayOneShot(cleanseSFX);
            }
            UpdateCleanseUI();
        }
        void UpdateCleanseUI() // 정화스킬 생길 때 텍스트가 진해지도록   
        {
            // 텍스트 갱신
            cleanseCountText.text = cleanseSkillCount.ToString();

            // 알파값 조절
            Color iconColor = cleanseIcon.color;
            iconColor.a = (cleanseSkillCount > 0) ? 1f : 0.4f;
            cleanseIcon.color = iconColor;
        }
        public void GatherInput()
        {
            m_Axis = new Vector2(Input.GetAxis(m_HorizontalAxis), Input.GetAxis(m_VerticalAxis));
            m_IsRun = Input.GetKey(m_RunKey);
            
        }

        public void BindMover(CreatureMover mover)
        {
            m_Mover = mover;
        }

        public void SetInput()
        {
            if (m_Mover != null)
            {
                // 카메라가 없기 때문에 target은 world 기준 방향으로 고정
                Vector3 forwardTarget = Vector3.forward * 10f; 
                m_Mover.SetInput(in m_Axis, in forwardTarget, in m_IsRun);
            }
        }

        public void Die() {
            // 자신의 게임 오브젝트를 비활성화
            gameObject.SetActive(false);
            Destroy(gameObject);
            // 씬에 존재하는 GameManager 타입의 오브젝트를 찾아서 가져오기
            GameManager gameManager = FindObjectOfType<GameManager>();
            // 가져온 GameManager 오브젝트의 EndGame() 메서드 실행
            gameManager.EndGame();
        }
}
}
