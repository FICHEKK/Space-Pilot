using UnityEngine;

[CreateAssetMenu(fileName = "Map Settings", menuName = "Map Settings", order = 0)]
public class MapSettings : ScriptableObject
{
    public int laneCount;
    public float laneWidth;
}
