using UnityEngine;

[CreateAssetMenu(fileName = "SynthesisRecipe", menuName = "GearBox/SynthesisRecipe")]
public class SynthesisRecipe : ScriptableObject
{
    public TowerData materialA;
    public TowerData materialB;
    public TowerData result;
}
