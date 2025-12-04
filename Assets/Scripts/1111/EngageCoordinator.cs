using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 동시에 공격 가능한 적 수를 제한하고, 플레이어 주변에 원형 슬롯을 배치해
/// 자리 겹침을 줄인다. 씬에 1개만 존재.
/// </summary>
public class EngageCoordinator : MonoBehaviour
{
    public static EngageCoordinator Instance;

    [SerializeField] private Transform target;        // 보통 Player Transform
    [SerializeField] private int maxAttackers = 2;    // 동시에 공격 가능한 수
    [SerializeField] private float ringRadius = 2.5f; // 슬롯 원 반지름
    [SerializeField] private int slotCount = 8;       // 슬롯 개수(원 위 고정 자리)
    [SerializeField] private float keepAliveTimeout = 0.8f; // 갱신 없으면 반납(초)

    private Dictionary<int, int> agentToSlot = new Dictionary<int, int>();   // agentId -> slotIndex
    private Dictionary<int, float> agentKeepAlive = new Dictionary<int, float>(); // agentId -> 남은 시간
    private int currentAttackers = 0; // 토큰 보유자 수

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // 만료된 에이전트 처리(keepAlive)
        List<int> toRemove = null;

        foreach (KeyValuePair<int, float> kv in agentKeepAlive)
        {
            float left = kv.Value - Time.deltaTime;
            if (left <= 0.0f)
            {
                if (toRemove == null)
                {
                    toRemove = new List<int>();
                }
                toRemove.Add(kv.Key);
            }
            else
            {
                agentKeepAlive[kv.Key] = left;
            }
        }

        if (toRemove != null)
        {
            for (int i = 0; i < toRemove.Count; ++i)
            {
                ForceRelease(toRemove[i]);
            }
        }
    }

    /// <summary>
    /// 토큰 요청 또는 갱신. 허가되면 slotIndex와 자리 좌표를 반환한다.
    /// </summary>
    public bool RequestOrUpdate(int agentId, Vector3 agentPos, out int slotIndex, out Vector3 slotWorldPos)
    {
        slotIndex = -1;
        slotWorldPos = agentPos;

        if (target == null)
        {
            return false;
        }

        // 이미 슬롯을 가진 경우(갱신)
        if (agentToSlot.ContainsKey(agentId) == true)
        {
            slotIndex = agentToSlot[agentId];
            slotWorldPos = GetSlotWorldPosition(slotIndex);
            agentKeepAlive[agentId] = keepAliveTimeout;
            return true;
        }

        // 새로운 요청: 토큰 수 제한 확인
        if (currentAttackers >= maxAttackers)
        {
            return false;
        }

        // 비어 있는 슬롯 중 agentPos와 가장 가까운 슬롯 찾기
        int bestSlot = -1;
        float bestDist = float.MaxValue;

        for (int s = 0; s < slotCount; ++s)
        {
            if (IsSlotTaken(s) == true)
            {
                continue;
            }

            Vector3 pos = GetSlotWorldPosition(s);
            float d = Vector3.SqrMagnitude(pos - agentPos);
            if (d < bestDist)
            {
                bestDist = d;
                bestSlot = s;
            }
        }

        if (bestSlot == -1)
        {
            return false;
        }

        // 할당
        agentToSlot[agentId] = bestSlot;
        agentKeepAlive[agentId] = keepAliveTimeout;
        ++currentAttackers;

        slotIndex = bestSlot;
        slotWorldPos = GetSlotWorldPosition(bestSlot);
        return true;
    }

    /// <summary>
    /// 토큰 반납(공격 종료/사망/멀어짐 등).
    /// </summary>
    public void Release(int agentId)
    {
        ForceRelease(agentId);
    }

    private void ForceRelease(int agentId)
    {
        if (agentToSlot.ContainsKey(agentId) == true)
        {
            agentToSlot.Remove(agentId);
            if (currentAttackers > 0)
            {
                --currentAttackers;
            }
        }

        if (agentKeepAlive.ContainsKey(agentId) == true)
        {
            agentKeepAlive.Remove(agentId);
        }
    }

    private bool IsSlotTaken(int slotIndex)
    {
        foreach (KeyValuePair<int, int> kv in agentToSlot)
        {
            if (kv.Value == slotIndex)
            {
                return true;
            }
        }
        return false;
    }

    private Vector3 GetSlotWorldPosition(int slotIndex)
    {
        float angle = (360.0f / Mathf.Max(1, slotCount)) * slotIndex;
        Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0.0f, Mathf.Sin(angle * Mathf.Deg2Rad)) * ringRadius;
        return target.position + offset;
    }
}
