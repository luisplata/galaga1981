using System.Collections.Generic;
using UnityEngine;

namespace NewVersion.Ship.Enemies
{
    [CreateAssetMenu(menuName = "Level/Create LevelConfiguration", fileName = "LevelConfiguration", order = 0)]
    public class LevelConfiguration : ScriptableObject
    {
        [SerializeField] public SpawnConfiguration[] spawnConfigurations;
    }
}