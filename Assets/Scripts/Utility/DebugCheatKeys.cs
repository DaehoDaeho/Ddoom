using UnityEngine;

public class DebugCheatKeys : MonoBehaviour
{
    [SerializeField] private KeyCode healKey = KeyCode.F5;  // 체력 회복에 사용할 키.
    [SerializeField] private KeyCode invincibleKey = KeyCode.F6;    // 무적 상태 키.

    [SerializeField] private Health[] healths;  // Health 컴포넌트를 가지고 있는 모든 오브젝트들을 연결할 배열.

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(healKey) == true)
        {
            if(healths != null)
            {
                for(int i=0; i<healths.Length; ++i)
                {
                    healths[i].SetHPToMax();
                }
            }
        }

        if(Input.GetKeyDown(invincibleKey) == true)
        {
            if (healths != null)
            {
                for (int i = 0; i < healths.Length; ++i)
                {
                    healths[i].ToggleInvincible();
                }
            }
        }
    }
}
