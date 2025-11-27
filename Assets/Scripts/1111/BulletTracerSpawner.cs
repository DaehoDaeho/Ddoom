using UnityEngine;

/// <summary>
/// [설치] 무기 오브젝트(WeaponAnchor 하위 또는 공용 FX 매니저)
/// [핵심] 발사 시 트레이서 프리팹을 만들어, 시작점 -> 끝점으로 짧게 이동 후 제거.
/// [필수] tracerPrefab(내부에 TrailRenderer 또는 LineRenderer 포함)
/// </summary>
public class BulletTracerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject tracerPrefab;
    [SerializeField] private float tracerLife = 0.2f;  // 화면에 머무는 시간
    [SerializeField] private float tracerSpeed = 200.0f; // 1초에 몇 미터 이동하나(선형 보간용)

    /// <summary>
    /// 발사 시 호출: 시작점에서 목표점까지 트레이서를 날린다.
    /// </summary>
    public void SpawnTracer(Vector3 start, Vector3 end)
    {
        if (tracerPrefab == null)
        {
            return;
        }

        GameObject go = Instantiate(tracerPrefab, start, Quaternion.identity);
        StartCoroutine(MoveAndDie(go, start, end));
    }

    private System.Collections.IEnumerator MoveAndDie(GameObject go, Vector3 start, Vector3 end)
    {
        float dist = Vector3.Distance(start, end);
        float duration = Mathf.Max(0.01f, dist / tracerSpeed);

        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / duration);
            Vector3 pos = Vector3.Lerp(start, end, u);
            go.transform.position = pos;
            yield return null;
        }

        // 잠깐 더 남겼다 지우면 트레일이 자연스럽게 사라짐
        yield return new WaitForSeconds(tracerLife);
        Destroy(go);
    }
}
