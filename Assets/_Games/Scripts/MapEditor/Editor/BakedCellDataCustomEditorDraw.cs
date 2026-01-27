using System.Collections;
using System.Collections.Generic;
using GamePlayFoundation.Entities.Tile;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.OdinInspector.Editor.Internal;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace GamePlayFoundation.MapEditor
{
    public class BakedCellDataCustomEditorDraw
        : TwoDimensionalArrayDrawer<IList, BakedCellEdit>
    {
        static TileCatalog _catalog;

        protected override BakedCellEdit DrawElement(Rect rect, BakedCellEdit value)
        {
            // ===== Click select =====
            if (Event.current.type == EventType.MouseDown &&
                rect.Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 0)
                {
                    value.IsSelected = !value.IsSelected;
                    value.indexSelected = EditorApplication.timeSinceStartup;
                }

                GUI.changed = true;
                Event.current.Use();
            }

            // ===== Selected highlight =====
            if (value.IsSelected)
            {
                EditorGUI.DrawRect(
                    rect.Padding(rect.width * 0.05f),
                    new Color(1f, 1f, 0f, 0.35f)
                );
            }

            if (value.BakedCellData.layers == null ||
                value.BakedCellData.layers.Count == 0)
                return value;

            // ===== Cache TileCatalog =====
            if (_catalog == null)
                _catalog = Resources.Load<TileCatalog>("Tile catalog");

            var layers = value.BakedCellData.layers;
            int count = layers.Count;

            Rect contentRect = rect.Padding(rect.width * 0.08f);

            // ===== 1 layer =====
            if (count == 1)
            {
                DrawLayer(contentRect, layers[0]);
                return value;
            }

            // ===== 2–3 layer: chia đều ngang =====
            if (count == 2 || count == 3)
            {
                float spacing = contentRect.width * 0.05f;
                float totalSpacing = spacing * (count - 1);
                float size = (contentRect.width - totalSpacing) / count;

                for (int i = 0; i < count; i++)
                {
                    Rect r = new Rect(
                        contentRect.x + i * (size + spacing),
                        contentRect.y,
                        size,
                        contentRect.height
                    );

                    DrawLayer(r, layers[i]);
                }

                return value;
            }

            // ===== 4+ layer: fallback grid =====
            int col = Mathf.CeilToInt(Mathf.Sqrt(count));
            float cellSize = contentRect.width / col;

            for (int i = 0; i < count; i++)
            {
                int x = i % col;
                int y = i / col;

                Rect r = new Rect(
                    contentRect.x + x * cellSize,
                    contentRect.y + y * cellSize,
                    cellSize,
                    cellSize
                ).Padding(2);

                DrawLayer(r, layers[i]);
            }

            // ===== Overlay số lượng layer =====
            GUI.Label(
                rect.AlignTop(14),
                $"+{count}",
                EditorStyles.miniBoldLabel
            );

            return value;
        }

        // =====================================================
        // Helpers
        // =====================================================

        static void DrawLayer(Rect rect, BakedCellLayerData layer)
        {
            var sprite = _catalog.sprites[layer.tileTileID];
            if (sprite == null)
                return;

            // ===== Draw sprite =====
            GUI.DrawTexture(rect, sprite.texture, ScaleMode.ScaleToFit);

            // =================================================
            // Overlay: Tile ID (bottom)
            // =================================================
            Rect idRect = rect.AlignBottom(14);

            GUI.color = new Color(0f, 0f, 0f, 0.6f);
            GUI.DrawTexture(idRect, Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(
                idRect,
                layer.tileTileID.ToString(),
                EditorStyles.boldLabel
            );

            // =================================================
            // Overlay: Hidden (top-left)
            // =================================================
            if (layer.hasHidden)
            {
                Rect hiddenRect = new Rect(
                    rect.x + 2,
                    rect.y + 2,
                    18,
                    18
                );

                EditorGUI.DrawRect(
                    hiddenRect,
                    new Color(0.15f, 0.15f, 0.15f, 0.8f)
                );

                GUI.Label(
                    hiddenRect,
                    "?",
                    EditorStyles.whiteBoldLabel
                );
            }

            // =================================================
            // Overlay: Frozen / Ice (top-right)
            // =================================================
            if (layer.hasFrozen)
            {
                Rect iceRect = new Rect(
                    rect.xMax - 22,
                    rect.y + 2,
                    20,
                    20
                );

                EditorGUI.DrawRect(
                    iceRect,
                    new Color(0.3f, 0.8f, 1f, 0.85f)
                );

                GUI.Label(
                    iceRect,
                    "❄",
                    EditorStyles.whiteBoldLabel
                );

                // ===== Frozen counter =====
                if (layer.frozenCounter > 0)
                {
                    Rect counterRect = new Rect(
                        iceRect.x - 10,
                        iceRect.y + iceRect.height - 10,
                        18,
                        18
                    );

                    EditorGUI.DrawRect(
                        counterRect,
                        new Color(0f, 0f, 0f, 0.75f)
                    );

                    GUI.Label(
                        counterRect,
                        layer.frozenCounter.ToString(),
                        EditorStyles.whiteMiniLabel
                    );
                }
            }
        }
    }
}