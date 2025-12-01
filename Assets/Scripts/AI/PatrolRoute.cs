using UnityEngine;

/// <summary>
/// [설치] 빈 오브젝트에 부착하고 자식들에 웨이포인트 트랜스폼을 배치.
/// [핵심] 순찰용 웨이포인트를 순환 방문한다.
/// [필수 연결] 없음(EnemyBrain이 참조)
/// </summary>
public class PatrolRoute : MonoBehaviour
{
    [SerializeField] private Transform[] points; // 순찰 지점 배열
    [SerializeField] private bool loop = true;   // 끝나면 처음으로?

    private int index = 0; // 현재 타겟 인덱스

    public bool HasPoints()
    {
        return points != null && points.Length > 0;
    }

    public Transform GetCurrent()
    {
        if (HasPoints() == false)
        {
            return null;
        }

        return points[index];
    }

    public void MoveNext()
    {
        if (HasPoints() == false)
        {
            return;
        }

        index += 1;

        if (index >= points.Length)
        {
            if (loop == true)
            {
                index = 0;
            }
            else
            {
                index = points.Length - 1;
            }
        }
    }
}
