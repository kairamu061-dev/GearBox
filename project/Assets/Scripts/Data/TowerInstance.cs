using System;

[Serializable]
public class TowerInstance
{
    public TowerData data;
    public int level;
    public Vector2IntSerializable gridPosition;
    public bool isOnGrid;

    public TowerInstance(TowerData data)
    {
        this.data = data;
        this.level = 1;
        this.isOnGrid = false;
    }
}

[Serializable]
public struct Vector2IntSerializable
{
    public int x;
    public int y;

    public Vector2IntSerializable(int x, int y) { this.x = x; this.y = y; }
    public UnityEngine.Vector2Int ToVector2Int() => new UnityEngine.Vector2Int(x, y);
}
