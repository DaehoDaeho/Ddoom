using UnityEngine;

public class PooledObject : MonoBehaviour
{
    [SerializeField] private float autoReturnSeconds = 2.0f;

    private SimplePool ownerPool;
    private bool inUse; // 현재 사용 중인지 여부.
    private float remain;   // 반환까지 걸리는 시간을 측정하기 위한 타이머 변수.

    // Update is called once per frame
    void Update()
    {
        if(inUse == true)
        {
            remain -= Time.deltaTime;

            if(remain <= 0.0f)
            {
                // 풀에 반환 처리.
                ReturnToPool();
            }
        }
    }

    public void Setup(SimplePool pool, float lifeTime)
    {
        ownerPool = pool;
        remain = lifeTime;
        inUse = true;
        gameObject.SetActive(true);
    }

    public void ReturnToPool()
    {
        if(inUse == false)
        {
            return;
        }

        inUse = false;
        gameObject.SetActive(false);

        // 플에 반환 처리.
        if(ownerPool != null)
        {
            ownerPool.Return(this);
        }
    }
}
