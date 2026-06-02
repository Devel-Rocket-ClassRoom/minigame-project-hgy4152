using System.Collections;
using UnityEngine;

public class Skill_Mkq : Skill
{
    [SerializeField]
    float spawnHeight = 8f;

    [SerializeField]
    float laserTravelDuration = 0.18f;

    [SerializeField]
    float laserDuration = 0.4f;

    [SerializeField]
    GameObject bonusUnitPrefab;

    [SerializeField]
    float bonusUnitOffsetX = 1.5f;

    bool _bonusMode;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            Chain1(testPos, 1f);

        if (Input.GetKeyDown(KeyCode.K))
        {
            Chain1(testPos, 1.5f);
            Chain2(testPos, 1.5f);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Chain1(testPos, 2f);
            Chain2(testPos, 2f);
            Chain3(testPos, 2f);
        }
    }

    public void SetBonusMode(bool active) => _bonusMode = active;

    // 1체인: 평상시 가는 레이저, 보너스 공격 시 유닛 이펙트
    public override void Chain1(Vector3 targetPos, float scaleFactor)
    {
        if (_bonusMode)
            StartCoroutine(SpawnBonusUnit(targetPos, scaleFactor));
        else
            FireLaser(targetPos, scaleFactor * 0.5f);
    }

    // 2체인: 중간 레이저 — 메인 공격이므로 보너스 모드 해제
    public override void Chain2(Vector3 targetPos, float scaleFactor)
    {
        _bonusMode = false;
        FireLaser(targetPos, scaleFactor * 1.0f);
    }

    // 3체인: 굵은 레이저 — 메인 공격이므로 보너스 모드 해제
    public override void Chain3(Vector3 targetPos, float scaleFactor)
    {
        _bonusMode = false;
        FireLaser(targetPos, scaleFactor * 1.5f);
    }

    void FireLaser(Vector3 targetPos, float scale)
    {
        if (effectPrefab == null)
            return;
        Vector3 spawnPos = targetPos + new Vector3(0f, spawnHeight, 0f);
        var go = Instantiate(effectPrefab, spawnPos, Quaternion.identity);
        go.transform.localScale = new Vector3(scale, scale, 1f);
        StartCoroutine(MoveLaser(go, spawnPos, targetPos));
    }

    IEnumerator MoveLaser(GameObject go, Vector3 from, Vector3 to)
    {
        float t = 0f;
        while (t < laserTravelDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                yield break;
            go.transform.position = Vector3.Lerp(from, to, t / laserTravelDuration);
            yield return null;
        }
        if (go != null)
        {
            go.transform.position = to;
            Destroy(go, laserDuration);
        }
    }

    // 보너스 유닛 1기 등장 (GameManager가 4회 호출)
    IEnumerator SpawnBonusUnit(Vector3 targetPos, float scaleFactor)
    {
        if (bonusUnitPrefab == null)
            yield break;

        float side = Random.value < 0.5f ? -1f : 1f;
        Vector3 unitPos = targetPos + new Vector3(side * bonusUnitOffsetX, spawnHeight, 0f);
        var unit = Instantiate(bonusUnitPrefab, unitPos, Quaternion.identity);
        unit.transform.localScale = Vector3.one * scaleFactor * 0.8f;
        yield return StartCoroutine(MoveLaser(unit, unitPos, targetPos));
    }
}
