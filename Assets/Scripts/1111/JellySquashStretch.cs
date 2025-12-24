using UnityEngine;

public class JellySquashStretch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _targetTransform; // 젤리 효과를 적용할 대상(없으면 자기 자신)

    [Header("Spring Settings")]
    [SerializeField] private float _springStrength = 60.0f; // 탄성(클수록 빨리 복원)
    [SerializeField] private float _damping = 10.0f;        // 감쇠(클수록 빨리 잦아듦)

    [Header("Shape Settings")]
    [SerializeField] private float _maxScaleOffset = 0.35f; // 스케일 변형 최대치(과하면 튀어 보임)
    [SerializeField] private float _volumeCompensation = 0.8f; // 부피 보존 느낌(0~1 권장)

    private Vector3 _baseScale;        // 원래 스케일
    private Vector3 _scaleOffset;      // 스프링으로 계산된 스케일 오프셋
    private Vector3 _scaleVelocity;    // 스프링 속도(진동용)

    private void Awake()
    {
        if (_targetTransform == null)
        {
            _targetTransform = this.transform;
        }

        _baseScale = _targetTransform.localScale;
        _scaleOffset = Vector3.zero;
        _scaleVelocity = Vector3.zero;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // 1) 스프링(단순화된 2차 시스템): offset이 0으로 돌아가려는 힘 + 감쇠
        //    a = -k * x - c * v
        Vector3 acceleration = (-_springStrength * _scaleOffset) - (_damping * _scaleVelocity);

        _scaleVelocity += acceleration * dt;
        _scaleOffset += _scaleVelocity * dt;

        // 2) 변형치 제한(너무 과하게 찌그러지는 것 방지)
        _scaleOffset.x = Mathf.Clamp(_scaleOffset.x, -_maxScaleOffset, _maxScaleOffset);
        _scaleOffset.y = Mathf.Clamp(_scaleOffset.y, -_maxScaleOffset, _maxScaleOffset);
        _scaleOffset.z = Mathf.Clamp(_scaleOffset.z, -_maxScaleOffset, _maxScaleOffset);

        // 3) 부피 보존 느낌: (x,z) 변화가 있으면 y에 반대로 보정
        float horizontal = (_scaleOffset.x + _scaleOffset.z) * 0.5f;
        float yComp = -horizontal * _volumeCompensation;

        Vector3 finalOffset = new Vector3(_scaleOffset.x, _scaleOffset.y + yComp, _scaleOffset.z);

        _targetTransform.localScale = _baseScale + finalOffset;
    }

    /// <summary>
    /// 젤리에 충격(임펄스)을 추가한다.
    /// - 사용처: 충돌 순간, 착지 순간, 클릭/스킬 히트 등
    /// - 파라미터: localImpulse는 "로컬 축 기준" 변형 방향/세기
    /// - 부작용: 스케일이 일시적으로 변형됨
    /// </summary>
    public void AddImpulse(Vector3 localImpulse)
    {
        // 충격을 속도 쪽에 더해주면, 순간적으로 튕기는 느낌이 잘 난다.
        _scaleVelocity += localImpulse;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 상대 속도가 클수록 더 찌그러지게
        Vector3 relativeVelocity = collision.relativeVelocity;

        float hitPower = relativeVelocity.magnitude;

        // 아래 임펄스는 "예쁜 기본값" 정도야. 오빠 프로젝트 스케일에 맞게 조절하면 됨.
        Vector3 localDir = this.transform.InverseTransformDirection(relativeVelocity.normalized);

        Vector3 impulse = new Vector3(
            -localDir.x * hitPower * 0.02f, // 옆 충격 -> 반대로 눌림
            -Mathf.Abs(localDir.y) * hitPower * 0.03f, // 위아래 충격 -> 눌림
            -localDir.z * hitPower * 0.02f
        );

        AddImpulse(impulse);
    }
}
