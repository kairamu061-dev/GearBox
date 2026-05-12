using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MysteryEventUI : MonoBehaviour
{
    [SerializeField] GameObject overlay;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text descText;
    [SerializeField] Transform choiceRoot;
    [SerializeField] Button choicePrefab;
    [SerializeField] Button btnClose;

    System.Action onClosed;

    public void Show(System.Action onClose)
    {
        onClosed = onClose;
        var evt = MysteryEventTable.Roll();
        titleText.text = evt.title;
        descText.text  = evt.description;

        foreach (Transform c in choiceRoot) Destroy(c.gameObject);

        if (evt.choices != null && evt.choices.Count > 0)
        {
            btnClose.gameObject.SetActive(false);
            foreach (var choice in evt.choices)
            {
                var c = choice;
                var btn = Instantiate(choicePrefab, choiceRoot);
                btn.GetComponentInChildren<TMP_Text>().text = c.label;
                btn.onClick.AddListener(() => { c.effect?.Invoke(); Close(); });
            }
        }
        else
        {
            btnClose.gameObject.SetActive(true);
            evt.effect?.Invoke();
        }

        overlay.SetActive(true);
    }

    void Close()
    {
        overlay.SetActive(false);
        onClosed?.Invoke();
    }
}

public static class MysteryEventTable
{
    public static MysteryEvent Roll()
    {
        float r = Random.value * 100f;
        if      (r < 25) return TowerChoice();
        else if (r < 30) return RelicFound();
        else if (r < 50) return ScrapFound();
        else if (r < 60) return Blueprint();
        else if (r < 75) return Trap();
        else if (r < 85) return MysteryPart();
        else if (r < 95) return HpRecovery();
        else             return Nothing();
    }

    static MysteryEvent TowerChoice() => new()
    {
        title = "廃棄タワーを発見",
        description = "錆びついたタワーが3基転がっている。1つ持っていけそうだ。",
        choices = new List<MysteryChoice>
        {
            new() { label = "蒸気砲をもらう",   effect = () => GainTower("steam_cannon") },
            new() { label = "電磁砲塔をもらう", effect = () => GainTower("electro_turret") },
            new() { label = "何も取らない",     effect = null },
        }
    };

    static MysteryEvent RelicFound() => new()
    {
        title = "謎の部品を発見",
        description = "用途不明の機械部品。持っていくと何か役に立つかもしれない。",
        effect = () => { /* TODO: RelicData ドロップ */ },
    };

    static MysteryEvent ScrapFound()
    {
        int amount = Random.Range(30, 81);
        return new()
        {
            title = "スクラップの山を発見",
            description = $"廃材の山を漁ったら {amount} ⚙ 手に入った。",
            effect = () => RunManager.Instance.AddScrap(amount),
        };
    }

    static MysteryEvent Blueprint() => new()
    {
        title = "設計図を発見",
        description = "ボロボロの設計図が落ちていた。解読すれば合成レシピが分かるかもしれない。",
        effect = () => { /* TODO: SynthesisRecipe 解放 */ },
    };

    static MysteryEvent Trap()
    {
        int loss = Random.Range(20, 51);
        loss = Mathf.Min(loss, RunManager.Instance.Scrap);
        return new()
        {
            title = "廃材の罠",
            description = $"床が抜けた！スクラップを {loss} ⚙ 失った。",
            effect = () => RunManager.Instance.SpendScrap(loss),
        };
    }

    static MysteryEvent MysteryPart() => new()
    {
        title = "謎の機械",
        description = "怪しい機械を触ったら所持タワーが改造された。",
        effect = () =>
        {
            var inv = RunManager.Instance.TowerInventory;
            if (inv.Count > 0 && inv[Random.Range(0, inv.Count)].level < 3)
                inv[Random.Range(0, inv.Count)].level++;
        },
    };

    static MysteryEvent HpRecovery() => new()
    {
        title = "廃兵の遺産",
        description = "古い救急キットを発見。戦車のHPが全回復した。",
        effect = () => RunManager.Instance.Heal(RunManager.Instance.MaxHp),
    };

    static MysteryEvent Nothing() => new()
    {
        title = "何もなかった",
        description = "探索したが何も見つからなかった。",
        effect = null,
    };

    static void GainTower(string id)
    {
        var data = Resources.Load<TowerData>($"Towers/{id}");
        if (data != null) RunManager.Instance.AddTower(new TowerInstance(data));
    }
}

public class MysteryEvent
{
    public string title;
    public string description;
    public System.Action effect;
    public List<MysteryChoice> choices;
}

public class MysteryChoice
{
    public string label;
    public System.Action effect;
}
