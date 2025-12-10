using UnityEngine;

[RequireComponent(typeof(InteractRaycaster))]
public class PlayerStateMachine : MonoBehaviour
{
    // 상태 값은 정지  이동  상호작용으로 구성된다
    private enum State
    {
        Idle,                                                // 정지 상태
        Move,                                                // 이동 상태
        Interact                                             // 상호작용 상태
    }

    [SerializeField] private KeyCode interactKey = KeyCode.E; // 상호작용 키  기본값 이 키

    [SerializeField] private PlayerMotor motor;               // 기존 이동 모듈 참조  오늘은 수정하지 않음
    [SerializeField] private InteractRaycaster interactor;    // 정면 레이 탐지 모듈 참조

    private State state = State.Idle;                         // 현재 상태  시작 시 정지

    private void Reset()
    {
        // 같은 오브젝트에 붙은 탐지 모듈 자동 연결
        if (interactor == null)
        {
            interactor = GetComponent<InteractRaycaster>();
        }
    }

    private void Awake()
    {
        // 누락 대비 재확인
        if (interactor == null)
        {
            interactor = GetComponent<InteractRaycaster>();
        }
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");             // 가로 입력 값
        float y = Input.GetAxisRaw("Vertical");               // 세로 입력 값
        Vector2 moveInput = new Vector2(x, y);                // 이동 입력 벡터

        bool hasMove = moveInput.sqrMagnitude > 0.0f;         // 이동 입력 존재 여부
        bool interactPressed = Input.GetKeyDown(interactKey) == true; // 상호작용 키 눌림 여부

        IInteractable target =                                 // 현재 프레임 후보  없으면 널
            interactor != null ? interactor.GetCurrent() : null;

        // 상태 전이 판단
        switch (state)
        {
            case State.Idle:
                {
                    // 상호작용 가드 충족 시 전이
                    if (interactPressed == true && target != null)
                    {
                        state = State.Interact;
                    }
                    // 이동 입력 발생 시 전이
                    else if (hasMove == true)
                    {
                        state = State.Move;
                    }
                    break;
                }
            case State.Move:
                {
                    // 이동 중 상호작용도 허용
                    if (interactPressed == true && target != null)
                    {
                        state = State.Interact;
                    }
                    // 입력 소멸 시 정지로 전이
                    else if (hasMove == false)
                    {
                        state = State.Idle;
                    }
                    break;
                }
            case State.Interact:
                {
                    // 대상 존재 시 한 번 실행 후 정지 복귀
                    if (target != null)
                    {
                        target.Interact(gameObject);
                    }
                    state = State.Idle;
                    break;
                }
        }

        // 상태 실행  이동은 이동 상태에서만 호출
        //if (state == State.Move)
        //{
        //    if (motor != null)
        //    {
        //        motor.Move(moveInput);                        // 기존 모듈 호출  시그니처가 다르면 이 한 줄만 교체
        //    }
        //}
    }
}
