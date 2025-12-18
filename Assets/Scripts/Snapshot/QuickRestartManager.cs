using Cinemachine;
using UnityEngine;

public class QuickRestartManager : MonoBehaviour
{
    [System.Serializable]
    private struct Snapshot
    {
        public Vector3 playerPos;   // 플레이어 위치.
        public Quaternion playerRot;    // 플레이어 회전값.
        public float hp;    // 플레이어의 hp.
        public int inMag;   // 장전된 탄 수.
        public int reserve; // 예비 탄약 수.
        public bool hasData;    // 데이터가 유효한지 여부.
    }

    [SerializeField] private Transform playerRoot;
    [SerializeField] private MonoBehaviour healthProviderObj;
    [SerializeField] private MonoBehaviour ammoProviderObj;

    [SerializeField] private KeyCode saveKey = KeyCode.F5;  // 저장에 사용할 입력 키.
    [SerializeField] private KeyCode restartKey = KeyCode.F9;   // 로딩에 사용할 입력 키.

    private IHealthProvider health;
    private IAmmoProvider ammo;

    private Snapshot snapshot;

    private void Awake()
    {
        health = healthProviderObj as IHealthProvider;
        ammo = ammoProviderObj as IAmmoProvider;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(saveKey) == true)
        {
            SaveNow();
        }

        if(Input.GetKeyDown(restartKey) == true)
        {
            RestartNow();
        }
    }

    public void SaveNow()
    {
        if(playerRoot == null)
        {
            return;
        }

        snapshot.playerPos = playerRoot.position;
        snapshot.playerRot = playerRoot.rotation;

        if(health != null)
        {
            snapshot.hp = health.GetCurrent();
        }

        if(ammo != null)
        {
            snapshot.inMag = ammo.GetInMag();
            snapshot.reserve = ammo.GetReserve();
        }

        snapshot.hasData = true;
        Debug.Log("스냅샷 저장 완료!!!");
    }

    public void RestartNow()
    {
        if(snapshot.hasData == false)
        {
            Debug.LogWarning("저장된 데이터가 없습니다.");
            return;
        }

        if(playerRoot == null)
        {
            Debug.LogWarning("플레이어의 Transform 정보가 없습니다.");
            return;
        }

        //playerRoot.position = snapshot.playerPos;
        //playerRoot.rotation = snapshot.playerRot;

        if(health != null)
        {
            float maxHp = health.GetMax();
            float clamped = Mathf.Clamp(snapshot.hp, 0.0f, maxHp);
            health.SetCurrent(clamped);
        }

        if(ammo != null)
        {
            ammo.SetInMag(snapshot.inMag);
            ammo.SetReserve(snapshot.reserve);
        }

        Debug.Log("데이터 복구 성공~");
    }
}
