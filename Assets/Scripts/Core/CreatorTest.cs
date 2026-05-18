using UnityEngine;

// 테스트 씬 전용 — 키 1/2/3으로 직업별 블럭 인스턴스화 확인
public class CreatorTest : MonoBehaviour
{
    [SerializeField]
    CharacterSet characterSet;

    void Spawn(ClassType classType)
    {
        Block block = characterSet.CreateBlock(classType);
        if (block == null)
        {
            Debug.LogWarning($"[CreatorTest] {classType} Creator가 CharacterSet에 할당되지 않음");
            return;
        }
        Debug.Log($"[CreatorTest] {classType} | id={block.data.id} atk={block.data.attackPower}");
    }
}
