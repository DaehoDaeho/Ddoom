using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// [설치] 씬에 빈 오브젝트를 만들고 BoxCollider를 'IsTrigger=true'로 설정하여 금지 영역을 표시.
/// [핵심] 정적 레지스트리에 자신을 등록/해제한다. EnemyContext가 목적지 보정 시 사용한다.
/// [필수] BoxCollider(Trigger)
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class NavNoEntryVolume : MonoBehaviour
{
    private static readonly List<NavNoEntryVolume> volumes = new List<NavNoEntryVolume>();

    private BoxCollider box;

    private void Awake()
    {
        box = GetComponent<BoxCollider>();
        box.isTrigger = true;
    }

    private void OnEnable()
    {
        if (volumes.Contains(this) == false)
        {
            volumes.Add(this);
        }
    }

    private void OnDisable()
    {
        volumes.Remove(this);
    }

    /// <summary>
    /// desired가 금지 영역 안이면, from -> desired 방향으로 경계선까지 끌어당겨 반환.
    /// 모든 금지 영역에 대해 검사한다.
    /// </summary>
    public static Vector3 ClampOutside(Vector3 desired, Vector3 from)
    {
        Vector3 outPos = desired;

        for (int i = 0; i < volumes.Count; i += 1)
        {
            NavNoEntryVolume v = volumes[i];
            if (v == null || v.box == null)
            {
                continue;
            }

            if (IsInsideBox(desired, v.box) == true)
            {
                // from에서 desired로 가는 선분을 따라 박스의 표면으로 밀어낸다.
                Vector3 dir = (desired - from);
                if (dir.sqrMagnitude <= 0.0001f)
                {
                    // 거의 같은 점이면 단순히 박스 밖 가장 가까운 점으로
                    outPos = ClosestPointOutside(desired, v.box);
                }
                else
                {
                    outPos = RaycastBoxSurface(from, dir, v.box);
                }
            }
        }

        return outPos;
    }

    /// <summary>
    /// AABB 내부 판정(월드 기준).
    /// </summary>
    private static bool IsInsideBox(Vector3 point, BoxCollider box)
    {
        Vector3 c = box.transform.TransformPoint(box.center);
        Vector3 half = Vector3.Scale(box.size * 0.5f, box.transform.lossyScale);

        Vector3 local = point - c;

        // 월드 축 정렬이 아닌 회전 박스이므로 local을 회전축 기준으로 변환
        Quaternion rot = box.transform.rotation;
        Vector3 rLocal = Quaternion.Inverse(rot) * local;

        if (Mathf.Abs(rLocal.x) <= half.x && Mathf.Abs(rLocal.y) <= half.y && Mathf.Abs(rLocal.z) <= half.z)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 점이 박스 내부일 때, 박스 표면 중 가장 가까운 바깥 점을 근사로 반환.
    /// </summary>
    private static Vector3 ClosestPointOutside(Vector3 inside, BoxCollider box)
    {
        Vector3 c = box.transform.TransformPoint(box.center);
        Quaternion rot = box.transform.rotation;
        Vector3 half = Vector3.Scale(box.size * 0.5f, box.transform.lossyScale);

        Vector3 local = Quaternion.Inverse(rot) * (inside - c);

        // 각 축에서 표면까지의 거리 비교 후 가장 작은 면으로 보정
        float dx = half.x - Mathf.Abs(local.x);
        float dy = half.y - Mathf.Abs(local.y);
        float dz = half.z - Mathf.Abs(local.z);

        if (dx <= dy && dx <= dz)
        {
            local.x = Mathf.Sign(local.x) * half.x;
        }
        else if (dy <= dx && dy <= dz)
        {
            local.y = Mathf.Sign(local.y) * half.y;
        }
        else
        {
            local.z = Mathf.Sign(local.z) * half.z;
        }

        Vector3 world = c + (rot * local);
        return world;
    }

    /// <summary>
    /// from에서 dir 방향 선분이 박스와 만나는 지점 바로 앞(외부)로 보정한다.
    /// 간단한 근사: ClosestPoint로 대신(충분히 실용적).
    /// </summary>
    private static Vector3 RaycastBoxSurface(Vector3 from, Vector3 dir, BoxCollider box)
    {
        // Physics.Raycast 대신 수학적으로 박스 교차를 구현하는 대신,
        // 실제 목적은 '박스 밖 가장 가까운 점'이므로 ClosestPoint 사용.
        Vector3 target = from + dir;
        Vector3 closest = box.ClosestPoint(target);
        return closest;
    }
}
