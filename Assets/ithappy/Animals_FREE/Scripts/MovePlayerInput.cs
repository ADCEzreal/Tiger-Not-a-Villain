using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(CreatureMover))]
    public class MovePlayerInput : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField]
        private string m_HorizontalAxis = "Horizontal";
        [SerializeField]
        private string m_VerticalAxis = "Vertical";
        
        [SerializeField]
        private KeyCode m_RunKey = KeyCode.LeftShift;


        private CreatureMover m_Mover;

        private Vector2 m_Axis;
        private bool m_IsRun;
       

        private Vector3 m_Target;
        

        private void Awake()
        {
            m_Mover = GetComponent<CreatureMover>();
        }

        private void Update()
        {
            GatherInput();
            SetInput();
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
                // m_Mover.SetInput(in m_Axis, in m_Target, in m_IsRun, m_IsJump);
                 // 카메라가 없기 때문에 target은 world 기준 방향으로 고정
                Vector3 forwardTarget = Vector3.forward * 10f; // 그냥 더미값
                m_Mover.SetInput(in m_Axis, in forwardTarget, in m_IsRun);
            }

           
        }
    }
}