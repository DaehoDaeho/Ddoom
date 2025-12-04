using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 이동 속도에 비례해 상체에 아주 작은 기울기/흔들림을 준다.
/// 과도한 흔들림은 피로하니 값은 작게 유지.
/// </summary>
public class TorsoMoveSway : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;   // 속도 정보
    [SerializeField] private Transform torsoRoot;  // 상체 루트(모델 루트 자식 권장)
    [SerializeField] private float forwardLeanMaxDeg = 5.0f;
    [SerializeField] private float sideSwayMaxDeg = 3.0f;
    [SerializeField] private float swaySpeed = 6.0f;

    private float phase = 0.0f; // 흔들림 위상(시간 누적)

    private void Reset()
    {
        agent = GetComponentInParent<NavMeshAgent>();
    }

    private void Update()
    {
        if (agent == null || torsoRoot == null)
        {
            return;
        }

        float speed = agent.velocity.magnitude; // 현재 속도
        phase += Time.deltaTime * (swaySpeed + speed);

        // 앞 숙임: 속도 비례
        float forwardLean = Mathf.Clamp(speed * 0.5f, 0.0f, forwardLeanMaxDeg);

        // 좌우 스웨이: 속도에 조금만 영향, 사인파로 미세 흔들림
        float side = Mathf.Sin(phase) * Mathf.Min(sideSwayMaxDeg, 1.0f + speed * 0.2f);

        Quaternion rot = Quaternion.Euler(forwardLean, 0.0f, side);
        torsoRoot.localRotation = rot;
    }
}
