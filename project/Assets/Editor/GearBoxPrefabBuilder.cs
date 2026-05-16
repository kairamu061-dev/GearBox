using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public static class GearBoxPrefabBuilder
{
    const string PrefabPath = "Assets/Prefabs";
    const string SOPath     = "Assets/ScriptableObjects";

    // ────────────────────────────────────────────
    // エントリポイント
    // ────────────────────────────────────────────
    public static void BuildAll()
    {
        EnsureDirs();
        RegisterTags("Player", "Enemy", "EnemyProjectile", "Obstacle");
        BuildRuntimePrefabs();
        BuildUIPrefabs();
        BuildScriptableObjects();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[GearBox] プレハブ・ScriptableObject の生成が完了しました。");
    }

    static void RegisterTags(params string[] tags)
    {
        var tagManager = new SerializedObject(
            AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset"));
        var tagsProp = tagManager.FindProperty("tags");
        foreach (var tag in tags)
        {
            bool exists = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
                if (tagsProp.GetArrayElementAtIndex(i).stringValue == tag)
                { exists = true; break; }
            if (exists) continue;
            tagsProp.arraySize++;
            tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
        }
        tagManager.ApplyModifiedProperties();
    }

    static void EnsureDirs()
    {
        foreach (var p in new[]{
            PrefabPath, $"{PrefabPath}/UI", $"{PrefabPath}/Enemy",
            $"{PrefabPath}/Battle", $"{SOPath}/Towers", $"{SOPath}/Enemies" })
        {
            if (!Directory.Exists(p)) Directory.CreateDirectory(p);
        }
    }

    // ────────────────────────────────────────────
    // ランタイムプレハブ
    // ────────────────────────────────────────────
    public static void BuildRuntimePrefabs()
    {
        BuildTankPrefab();
        BuildProjectilePrefab("EnemyProjectile");
        BuildScrapObjectPrefab();
        BuildEnemyPrefab("EnemyChaser",     Color.red,    AIType.Chaser);
        BuildEnemyPrefab("EnemyTurret",     Color.yellow, AIType.Turret);
        BuildEnemyPrefab("EnemyRusher",     Color.magenta,AIType.Rusher);
        BuildFortressPrefab();
        BuildBossPrefab();
        BuildGoalTriggerPrefab();
        BuildSolidWallPrefab();
        BuildDestructibleWallPrefab();
    }

    static void BuildTankPrefab()
    {
        const string path = PrefabPath + "/Battle/Tank.prefab";
        var root = new GameObject("Tank");

        var rb = root.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        var col = root.AddComponent<CircleCollider2D>();
        col.radius = 0.5f;
        root.AddComponent<TankController>();
        root.AddComponent<ScrapCollector>();
        root.tag = "Player";

        // スプライト（白丸）
        var sr = root.AddComponent<SpriteRenderer>();
        sr.sprite = GetDefaultSprite();
        sr.color  = new Color(0.8f, 0.7f, 0.3f);

        // タワーマウント
        var mount = new GameObject("TowerMount");
        mount.transform.SetParent(root.transform, false);

        SavePrefab(root, path);
    }

    static void BuildEnemyPrefab(string name, Color color, AIType aiType)
    {
        string path = $"{PrefabPath}/Enemy/{name}.prefab";
        var root = new GameObject(name);

        var rb = root.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        root.AddComponent<CircleCollider2D>().radius = 0.4f;
        var ctrl = root.AddComponent<EnemyController>();
        root.tag = "Enemy";

        var sr = root.AddComponent<SpriteRenderer>();
        sr.sprite = GetDefaultSprite();
        sr.color  = color;

        // ScrapObject プレハブ参照をセット
        var scrapPrefabGO = AssetDatabase.LoadAssetAtPath<GameObject>(
            $"{PrefabPath}/Battle/ScrapObject.prefab");
        if (scrapPrefabGO != null)
        {
            var so = new SerializedObject(ctrl);
            so.FindProperty("scrapPrefab").objectReferenceValue =
                scrapPrefabGO.GetComponent<ScrapObject>();
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        SavePrefab(root, path);
    }

    static void BuildProjectilePrefab(string name)
    {
        string path = $"{PrefabPath}/Enemy/{name}.prefab";
        var root = new GameObject(name);
        root.tag = "EnemyProjectile";

        var col = root.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.15f;
        root.AddComponent<EnemyProjectile>();

        var sr = root.AddComponent<SpriteRenderer>();
        sr.sprite = GetDefaultSprite();
        sr.color  = new Color(1f, 0.4f, 0.1f);
        root.transform.localScale = Vector3.one * 0.3f;

        SavePrefab(root, path);
    }

    static void BuildScrapObjectPrefab()
    {
        const string path = PrefabPath + "/Battle/ScrapObject.prefab";
        var root = new GameObject("ScrapObject");

        var col = root.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.25f;
        root.AddComponent<ScrapObject>();

        var sr = root.AddComponent<SpriteRenderer>();
        sr.sprite = GetDefaultSprite();
        sr.color  = new Color(0.66f, 0.73f, 0.13f);
        root.transform.localScale = Vector3.one * 0.4f;

        SavePrefab(root, path);
    }

    static void BuildFortressPrefab()
    {
        const string path = PrefabPath + "/Enemy/Fortress.prefab";
        var root = new GameObject("Fortress");
        root.tag = "Enemy";

        var col = root.AddComponent<BoxCollider2D>();
        col.size = Vector2.one * 2f;
        root.AddComponent<FortressController>();

        var sr = root.AddComponent<SpriteRenderer>();
        sr.sprite = GetDefaultSprite();
        sr.color  = new Color(0.4f, 0.3f, 0.6f);
        root.transform.localScale = Vector3.one * 2f;

        var scrapPrefabGO = AssetDatabase.LoadAssetAtPath<GameObject>(
            $"{PrefabPath}/Battle/ScrapObject.prefab");
        var projPrefabGO = AssetDatabase.LoadAssetAtPath<GameObject>(
            $"{PrefabPath}/Enemy/EnemyProjectile.prefab");

        var so = new SerializedObject(root.GetComponent<FortressController>());
        if (scrapPrefabGO != null)
            so.FindProperty("scrapPrefab").objectReferenceValue =
                scrapPrefabGO.GetComponent<ScrapObject>();
        if (projPrefabGO != null)
            so.FindProperty("projectilePrefab").objectReferenceValue = projPrefabGO;
        so.ApplyModifiedPropertiesWithoutUndo();

        SavePrefab(root, path);
    }

    static void BuildBossPrefab()
    {
        const string path = PrefabPath + "/Enemy/Boss.prefab";
        var root = new GameObject("Boss");
        root.tag = "Enemy";

        var rb = root.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f; rb.freezeRotation = true;
        root.AddComponent<CircleCollider2D>().radius = 1.5f;
        var boss = root.AddComponent<BossController>();

        var sr = root.AddComponent<SpriteRenderer>();
        sr.sprite = GetDefaultSprite();
        sr.color  = new Color(0.6f, 0.1f, 0.1f);
        root.transform.localScale = Vector3.one * 3f;

        var scrapPrefabGO = AssetDatabase.LoadAssetAtPath<GameObject>(
            $"{PrefabPath}/Battle/ScrapObject.prefab");
        var projPrefabGO  = AssetDatabase.LoadAssetAtPath<GameObject>(
            $"{PrefabPath}/Enemy/EnemyProjectile.prefab");

        var so = new SerializedObject(boss);
        if (scrapPrefabGO != null)
            so.FindProperty("scrapPrefab").objectReferenceValue =
                scrapPrefabGO.GetComponent<ScrapObject>();
        if (projPrefabGO != null)
            so.FindProperty("projectilePrefab").objectReferenceValue = projPrefabGO;
        so.ApplyModifiedPropertiesWithoutUndo();

        SavePrefab(root, path);
    }

    static void BuildGoalTriggerPrefab()
    {
        const string path = PrefabPath + "/Battle/GoalTrigger.prefab";
        var root = new GameObject("GoalTrigger");

        var col = root.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 2f;
        root.AddComponent<GoalTrigger>();

        var sr = root.AddComponent<SpriteRenderer>();
        sr.sprite = GetDefaultSprite();
        sr.color  = new Color(0.3f, 0.8f, 0.3f, 0.5f);
        root.transform.localScale = Vector3.one * 4f;

        SavePrefab(root, path);
    }

    static void BuildSolidWallPrefab()
    {
        const string path = PrefabPath + "/Battle/SolidWall.prefab";
        var root = new GameObject("SolidWall");
        root.tag = "Obstacle";

        root.AddComponent<BoxCollider2D>();
        var sr = root.AddComponent<SpriteRenderer>();
        sr.sprite = GetDefaultSprite();
        sr.color  = new Color(0.3f, 0.25f, 0.2f);

        SavePrefab(root, path);
    }

    static void BuildDestructibleWallPrefab()
    {
        const string path = PrefabPath + "/Battle/DestructibleWall.prefab";
        var root = new GameObject("DestructibleWall");
        root.tag = "Obstacle";

        root.AddComponent<BoxCollider2D>();
        root.AddComponent<DestructibleWall>();
        var sr = root.AddComponent<SpriteRenderer>();
        sr.sprite = GetDefaultSprite();
        sr.color  = new Color(0.55f, 0.42f, 0.28f);

        SavePrefab(root, path);
    }

    // ────────────────────────────────────────────
    // UI プレハブ
    // ────────────────────────────────────────────
    public static void BuildUIPrefabs()
    {
        BuildGridCellUIPrefab();
        BuildTowerCardUIPrefab();
        BuildNodeButtonUIPrefab();
        BuildShopItemUIPrefab();
        BuildSynthesisMaterialCardPrefab();
        BuildResultDropItemUIPrefab();
    }

    static void BuildGridCellUIPrefab()
    {
        const string path = PrefabPath + "/UI/GridCellUI.prefab";
        var root = new GameObject("GridCellUI");
        root.AddComponent<RectTransform>().sizeDelta = new Vector2(80, 80);

        var bg = root.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.16f, 0.1f, 0.8f);
        root.AddComponent<Button>();
        var cell = root.AddComponent<GridCellUI>();

        // タワーアイコン
        var iconGo = new GameObject("TowerIcon");
        iconGo.transform.SetParent(root.transform, false);
        var iconRT = iconGo.AddComponent<RectTransform>();
        iconRT.anchorMin = new Vector2(0.1f, 0.1f);
        iconRT.anchorMax = new Vector2(0.9f, 0.9f);
        iconRT.offsetMin = iconRT.offsetMax = Vector2.zero;
        var icon = iconGo.AddComponent<Image>();
        icon.color = Color.white;
        iconGo.SetActive(false);

        // 参照バインド
        var so = new SerializedObject(cell);
        so.FindProperty("backgroundImage").objectReferenceValue = bg;
        so.FindProperty("towerIcon").objectReferenceValue = icon;
        so.ApplyModifiedPropertiesWithoutUndo();

        SavePrefab(root, path);
    }

    static void BuildSynthesisMaterialCardPrefab()
    {
        const string path = PrefabPath + "/UI/SynthesisMaterialCard.prefab";
        var root = new GameObject("SynthesisMaterialCard");
        root.AddComponent<RectTransform>().sizeDelta = new Vector2(500, 52);
        root.AddComponent<Image>().color = new Color(0.18f, 0.14f, 0.09f);
        root.AddComponent<LayoutElement>().preferredHeight = 52;
        var card = root.AddComponent<SynthesisMaterialCard>();

        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(root.transform, false);
        var labelRT = labelGo.AddComponent<RectTransform>();
        labelRT.anchoredPosition = new Vector2(-40, 0);
        labelRT.sizeDelta = new Vector2(380, 48);
        var label = labelGo.AddComponent<TextMeshProUGUI>();
        label.fontSize = 16; label.alignment = TextAlignmentOptions.MidlineLeft;
        label.color = new Color(0.91f, 0.84f, 0.64f);

        var hlGo = new GameObject("Highlight");
        hlGo.transform.SetParent(root.transform, false);
        var hlRT = hlGo.AddComponent<RectTransform>();
        hlRT.anchorMin = Vector2.zero; hlRT.anchorMax = Vector2.one;
        hlRT.offsetMin = hlRT.offsetMax = Vector2.zero;
        var hl = hlGo.AddComponent<Image>();
        hl.color = new Color(1f, 0.8f, 0.2f, 0.3f);
        hlGo.SetActive(false);

        root.AddComponent<Button>();

        var so = new SerializedObject(card);
        so.FindProperty("label").objectReferenceValue     = label;
        so.FindProperty("highlight").objectReferenceValue = hl;
        so.FindProperty("button").objectReferenceValue    = root.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();

        SavePrefab(root, path);
    }

    static void BuildResultDropItemUIPrefab()
    {
        const string path = PrefabPath + "/UI/ResultDropItemUI.prefab";
        var root = new GameObject("ResultDropItemUI");
        root.AddComponent<RectTransform>().sizeDelta = new Vector2(500, 52);
        root.AddComponent<Image>().color = new Color(0.18f, 0.14f, 0.09f);
        root.AddComponent<LayoutElement>().preferredHeight = 52;
        var dropUI = root.AddComponent<ResultDropItemUI>();

        var labelGo = new GameObject("ItemLabel");
        labelGo.transform.SetParent(root.transform, false);
        var labelRT = labelGo.AddComponent<RectTransform>();
        labelRT.anchoredPosition = new Vector2(-30, 0);
        labelRT.sizeDelta = new Vector2(380, 48);
        var label = labelGo.AddComponent<TextMeshProUGUI>();
        label.fontSize = 16; label.alignment = TextAlignmentOptions.MidlineLeft;
        label.color = new Color(0.91f, 0.84f, 0.64f);

        var toggleGo = new GameObject("Toggle");
        toggleGo.transform.SetParent(root.transform, false);
        var toggleRT = toggleGo.AddComponent<RectTransform>();
        toggleRT.anchoredPosition = new Vector2(220, 0);
        toggleRT.sizeDelta = new Vector2(32, 32);
        var toggle = toggleGo.AddComponent<Toggle>();
        var bgGo = new GameObject("Background");
        bgGo.transform.SetParent(toggleGo.transform, false);
        var bg = bgGo.AddComponent<Image>();
        bg.color = new Color(0.3f, 0.3f, 0.3f);
        bgGo.GetComponent<RectTransform>().sizeDelta = new Vector2(32, 32);
        var checkGo = new GameObject("Checkmark");
        checkGo.transform.SetParent(bgGo.transform, false);
        var check = checkGo.AddComponent<Image>();
        check.color = new Color(0.78f, 0.48f, 0.26f);
        checkGo.GetComponent<RectTransform>().sizeDelta = new Vector2(24, 24);
        toggle.targetGraphic = bg;
        toggle.graphic = check;

        var so = new SerializedObject(dropUI);
        so.FindProperty("itemLabel").objectReferenceValue = label;
        so.FindProperty("toggle").objectReferenceValue    = toggle;
        so.ApplyModifiedPropertiesWithoutUndo();

        SavePrefab(root, path);
    }

    static void BuildTowerCardUIPrefab()
    {
        const string path = PrefabPath + "/UI/TowerCardUI.prefab";
        var root = new GameObject("TowerCardUI");
        root.AddComponent<RectTransform>().sizeDelta = new Vector2(180, 60);

        var bg = root.AddComponent<Image>();
        bg.color = new Color(0.18f, 0.14f, 0.09f);
        root.AddComponent<Button>();
        var card = root.AddComponent<TowerCardUI>();

        // アイコン
        var iconGo = new GameObject("Icon");
        iconGo.transform.SetParent(root.transform, false);
        var iconRT = iconGo.AddComponent<RectTransform>();
        iconRT.anchoredPosition = new Vector2(-65f, 0);
        iconRT.sizeDelta = new Vector2(50, 50);
        var icon = iconGo.AddComponent<Image>();

        // 名前
        var nameGo = new GameObject("NameText");
        nameGo.transform.SetParent(root.transform, false);
        var nameRT = nameGo.AddComponent<RectTransform>();
        nameRT.anchoredPosition = new Vector2(15f, 0);
        nameRT.sizeDelta = new Vector2(110, 50);
        var nameText = nameGo.AddComponent<TextMeshProUGUI>();
        nameText.fontSize = 16;
        nameText.alignment = TextAlignmentOptions.MidlineLeft;
        nameText.color = new Color(0.91f, 0.84f, 0.64f);

        // 選択ハイライト
        var hlGo = new GameObject("SelectHighlight");
        hlGo.transform.SetParent(root.transform, false);
        var hlRT = hlGo.AddComponent<RectTransform>();
        hlRT.anchorMin = Vector2.zero; hlRT.anchorMax = Vector2.one;
        hlRT.offsetMin = hlRT.offsetMax = Vector2.zero;
        var hl = hlGo.AddComponent<Image>();
        hl.color = new Color(1f, 0.8f, 0.2f, 0.3f);
        hlGo.SetActive(false);

        var so = new SerializedObject(card);
        so.FindProperty("iconImage").objectReferenceValue       = icon;
        so.FindProperty("nameText").objectReferenceValue        = nameText;
        so.FindProperty("selectionHighlight").objectReferenceValue = hl;
        so.ApplyModifiedPropertiesWithoutUndo();

        SavePrefab(root, path);
    }

    static void BuildNodeButtonUIPrefab()
    {
        const string path = PrefabPath + "/UI/NodeButtonUI.prefab";
        var root = new GameObject("NodeButtonUI");
        root.AddComponent<RectTransform>().sizeDelta = new Vector2(70, 70);

        var icon = root.AddComponent<Image>();
        icon.color = new Color(0.6f, 0.5f, 0.3f);
        root.AddComponent<Button>();
        var node = root.AddComponent<NodeButtonUI>();

        // 旗
        var flagGo = new GameObject("Flag");
        flagGo.transform.SetParent(root.transform, false);
        var flagRT = flagGo.AddComponent<RectTransform>();
        flagRT.anchoredPosition = new Vector2(20, 20);
        flagRT.sizeDelta = new Vector2(20, 20);
        var flag = flagGo.AddComponent<Image>();
        flag.color = new Color(0.9f, 0.2f, 0.2f);
        flagGo.SetActive(false);

        // 戦車
        var tankGo = new GameObject("TankIcon");
        tankGo.transform.SetParent(root.transform, false);
        var tankRT = tankGo.AddComponent<RectTransform>();
        tankRT.anchoredPosition = new Vector2(-20, 20);
        tankRT.sizeDelta = new Vector2(20, 20);
        var tankImg = tankGo.AddComponent<Image>();
        tankImg.color = new Color(0.3f, 0.8f, 0.3f);
        tankGo.SetActive(false);

        // ラベル
        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(root.transform, false);
        var labelRT = labelGo.AddComponent<RectTransform>();
        labelRT.anchoredPosition = new Vector2(0, -44);
        labelRT.sizeDelta = new Vector2(80, 20);
        var label = labelGo.AddComponent<TextMeshProUGUI>();
        label.fontSize = 10;
        label.alignment = TextAlignmentOptions.Center;
        label.color = new Color(0.91f, 0.84f, 0.64f);

        var so = new SerializedObject(node);
        so.FindProperty("iconImage").objectReferenceValue  = icon;
        so.FindProperty("flagImage").objectReferenceValue  = flag;
        so.FindProperty("tankImage").objectReferenceValue  = tankImg;
        so.FindProperty("labelText").objectReferenceValue  = label;
        so.FindProperty("button").objectReferenceValue     = root.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();

        SavePrefab(root, path);
    }

    static void BuildShopItemUIPrefab()
    {
        const string path = PrefabPath + "/UI/ShopItemUI.prefab";
        var root = new GameObject("ShopItemUI");
        root.AddComponent<RectTransform>().sizeDelta = new Vector2(700, 64);
        root.AddComponent<Image>().color = new Color(0.15f, 0.12f, 0.08f);
        root.AddComponent<LayoutElement>().preferredHeight = 64;
        var item = root.AddComponent<ShopItemUI>();

        var nameGo = new GameObject("NameText");
        nameGo.transform.SetParent(root.transform, false);
        var nameRT = nameGo.AddComponent<RectTransform>();
        nameRT.anchoredPosition = new Vector2(-180, 0);
        nameRT.sizeDelta = new Vector2(280, 50);
        var nameText = nameGo.AddComponent<TextMeshProUGUI>();
        nameText.fontSize = 18; nameText.alignment = TextAlignmentOptions.MidlineLeft;
        nameText.color = new Color(0.91f, 0.84f, 0.64f);

        var priceGo = new GameObject("PriceText");
        priceGo.transform.SetParent(root.transform, false);
        var priceRT = priceGo.AddComponent<RectTransform>();
        priceRT.anchoredPosition = new Vector2(80, 0);
        priceRT.sizeDelta = new Vector2(140, 50);
        var priceText = priceGo.AddComponent<TextMeshProUGUI>();
        priceText.fontSize = 18; priceText.alignment = TextAlignmentOptions.Center;
        priceText.color = new Color(0.66f, 0.73f, 0.13f);

        var btnGo = new GameObject("BuyButton");
        btnGo.transform.SetParent(root.transform, false);
        var btnRT = btnGo.AddComponent<RectTransform>();
        btnRT.anchoredPosition = new Vector2(290, 0);
        btnRT.sizeDelta = new Vector2(110, 46);
        btnGo.AddComponent<Image>().color = new Color(0.78f, 0.48f, 0.26f);
        btnGo.AddComponent<Button>();
        var btnLabelGo = new GameObject("Label");
        btnLabelGo.transform.SetParent(btnGo.transform, false);
        var btnLabelRT = btnLabelGo.AddComponent<RectTransform>();
        btnLabelRT.anchorMin = Vector2.zero; btnLabelRT.anchorMax = Vector2.one;
        btnLabelRT.offsetMin = btnLabelRT.offsetMax = Vector2.zero;
        var btnLabel = btnLabelGo.AddComponent<TextMeshProUGUI>();
        btnLabel.text = "購入"; btnLabel.fontSize = 16;
        btnLabel.alignment = TextAlignmentOptions.Center;
        btnLabel.color = new Color(0.91f, 0.84f, 0.64f);

        var so = new SerializedObject(item);
        so.FindProperty("nameText").objectReferenceValue  = nameText;
        so.FindProperty("priceText").objectReferenceValue = priceText;
        so.FindProperty("buyButton").objectReferenceValue = btnGo.GetComponent<Button>();
        so.FindProperty("btnLabel").objectReferenceValue  = btnLabel;
        so.ApplyModifiedPropertiesWithoutUndo();

        SavePrefab(root, path);
    }

    // ────────────────────────────────────────────
    // ScriptableObjects
    // ────────────────────────────────────────────
    public static void BuildScriptableObjects()
    {
        // 蒸気砲（初期タワー）
        BuildTowerData("TowerData_SteamCannon", new TowerDataParams
        {
            towerId = "steam_cannon", displayName = "蒸気砲",
            attackType = AttackType.Aimed, damageType = DamageType.Single,
            damage = 30, cooldown = 2.0f, range = 8.0f, basePrice = 40,
        });

        // 電磁砲塔
        BuildTowerData("TowerData_ElectroTurret", new TowerDataParams
        {
            towerId = "electro_turret", displayName = "電磁砲塔",
            attackType = AttackType.AutoAim, damageType = DamageType.Single,
            damage = 20, cooldown = 1.0f, range = 9.0f, basePrice = 55,
        });

        // 蒸気炎放器
        BuildTowerData("TowerData_FlameEmitter", new TowerDataParams
        {
            towerId = "flame_emitter", displayName = "蒸気炎放器",
            attackType = AttackType.Area, damageType = DamageType.Area,
            damage = 15, cooldown = 0.5f, range = 3.0f, basePrice = 35,
        });

        // 敵データ
        BuildEnemyData("EnemyData_Chaser", new EnemyDataParams
        {
            enemyName = "スクラップウォーカー", maxHp = 40, moveSpeed = 3f,
            attackDamage = 10, attackCooldown = 1f, attackRange = 0.6f,
            aiType = AIType.Chaser, scrapDropMin = 5, scrapDropMax = 10,
        });

        BuildEnemyData("EnemyData_Rusher", new EnemyDataParams
        {
            enemyName = "装甲列車", maxHp = 150, moveSpeed = 6f,
            attackDamage = 30, attackCooldown = 2f, attackRange = 0.6f,
            aiType = AIType.Rusher, scrapDropMin = 30, scrapDropMax = 50,
        });
    }

    struct TowerDataParams
    {
        public string towerId, displayName;
        public AttackType attackType;
        public DamageType damageType;
        public float damage, cooldown, range;
        public int basePrice;
    }

    static void BuildTowerData(string assetName, TowerDataParams p)
    {
        string path = $"{SOPath}/Towers/{assetName}.asset";
        if (File.Exists(path)) return;
        var data = ScriptableObject.CreateInstance<TowerData>();
        data.towerId = p.towerId; data.displayName = p.displayName;
        data.attackType = p.attackType; data.damageType = p.damageType;
        data.damage = p.damage; data.cooldown = p.cooldown; data.range = p.range;
        data.basePrice = p.basePrice;
        AssetDatabase.CreateAsset(data, path);
    }

    struct EnemyDataParams
    {
        public string enemyName;
        public float maxHp, moveSpeed, attackDamage, attackCooldown, attackRange;
        public AIType aiType;
        public int scrapDropMin, scrapDropMax;
    }

    static void BuildEnemyData(string assetName, EnemyDataParams p)
    {
        string path = $"{SOPath}/Enemies/{assetName}.asset";
        if (File.Exists(path)) return;
        var data = ScriptableObject.CreateInstance<EnemyData>();
        data.enemyName = p.enemyName; data.maxHp = p.maxHp; data.moveSpeed = p.moveSpeed;
        data.attackDamage = p.attackDamage; data.attackCooldown = p.attackCooldown;
        data.attackRange = p.attackRange; data.aiType = p.aiType;
        data.scrapDropMin = p.scrapDropMin; data.scrapDropMax = p.scrapDropMax;
        AssetDatabase.CreateAsset(data, path);
    }

    // ────────────────────────────────────────────
    // ユーティリティ
    // ────────────────────────────────────────────
    public static T LoadPrefab<T>(string subPath) where T : Component
    {
        var go = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabPath}/{subPath}");
        return go != null ? go.GetComponent<T>() : null;
    }

    public static GameObject LoadPrefabGO(string subPath) =>
        AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabPath}/{subPath}");

    public static T LoadSO<T>(string subPath) where T : ScriptableObject =>
        AssetDatabase.LoadAssetAtPath<T>($"{SOPath}/{subPath}");

    static void SavePrefab(GameObject go, string path)
    {
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
    }

    static Sprite GetDefaultSprite() =>
        AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
}
