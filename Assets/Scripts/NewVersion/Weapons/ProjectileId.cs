﻿using UnityEngine;

namespace NewVersion.Weapons
{
    [CreateAssetMenu(menuName = "Weapons/Create ProyectileId", fileName = "ProyectileId", order = 0)]
    public class ProjectileId : ScriptableObject
    {
        [SerializeField] private string id;

        public string Id => id;
    }
}