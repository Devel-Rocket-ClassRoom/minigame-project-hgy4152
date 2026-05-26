using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossPattern", menuName = "Boss/BossPattern")]
public class BossPattern : ScriptableObject, IIdentifiable
{
    public string id;
    public string patternName;
    public List<Modifier> passive = new();
    public Modifier[] turnModifiers = new Modifier[5];

    public string Id => id;
}
