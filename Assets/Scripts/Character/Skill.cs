using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField]
    protected GameObject effectPrefab;

    public abstract void Chain1(Vector3 targetPos, float scaleFactor);
    public abstract void Chain2(Vector3 targetPos, float scaleFactor);
    public abstract void Chain3(Vector3 targetPos, float scaleFactor);
}
