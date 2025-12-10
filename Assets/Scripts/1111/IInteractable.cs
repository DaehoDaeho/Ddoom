using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject source);  // 상호작용 호출자 전달. 보통 플레이어 오브젝트가 넘어옴
}
