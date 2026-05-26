using UnityEngine;

[CreateAssetMenu(fileName = "Mod_ClassDiscriminate", menuName = "Boss/Modifier/ClassDiscriminate")]
public class Mod_ClassDiscriminate : Modifier
{
    [Range(0f, 1f)]
    public float perBlock = 0.1f;

    public override void Apply(ChainJudge judge)
    {
        judge.classDiscriminateActive = true;
        judge.classDiscriminatePerBlock = perBlock;
    }
}
