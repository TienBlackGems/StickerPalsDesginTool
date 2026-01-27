using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlayFoundation.MapEditor
{
    [System.Serializable]
    [HideReferenceObjectPicker, HideLabel]
    public class BakedCellEdit
    {
        [HideInInspector] public bool IsSelected;
        [HideInInspector] public double indexSelected;
        public BakedCellData BakedCellData = new BakedCellData(); 
    }
}