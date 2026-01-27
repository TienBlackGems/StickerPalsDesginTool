using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlayFoundation.MapEditor
{
    public class MapDesignWindow : OdinEditorWindow
    {
        [ValueDropdown("GetAllLevelData")]
        [OnValueChanged("OnChangedLevel"), PropertyOrder(-1)]
        public LevelDataSo levelData;

        [OnValueChanged("OnDataChanged"), PropertyOrder(-1)]
        public int Row;
        
        [OnValueChanged("OnDataChanged"), PropertyOrder(-1)]
        public int Cols;

        [PropertyOrder(-1)]
        public float CameraFOV;

        [Button(ButtonSizes.Large), PropertyOrder(-1)]
        public void Save()
        {
            levelData.Cols = Row;
            levelData.Rows = Cols;
            levelData.Camera.CameraFOV = CameraFOV;

            var bakedData = new List<BakedCellData>();
            foreach (var bakedCellEdit in CellEdits)
            {
                var jsonData = JsonUtility.ToJson(bakedCellEdit.BakedCellData);
                bakedData.Add(JsonUtility.FromJson<BakedCellData>(jsonData));
            }
            levelData.BakedCells = bakedData;
            EditorUtility.SetDirty(levelData);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            SaveLevel(levelData.name, JsonConvert.SerializeObject(levelData, settings));
        }
        
        public static string GetLevelFolder()
        {
            return Path.Combine(
                Application.persistentDataPath,
                "Levels"
            );
        }

        public static void SaveLevel(string levelName, string json)
        {
#if UNITY_EDITOR
            string dir = GetLevelFolder();

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string path = Path.Combine(dir, levelName + ".txt");
            File.WriteAllText(path, json);

            Debug.Log($"Level saved to: {path}");
#endif
        }

        [HorizontalGroup("Edit")]
        [TableMatrix(SquareCells = true, HideColumnIndices = true, HideRowIndices = true, ResizableColumns = false)]
        [OnValueChanged("OnTapEdit")]
        public BakedCellEdit[,] CellEdits;

        [HorizontalGroup("Edit")]
        public BakedCellSelectedEdit bakedCellSelectedEdit;
        
        [MenuItem("Tools/Map Design Window")]
        private static void OpenWindow()
        {
            GetWindow<MapDesignWindow>("Map Design Window").Show();
        }

        private void OnTapEdit()
        {
            var listBakedCellEdits = new List<BakedCellEdit>();
            foreach (var bakedCellEdit in CellEdits)
            {
                if(bakedCellEdit.IsSelected)
                    listBakedCellEdits.Add(bakedCellEdit);
            }
            listBakedCellEdits.Sort((a, b) => a.indexSelected.CompareTo(b.indexSelected));
            listBakedCellEdits.Reverse();
            if (listBakedCellEdits.Count > 1)
            {
                for (var i = 1; i < listBakedCellEdits.Count; i++)
                    listBakedCellEdits[i].IsSelected = false; 
            }

            if (listBakedCellEdits.Count >= 1)
            {
                bakedCellSelectedEdit.bakedCellEdit = listBakedCellEdits[0];
                bakedCellSelectedEdit.IsShowing = true;
            }
            else
                bakedCellSelectedEdit.IsShowing = false;
        }

        private void OnChangedLevel()
        {
            if(levelData == null) return;
            Row = levelData.Cols;
            Cols = levelData.Rows;
            CameraFOV = levelData.Camera.CameraFOV;
            
            CellEdits = new BakedCellEdit[Row, Cols];
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    int y = Cols - j - 1;
                    CellEdits [i, j] = new BakedCellEdit();
                    var data = levelData.BakedCells.Find(cell => cell.x == i && cell.y == y);
                    if (data != null)
                    {
                        var json = JsonUtility.ToJson(data);
                        CellEdits[i, j].BakedCellData = JsonUtility.FromJson<BakedCellData>(json);
                    }

                    CellEdits[i, j].BakedCellData.x = i;
                    CellEdits[i, j].BakedCellData.y = y;
                }
            }
        }

        private void OnDataChanged()
        {
            if (levelData == null)
                return;

            int newRows = Row;
            int newCols = Cols;

            var newCells = new BakedCellEdit[newRows, newCols];

            if (CellEdits != null)
            {
                int oldRows = CellEdits.GetLength(0);
                int oldCols = CellEdits.GetLength(1);

                int copyRows = Mathf.Min(oldRows, newRows);
                int copyCols = Mathf.Min(oldCols, newCols);

                for (int i = 0; i < copyRows; i++)
                {
                    for (int j = 0; j < copyCols; j++)
                    {
                        newCells[i, j] = CellEdits[i, j];
                    }
                }
            }

            for (int i = 0; i < newRows; i++)
            {
                for (int j = 0; j < newCols; j++)
                {
                    int y = Cols - j - 1;
                    if (newCells[i, j] == null)
                        newCells[i, j] = new BakedCellEdit();
                    
                    newCells[i, j].BakedCellData.x = i;
                    newCells[i, j].BakedCellData.y = y;
                }
            }

            CellEdits = newCells;
        }
        
        public LevelDataSo[] GetAllLevelData()
        {
            return Resources.LoadAll<LevelDataSo>("Levels");
        }
    }
}