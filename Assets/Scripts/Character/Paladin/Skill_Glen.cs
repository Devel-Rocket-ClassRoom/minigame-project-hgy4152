using System.Collections.Generic;
using UnityEngine;

public class Skill_Glen : Skill
{
    [SerializeField]
    float explosionDuration = 0.5f;

    [SerializeField]
    GameObject passiveMarkPrefab;

    [SerializeField]
    float markYOffset = 1.5f;

    bool _passiveActive;
    readonly List<GameObject> _marks = new List<GameObject>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            Chain1(testPos, 1f);

        if (Input.GetKeyDown(KeyCode.K))
            Chain1(testPos, 1.5f);

        if (Input.GetKeyDown(KeyCode.L))
            Chain1(testPos, 2f);
    }

    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        Explode(targetPos, scaleFactor);

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        Explode(targetPos, scaleFactor);

    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        Explode(targetPos, scaleFactor);

    void Explode(Vector3 pos, float scale)
    {
        if (effectPrefab == null)
            return;
        var go = Instantiate(effectPrefab, pos, Quaternion.identity);
        go.transform.localScale = Vector3.one * scale;
        Destroy(go, explosionDuration);
    }

    public void SetPassiveActive(bool active)
    {
        if (_passiveActive == active)
            return;
        _passiveActive = active;

        foreach (var m in _marks)
            if (m != null)
                Destroy(m);
        _marks.Clear();

        if (!active || passiveMarkPrefab == null)
            return;

        foreach (var ch in FindObjectsOfType<Character>())
        {
            var mark = Instantiate(passiveMarkPrefab, ch.transform);
            mark.transform.localPosition = new Vector3(0f, markYOffset, 0f);
            _marks.Add(mark);
        }
    }

    void OnDestroy()
    {
        foreach (var m in _marks)
            if (m != null)
                Destroy(m);
        _marks.Clear();
    }
}
