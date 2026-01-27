using System;
using System.Collections.Generic;
using System.Linq;
using GamePlayFoundation.Entities.Tile;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataSo", menuName = "Game/Level/Level Data")]
public class LevelDataSo : ScriptableObject
{
    [field: SerializeField] public int Rows { get; set; }
    [field: SerializeField] public int Cols { get; set; }

    [field: SerializeField] public float CellWidth { get; set; }
    [field: SerializeField] public float CellHeight { get; set; }

    [field: SerializeField] public List<BakedCellData> BakedCells { get; set; }
    [field: SerializeField] public List<BakedTutorialStepData> BakedTutorial { get; set; }

    [field: SerializeField] public LevelCameraData Camera { get; set; }
    [field: SerializeField] public LevelMetaData Level { get; set; }

    [field: SerializeField] public LevelEconomyData Economy { get; set; }
    [field: SerializeField] public LevelGameData Game { get; set; }
    [field: SerializeField] public LevelThemeData Theme { get; set; }

    [Button]
    private void DebugLevelDoubleLayer()
    {
        int count = 0;
        foreach (var bakedCellData in BakedCells)
        {
            if (bakedCellData.layers.Count > 1)
                count++;
        }
        if(count > 0) Debug.Log($"{name} {count}");
    }
}

[Serializable]
public enum CellType
{
    Tile1x1
}

[Serializable]
public class BakedTutorialStepData
{
    [SerializeField] public string Message;
    [SerializeField] public float Delay;

    [SerializeField] public bool UseGridCoordinates;

    [SerializeField] public Vector2Int MainCell;
    [SerializeField] public Vector2Int SecondaryCell;

    [SerializeField] public Vector2 MainViewportPosition;
    [SerializeField] public Vector2 SecondaryViewportPosition;

    [SerializeField] public bool IsDrag;
    [SerializeField] public bool PingPong;

    [SerializeField] public List<Vector2Int> AllowedCells;
    [SerializeField] public Vector2Int AllowedDragDirection;

    [SerializeField] public float RequiredDragDistance;
    [SerializeField] public bool AdvanceOnInput;

    [SerializeField] public List<Vector2Int> HighlightCells;

    [SerializeField] public bool HasSecondaryHand;
    [SerializeField] public bool SecondaryHandIsDrag;

    [SerializeField] public Vector2Int SecondaryTapCell;
    [SerializeField] public Vector2 SecondaryTapViewportPosition;

    [SerializeField] public Vector2 SecondaryHandStart;
    [SerializeField] public Vector2 SecondaryHandEnd;
}

[Serializable]
public class BakedCellData
{
    [SerializeField] public int x;
    [SerializeField] public int y;

    [SerializeField] public CellType cellType;

    //[SerializeField] public int multiCellGroupId;
 //   [SerializeField] public bool isMultiCellPivot;

  //  [SerializeField] public Vector2Int groupPivot;

 //   [SerializeField] public int connectionId;

    [SerializeField] public List<BakedCellLayerData> layers;
}

[Serializable]
public class BakedCellLayerData
{
  //  [SerializeField] public int layerType;
 //   [SerializeField] public int gridObjectType;
 //   [SerializeField] public int layerIndex;

 //   [SerializeField] public int tileTileShapes;
 //   [SerializeField] public List<Vector2Int> tileTileShapesCells;

  //  [SerializeField] public int tileCellFeature;

    [SerializeField] public bool hasFrozen;
    [SerializeField] public int frozenCounter;

    [SerializeField] public bool hasHidden;
   // [SerializeField] public int hiddenCounter;

//    [SerializeField] public bool hasKey;
 //   [SerializeField] public int keyKeyLockColor;
    [SerializeField] public int tileTileID;

#if UNITY_EDITOR

    [ValueDropdown("GetAllTiles"), OnValueChanged("OnChangedItem")]
    [ShowInInspector] private Sprite item;

    private void OnChangedItem()
    {
        Sprite[] sprites = GetAllTiles();
        tileTileID = sprites.ToList().IndexOf(item);
    }
    
    [OnInspectorGUI]
    private void OnUpdate()
    {
        if(item != null) return;
        Sprite[] sprites = GetAllTiles();
        if(tileTileID > sprites.Length) return;
        item = sprites[tileTileID];
    }
    
    private Sprite[] GetAllTiles()
    {
        return Resources.Load<TileCatalog>("Tile catalog").sprites;
    }
#endif

  //  [SerializeField] public int lockLockShape;
   // [SerializeField] public List<Vector2Int> lockLockShapeCells;

  //  [SerializeField] public int lockKeyLockColor;
}

[Serializable]
public class LevelCameraData
{
    [field: SerializeField] public Vector3 CameraPosition { get; private set; }
    [field: SerializeField] public Vector3 CameraRotation { get; private set; }
    [field: SerializeField] public float CameraFOV { get; set; }
}

[Serializable]
public class LevelMetaData
{
    [field: SerializeField] public bool IsBossLevel { get; private set; }
    [field: SerializeField] public bool IsHardLevel { get; private set; }
    [field: SerializeField] public bool CanBeRandomlySelected { get; private set; }
}

[Serializable]
public class LevelEconomyData
{
    [field: SerializeField] public int CurrencyReward { get; private set; }
    [field: SerializeField] public int HardCurrencyReward { get; private set; }

    [field: SerializeField] public int RvCount { get; private set; }
    [field: SerializeField] public List<int> ReviveCosts { get; private set; }
}

[Serializable]
public class LevelGameData
{
    [field: SerializeField] public bool HasTimer { get; private set; }
    [field: SerializeField] public int TimerDuration { get; private set; }

    [field: SerializeField] public bool HasHelper { get; private set; }
}


[Serializable]
public class LevelThemeData
{
    [field: SerializeField] public int ThemeMode { get; private set; }
    [field: SerializeField] public int SpecificTheme { get; private set; }
}