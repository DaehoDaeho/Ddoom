using UnityEngine;

/// <summary>
/// [설치] 프로젝트 창에서 우클릭 -> Create -> Recoil Pattern Curve 로 에셋 생성
/// [핵심] 시간 또는 발수(index)에 따라 X/Y 반동 각도를 곡선으로 제공.
/// 쉬운 버전: '탄 수' 기반 인덱스로 샘플링.
/// </summary>
[CreateAssetMenu(menuName = "FPS/Recoil Pattern Curve")]
public class RecoilPatternCurve : ScriptableObject
{
    [Header("Pitch(위/아래) 반동 곡선")]
    [SerializeField] private AnimationCurve pitchByShot = AnimationCurve.Linear(0.0f, 1.0f, 30.0f, 2.0f);
    // 예: 1발째 1도, 30발째 2도

    [Header("Yaw(좌/우) 반동 곡선")]
    [SerializeField] private AnimationCurve yawByShot = AnimationCurve.EaseInOut(0.0f, -0.3f, 30.0f, 0.3f);
    // 예: 좌우로 조금씩 바뀌는 패턴

    /// <summary>
    /// n번째 탄(1부터 시작 권장)에서 줄 반동 각도(Pitch/Yaw)를 돌려준다.
    /// </summary>
    public Vector2 SampleKickForShot(int shotIndex)
    {
        if (shotIndex < 1)
        {
            shotIndex = 1;
        }

        float x = yawByShot.Evaluate((float)shotIndex);
        float y = pitchByShot.Evaluate((float)shotIndex);
        return new Vector2(x, y);
    }
}
