using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class StringTableTests
{
    // ── 헬퍼 ─────────────────────────────────────────────────────────────
    private static BlockData MakeBlock(string id, string displayName = "", string description = "")
    {
        var data = ScriptableObject.CreateInstance<BlockData>();
        data.id = id;
        data.displayName = displayName;
        data.description = description;
        return data;
    }

    private static BlockTable MakeTable(params BlockData[] items)
    {
        var table = ScriptableObject.CreateInstance<BlockTable>();
        var entries =
            (List<BlockData>)
                typeof(StringTable<BlockData>)
                    .GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(table);
        entries.AddRange(items);
        return table;
    }

    // ── Get ───────────────────────────────────────────────────────────────
    [Test]
    public void Get_ReturnsEntry_WhenIdExists()
    {
        var block = MakeBlock("wa1");
        var table = MakeTable(block);

        Assert.That(table.Get("wa1"), Is.EqualTo(block));
    }

    [Test]
    public void Get_ReturnsNull_WhenIdMissing()
    {
        var table = MakeTable(MakeBlock("wa1"));

        Assert.That(table.Get("none"), Is.Null);
    }

    // ── TryGet ────────────────────────────────────────────────────────────
    [Test]
    public void TryGet_ReturnsTrue_WhenIdExists()
    {
        var block = MakeBlock("ar1");
        var table = MakeTable(block);

        var result = table.TryGet("ar1", out var entry);

        Assert.That(result, Is.True);
        Assert.That(entry, Is.EqualTo(block));
    }

    [Test]
    public void TryGet_ReturnsFalse_WhenIdMissing()
    {
        var table = MakeTable(MakeBlock("ar1"));

        var result = table.TryGet("none", out var entry);

        Assert.That(result, Is.False);
        Assert.That(entry, Is.Null);
    }

    // ── GetName / GetDescription ──────────────────────────────────────────
    [Test]
    public void GetName_ReturnsDisplayName()
    {
        var table = MakeTable(MakeBlock("wa1", displayName: "레온"));

        Assert.That(table.GetName("wa1"), Is.EqualTo("레온"));
    }

    [Test]
    public void GetDescription_ReturnsDescription()
    {
        var table = MakeTable(MakeBlock("wa1", description: "전사 블록"));

        Assert.That(table.GetDescription("wa1"), Is.EqualTo("전사 블록"));
    }

    [Test]
    public void GetName_FallsBackToId_WhenIdMissing()
    {
        var table = MakeTable();

        Assert.That(table.GetName("wa1"), Is.EqualTo("wa1"));
    }

    [Test]
    public void GetDescription_ReturnsEmpty_WhenIdMissing()
    {
        var table = MakeTable();

        Assert.That(table.GetDescription("wa1"), Is.EqualTo(string.Empty));
    }

    // ── All ───────────────────────────────────────────────────────────────
    [Test]
    public void All_ReturnsAllEntries()
    {
        var b1 = MakeBlock("wa1");
        var b2 = MakeBlock("ar1");
        var table = MakeTable(b1, b2);

        Assert.That(table.All, Is.EquivalentTo(new[] { b1, b2 }));
    }

    // ── 방어 로직 ─────────────────────────────────────────────────────────
    [Test]
    public void DuplicateId_LogsWarning()
    {
        var table = MakeTable(MakeBlock("wa1"), MakeBlock("wa1"));

        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("중복 ID.*wa1"));
        _ = table.Get("wa1"); // 캐시 빌드 시점에 경고 발생
    }

    [Test]
    public void NullEntry_IsSkippedGracefully()
    {
        var table = MakeTable(null, MakeBlock("wa1"));

        Assert.That(table.Get("wa1"), Is.Not.Null);
    }

    [Test]
    public void EmptyId_IsSkippedGracefully()
    {
        var table = MakeTable(MakeBlock(""), MakeBlock("wa1"));

        Assert.That(table.Get("wa1"), Is.Not.Null);
        Assert.That(table.Get(""), Is.Null);
    }
}

// ── TableRegistry 통합 테스트 ────────────────────────────────────────────
public class TableRegistryIntegrationTests
{
    [Test]
    public void Instance_IsNotNull()
    {
        Assert.That(
            TableRegistry.Instance,
            Is.Not.Null,
            "Assets/Resources/TableRegistry.asset 이 존재해야 합니다."
        );
    }

    [Test]
    public void Block_Get_ReturnsWarriorBlock()
    {
        Assert.That(
            TableRegistry.Instance.Block.Get("wa1"),
            Is.Not.Null,
            "BlockTable entries에 id='wa1' 블록이 등록되어 있어야 합니다."
        );
    }

    [Test]
    public void JokerCard_Get_ReturnsChain1()
    {
        Assert.That(
            TableRegistry.Instance.JokerCard.Get("ch1"),
            Is.Not.Null,
            "JokerCardTable entries에 id='ch1' 카드가 등록되어 있어야 합니다."
        );
    }

    [Test]
    public void Enemy_AllIsNotEmpty()
    {
        Assert.That(
            TableRegistry.Instance.Enemy.All.Count,
            Is.GreaterThan(0),
            "EnemyTable에 적이 한 개 이상 등록되어 있어야 합니다."
        );
    }
}
