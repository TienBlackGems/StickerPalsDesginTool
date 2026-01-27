using UnityEngine;

namespace GamePlayFoundation.Entities.Tile
{
    [CreateAssetMenu(menuName = "GamePlayFoundation/Tile catalog", fileName = "Tile catalog")]
    public class TileCatalog : ScriptableObject
    {
        public Sprite[] sprites;
    }
}