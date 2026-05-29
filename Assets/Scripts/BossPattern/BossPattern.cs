using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossPattern", menuName = "Boss/BossPattern")]
public class BossPattern : ScriptableObject, IIdentifiable
{
    public string id;
    public string patternName;
    public List<Modifier> passive = new();
    public Modifier[] turnModifiers = new Modifier[5];
    public float[] hpThresholds = { 0.75f, 0.5f, 0.25f, 0f };

    public string Id => id;
}
