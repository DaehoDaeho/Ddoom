using UnityEngine;

public class SimpleInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string message = "상호작용 완료";  // 안내 문구  화면 로그로 확인용

    // 상호작용 요청이 도착했을 때 한 번 실행한다
    // 호출자는 상호작용을 시도한 주체이며 보통 플레이어이다
    public void Interact(GameObject source)
    {
        Debug.Log($"상호작용 대상 반응  발신자  {source.name}  메시지  {message}");
    }
}
