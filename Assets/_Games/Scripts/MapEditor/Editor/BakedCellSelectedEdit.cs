using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlayFoundation.MapEditor
{
    [System.Serializable]
    [HideReferenceObjectPicker, HideLabel]
    public class BakedCellSelectedEdit
    {
        [ShowIf("IsShowing")]
        public BakedCellEdit bakedCellEdit;
        [HideInInspector]
        public bool IsShowing;
    }
}