using UnityEngine;

public class InteractRaycaster : MonoBehaviour
{
    [SerializeField] private Camera viewCamera;              // 시야 기준 카메라 참조
    [SerializeField] private float maxDistance = 3.0f;       // 상호작용 가능 거리
    [SerializeField] private LayerMask hitMask = ~0;         // 충돌 검사에 사용할 마스크

    private IInteractable current;                           // 현재 프레임 기준 상호작용 후보  없으면 널

    private void Reset()
    {
        // 카메라 참조 자동 보정
        if (viewCamera == null)
        {
            viewCamera = Camera.main;
        }
    }

    private void Update()
    {
        // 이전 프레임 후보 초기화
        current = null;

        // 카메라 누락 시 진행 중단
        if (viewCamera == null)
        {
            return;
        }

        // 카메라 위치와 정면으로 레이를 생성
        //Ray ray = new Ray(
        //    viewCamera.transform.position,
        //    viewCamera.transform.forward
        //);

        // 충돌 검사 수행  트리거는 무시
        RaycastHit hit;                                      // 충돌 결과 저장 변수
        //bool didHit = Physics.Raycast(
        //    ray,
        //    out hit,
        //    maxDistance,
        //    hitMask,
        //    QueryTriggerInteraction.Ignore
        //);
        bool didHit = PhysicsRaycasterHelper.TryInteractRay(Camera.main, maxDistance, out hit);

        // 적중했다면 부모 계층에서 인터페이스 구현 검색
        if (didHit == true)
        {
            current = hit.collider.GetComponentInParent<IInteractable>();
        }
    }

    public IInteractable GetCurrent()                        // 현재 후보 반환  없으면 널
    {
        return current;
    }
}
