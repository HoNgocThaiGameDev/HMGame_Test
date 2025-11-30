using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TestHM
{
    public class BoardGenerator : MonoBehaviour
    {
        private TileManager tileManager;

        public void Initialize(TileManager manager)
        {
            tileManager = manager;
        }

        public IEnumerator GenerateBoard()
        {
            for (int i = 0; i < tileManager.Layers.Count; i++)
            {
                LayerState layer = tileManager.Layers[i];
                bool locked = !layer.IsUnlocked;
                int sortingBase = (tileManager.Layers.Count - i) * 10;

                if (layer.Orientation == LayerOrientation.Horizontal)
                {
                    yield return StartCoroutine(CreateHorizontalLayer(layer, locked, sortingBase));
                }
                else
                {
                    yield return StartCoroutine(CreateVerticalLayer(layer, locked, sortingBase));
                }
            }

            tileManager.SetCurrentLayerIndex(0);
            if (tileManager.Layers.Count > 0)
            {
                tileManager.SetRemainingBoardTiles(tileManager.Layers[0].TilesRemaining);
            }
            tileManager.SetBoardReady(true);

            GameManager.Instance?.StartGame();
            tileManager.MaybeStartAutoplay();
        }

        public bool BuildLayers()
        {
            tileManager.Layers.Clear();
            tileManager.TileToLayer.Clear();

            for (int i = 0; i < tileManager.TotalLayers; i++)
            {
                bool isHorizontal = i % 2 == 0;
                ResolveLayerDimensions(i, isHorizontal, out int layerRows, out int layerColumns);
                int tileCount = layerRows * layerColumns;

                if (tileCount <= 0)
                {
                    //Debug.LogError($"layer {i} must have a positive number of tiles");
                    return false;
                }

                LayerState layer = new LayerState
                {
                    Orientation = isHorizontal ? LayerOrientation.Horizontal : LayerOrientation.Vertical,
                    SpriteBag = CreateSpriteBag(tileCount),
                    TilesRemaining = tileCount,
                    TotalTiles = tileCount,
                    TilesMatched = 0,
                    IsUnlocked = i == 0,
                    Rows = layerRows,
                    Columns = layerColumns
                };

                tileManager.Layers.Add(layer);
            }

            tileManager.SetCurrentLayerIndex(0);
            return true;
        }

        private Queue<Sprite> CreateSpriteBag(int tileCount)
        {
            List<Sprite> temp = new List<Sprite>(tileCount);
            int groups = Mathf.Max(1, tileCount / 3);

            List<Sprite> allSprites = new List<Sprite>(tileManager.FSprites);

            for (int i = 0; i < allSprites.Count; i++)
            {
                Sprite tempS = allSprites[i];
                int randomIndex = Random.Range(i, allSprites.Count);
                allSprites[i] = allSprites[randomIndex];
                allSprites[randomIndex] = tempS;
            }

            int groupsAdded = 0;

            foreach (var sprite in allSprites)
            {
                if (groupsAdded >= groups) break;
                temp.Add(sprite);
                temp.Add(sprite);
                temp.Add(sprite);
                groupsAdded++;
            }

            while (groupsAdded < groups)
            {
                Sprite selected = tileManager.FSprites[Random.Range(0, tileManager.FSprites.Length)];
                temp.Add(selected);
                temp.Add(selected);
                temp.Add(selected);
                groupsAdded++;
            }

            if (temp.Count > tileCount)
            {
                temp.RemoveRange(tileCount, temp.Count - tileCount);
            }

            for (int i = temp.Count - 1; i > 0; i--)
            {
                int rand = Random.Range(0, i + 1);
                (temp[i], temp[rand]) = (temp[rand], temp[i]);
            }

            return new Queue<Sprite>(temp);
        }

        private IEnumerator CreateHorizontalLayer(LayerState layer, bool locked, int sortingBase)
        {
            float tileWidth = 0.3f;
            float totalWidth = (layer.Columns - 1) * (tileWidth + tileManager.HorizontalTileOffset);
            float totalHeight = (layer.Rows - 1) * (tileWidth + tileManager.RowSpacing);
            float startX = -totalWidth / 2f;
            float startY = totalHeight / 2f;
            float delay = 0.05f;

            for (int row = 0; row < layer.Rows; row++)
            {
                for (int col = 0; col < layer.Columns; col++)
                {
                    GameObject newTile = Instantiate(tileManager.HorizontalBaseTilePrefab);

                    float posX = startX + col * (tileWidth + tileManager.HorizontalTileOffset);
                    float posY = startY - row * (tileWidth + tileManager.RowSpacing);
                    newTile.transform.position = new Vector3(posX, posY, 0);
                    newTile.transform.localScale = Vector3.zero;

                    StartCoroutine(ScaleTile(newTile, tileManager.HorizontalTileScale));

                    Sprite assignedSprite = GetNextSpriteFromBag(layer.SpriteBag);
                    ConfigureTile(newTile, assignedSprite, sortingBase + 1, sortingBase + 2);
                    newTile.transform.SetParent(transform);

                    tileManager.RegisterPlayableTile(newTile, assignedSprite, layer, locked);
                    yield return new WaitForSeconds(delay);
                }
            }
        }

        private IEnumerator CreateVerticalLayer(LayerState layer, bool locked, int sortingBase)
        {
            float tileHeight = 0.3f;
            float spacing = tileManager.ColumnSpacing;
            float totalWidth = (layer.Columns - 1) * spacing;
            float totalHeight = (layer.Rows - 1) * (tileHeight + tileManager.VerticalTileOffset);
            float horizontalOffset = layer.Columns == 1 ? 0f : 0f;
            float startX = -totalWidth / 2f + horizontalOffset;
            float startY = totalHeight / 2f;
            float delay = 0.05f;

            for (int col = 0; col < layer.Columns; col++)
            {
                for (int row = 0; row < layer.Rows; row++)
                {
                    GameObject newTile = Instantiate(tileManager.VerticalBaseTilePrefab);
                    float posX = startX + col * tileManager.ColumnSpacing;
                    float posY = startY - row * (tileHeight + tileManager.VerticalTileOffset);

                    newTile.transform.position = new Vector3(posX, posY, 0);
                    newTile.transform.localScale = Vector3.zero;
                    StartCoroutine(ScaleTile(newTile, tileManager.VerticalTileScale));

                    Sprite assignedSprite = GetNextSpriteFromBag(layer.SpriteBag);
                    ConfigureTile(newTile, assignedSprite, sortingBase, sortingBase + 1);
                    newTile.transform.SetParent(transform);
                    tileManager.RegisterPlayableTile(newTile, assignedSprite, layer, locked);

                    yield return new WaitForSeconds(delay);
                }
            }
        }

        private void ResolveLayerDimensions(int layerIndex, bool isHorizontal, out int resolvedRows, out int resolvedColumns)
        {
            resolvedRows = isHorizontal ? tileManager.Rows : tileManager.VerticalRows;
            resolvedColumns = isHorizontal ? tileManager.Columns : tileManager.VerticalColumns;

            if (layerIndex < tileManager.LayerDimensions.Count && tileManager.LayerDimensions[layerIndex] != null)
            {
                var custom = tileManager.LayerDimensions[layerIndex];
                if (custom.rows > 0)
                {
                    resolvedRows = custom.rows;
                }
                if (custom.columns > 0)
                {
                    resolvedColumns = custom.columns;
                }
            }
        }

        private void ConfigureTile(GameObject tile, Sprite sprite, int baseSortingOrder, int fSortingOrder)
        {
            SpriteRenderer baseRenderer = tile.GetComponent<SpriteRenderer>();
            if (baseRenderer != null)
            {
                baseRenderer.sortingLayerName = "Foreground";
                baseRenderer.sortingOrder = baseSortingOrder;
            }

            Transform fSprite = tileManager.ResolveFSpriteTransform(tile);
            if (fSprite == null)
            {
                return;
            }

            SpriteRenderer fRenderer = fSprite.GetComponent<SpriteRenderer>();
            if (fRenderer == null)
            {
                return;
            }

            fRenderer.sortingLayerName = "Foreground";
            fRenderer.sortingOrder = fSortingOrder;
            fRenderer.sprite = sprite;
            fSprite.localScale = Vector3.one;
            fSprite.localPosition = new Vector3(-0.043f, 0.022f, 0);

            SortingGroup sortingGroup = tile.GetComponent<SortingGroup>();
            if (sortingGroup != null)
            {
                sortingGroup.sortingLayerName = "Foreground";
                sortingGroup.sortingOrder = Mathf.Max(baseSortingOrder, fSortingOrder);
            }
        }

        private Sprite GetNextSpriteFromBag(Queue<Sprite> bag)
        {
            if (bag == null || bag.Count == 0)
            {
                //Debug.LogWarning("sprite bag depleted unexpectedly, recycling sprites");
                return tileManager.FSprites[Random.Range(0, tileManager.FSprites.Length)];
            }

            return bag.Dequeue();
        }

        IEnumerator ScaleTile(GameObject tile, Vector3 targetScale)
        {
            float duration = 0.1f;
            Vector3 initialScale = Vector3.zero;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                tile.transform.localScale = Vector3.Lerp(initialScale, targetScale, (elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            tile.transform.localScale = targetScale;
        }
    }
}
