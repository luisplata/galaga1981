using UnityEngine;

namespace NewVersion.Ship
{
    [CreateAssetMenu(menuName = "Create ShipId", fileName = "ShipId", order = 0)]
    public class ShipId : ScriptableObject
    {
        [SerializeField] private string id;

        public string Id => id;
    }
}