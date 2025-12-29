using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 각 본의 물리 시뮬레이션에 필요한 데이터를 저장하기 위한 클래스.
/// </summary>
public class BoneNode
{
    public Transform transform;
    public Vector3 currentPos;  // 현재 플레임에서의 월드 좌표 위치.
    public Vector3 prevPos; // 이전 프레임에서의 월드 좌표 위치.
    public Vector3 initialLocalPos; // 초기 로컬 좌표 위치(부모 기준 상대 위치)
    public Quaternion initialLocalRot;  // 초기 로컬 회전값.

    public BoneNode(Transform t)
    {
        transform = t;
        currentPos = t.position;
        prevPos = t.position;
        initialLocalPos = t.localPosition;
        initialLocalRot = t.localRotation;
    }
}

/// <summary>
/// 간단한 스프링본 시뮬레이션 시스템.
/// </summary>
public class SimpleSpringBone : MonoBehaviour
{
    // 강성 계수.
    // 본이 원래 위치로 돌아가려고 하는 힘의 강도.
    // 값이 클수록 원위치로 돌아가는 속도가 빠르고, 작을수록 부드럽게 움직인다.
    [SerializeField] private float stiffness = 0.1f;

    // 감쇠 계수 : 움직임이 시간에 따라 줄어드는 강도.
    // 1.0에 가까울수록 오래 흔들리고, 낮을 수록 빨리 멈춘다.
    [SerializeField] private float damping = 0.9f;

    // 중력 계수 : 본에 적용되는 아래 방향 중력의 강도.
    // 값이 클수록 아래로 처지는 효과가 강해진다.
    [SerializeField] private float gravity = 0.05f;

    // 질량 : 본의 무게.
    // 값이 클수록 힘에 대한 반응이 느려짐.
    [SerializeField] private float mass = 1.0f;

    // 충돌 반경 : 본 주변의 충돌 체크 영역 크기.
    [SerializeField] private float collisionRadius = 0.1f;

    // 충돌 레이어 : 어떤 레이어의 오브젝트에 충돌 처리를 할지 설정.
    [SerializeField] private LayerMask collisionLayers;

    // 스프링 체인의 시작점이 되는 오브젝트의 트랜스폼.
    [SerializeField] private Transform rootBone;

    // 모든 본 노드들을 순서대로 저장.
    private List<BoneNode> boneChain = new List<BoneNode>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 루트 본이 할당되지 않은 경우 오류 메시지 출력.
        if(rootBone == null)
        {
            Debug.LogError("Root Bone Not Assigned!!!");
            return;
        }

        // 루트 본부터 시작하여 모든 자식 본들을 체인에 추가.
        InitializeBoneChain(rootBone);
    }

    /// <summary>
    /// 본 체인 초기화.
    /// 부모/자식 관계를 따라 순회하며 BoneNode 객체 생성.
    /// </summary>
    /// <param name="bone"></param>
    void InitializeBoneChain(Transform bone)
    {
        // 현재 본을 체인에 추가.
        boneChain.Add(new BoneNode(bone));

        // 현재 본의 모든 자식들에 대해 재귀적 호출.
        foreach(Transform child in bone)
        {
            InitializeBoneChain(child);
        }
    }

    /// <summary>
    /// 애니메이션이 모두 적용된 후 스프링본 시뮬레이션 실행.
    /// 매 프레임마다 물리 계산을 수행하고 본의 위치/회전을 업데이트.
    /// </summary>
    void LateUpdate()
    {
        // 본 체인이 비어 있으면 아무것도 하지 않는다.
        if(boneChain.Count == 0)
        {
            return;
        }

        // 첫번째 본은 부모 오브젝트의 트랜스폼을 그대로 따라감.
        // 루트는 외부(애니메이션, 캐릭터 움직임 등)에 의해 제어되므로 물리 시뮬레이션을 적용하지 않음.
        boneChain[0].currentPos = boneChain[0].transform.position;
        boneChain[0].prevPos = boneChain[0].currentPos;

        // 나머지 본들에 대해 스프링 물리 시뮬레이션 적용.
        for(int i=1; i<boneChain.Count; ++i)
        {
            UpdateBone(boneChain[i], boneChain[i - 1]);
        }

        // 계산된 위치와 회전을 실제 트랜스폼에 적용.
        ApplyTransform();
    }

    /// <summary>
    /// 스프링 물리 시뮬레이션.
    /// 속도, 스프링 힘, 중력을 계산하여 본의 새로운 위치를 설정.
    /// </summary>
    /// <param name="bone"></param>
    /// <param name="parent"></param>
    void UpdateBone(BoneNode bone, BoneNode parent)
    {
        // 속도 계산.
        Vector3 velocity = (bone.currentPos - bone.prevPos) * damping;

        // 이전 위치를 현재 위치로 업데이트.
        bone.prevPos = bone.currentPos;

        // 목표 위치 계산.
        // 부모 본을 기준으로 한 원래의 상대 위치를 월드 좌표로 변환.
        // 이 위치가 본이 원래 있어야 할 위치.
        Vector3 targetPos = parent.transform.TransformPoint(bone.initialLocalPos);

        // 스프링 힘 계산.
        Vector3 force = (targetPos - bone.currentPos) * stiffness;

        // 중력 적용.
        force += Vector3.down * gravity * mass;

        // 새로운 위치 계산.
        bone.currentPos += velocity + force;

        // 충돌 체크.
        CheckCollision(bone);

        // 거리 제약.
        ConstraintDistance(bone, parent);
    }

    /// <summary>
    /// 본 주변의 콜라이더와 충돌 검사 및 응답 처리.
    /// </summary>
    /// <param name="bone"></param>
    void CheckCollision(BoneNode bone)
    {
        // 본의 현재 위치를 중심으로 충돌 반경의 구 형태의 영역 내에 collisionLayers에 해당하는 모든 콜라이더를 검색.
        Collider[] colliders = Physics.OverlapSphere(bone.currentPos, collisionRadius, collisionLayers);

        foreach(Collider col in colliders)
        {
            // 가장 가까운 점 찾기.
            Vector3 closest = col.ClosestPoint(bone.currentPos);

            // 본의 위치에서 가장 가까운 점으로의 벡터.
            // 이 벡터의 방향은 Collider 밖으로 향함.
            Vector3 offset = bone.currentPos - closest;

            //충돌 판정 및 위치 보정.
            if(offset.magnitude < collisionRadius)
            {
                // 본을 Collider 표면에서 collisionRadius 만큼 떨어진 위치로 이동.
                bone.currentPos = closest + offset.normalized * collisionRadius;
            }
        }
    }


    /// <summary>
    /// 거리 제약:부모 본과 자식 본 사이의 거리를 원래 거리로 고정.
    /// 본이 늘어나거나 줄어드는 것을 방지하여 물리적으로 안정적인 체인 유지.
    /// </summary>
    /// <param name="bone"></param>
    /// <param name="parent"></param>
    void ConstraintDistance(BoneNode bone, BoneNode parent)
    {
        // 원래 거리 계산.
        float originalDist = bone.initialLocalPos.magnitude;

        // 현재 거리와 방향 계산.
        Vector3 direction = bone.currentPos - parent.currentPos;
        float currentDist = direction.magnitude;

        // 거리 보정.
        // 거리가 0에 가까우면 계산 오류를 방지하기 위해 건너뜀.
        if(currentDist > 0.001f)
        {
            // 본의 방향은 유지하면서 거리만 원래대로 조정.
            bone.currentPos = parent.currentPos + direction.normalized * originalDist;
        }
    }

    /// <summary>
    /// 계산된 물리 위치와 회전을 실제 트랜스폼에 적용.
    /// </summary>
    void ApplyTransform()
    {
        for(int i=1; i<boneChain.Count; ++i)
        {
            BoneNode bone = boneChain[i];
            BoneNode parent = boneChain[i - 1];

            // 위치 적용.
            // 시뮬레이션에서 계산된 월드 좌표를 트랜스폼에 직접 설정.
            bone.transform.position = bone.currentPos;

            // 회전 계산 및 적용.
            // 자식 본이 있는 경우에만 회전 계산.
            if(bone.transform.childCount > 0)
            {
                // 원래 방향 벡터 계산.
                // 부모의 로컬 공간에서의 초기 방향을 월드 공간으로 변환.
                Vector3 originalDir = parent.transform.TransformDirection(bone.initialLocalPos.normalized);

                // 현재 방향 벡터 계산.
                // 물리 시뮬레이션 결과로 본이 현재 향하고 있는 방향.
                Vector3 currentDir = (bone.currentPos - parent.currentPos).normalized;

                // 회전 계산 및 적용
                // 방향 벡터의 크기가 유효한지 확인
                if(currentDir.magnitude > 0.001f && originalDir.magnitude > 0.001f)
                {
                    // originalDir에서 currentDir으로의 회전을 계산.
                    // 한 벡터를 다른 벡터로 회전시키는 회전값 설정.
                    Quaternion rotation = Quaternion.FromToRotation(originalDir, currentDir);

                    // 최종 회전 = 방향 회전 * 부모 회전 * 초기 로컬 회전.
                    // 이를 통해 본이 물리 시뮬레이션 방향을 향하면서도 원래의 회전 오프셋을 유지.
                    bone.transform.rotation = rotation * parent.transform.rotation * bone.initialLocalRot;
                }
            }
        }
    }
}
