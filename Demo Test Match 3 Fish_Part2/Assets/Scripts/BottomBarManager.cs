using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TestHM
{
    public enum AnimationType
    {
        Default,
        SlowFade
    }

    public class BottomBarManager : MonoBehaviour
    {
        public Transform bottomBar;
        public Transform tileCloneParent;
        public float tileSpacing = 100.0f;
        public float horizontalOffset = 0.0f;
        public float verticalOffset = 0.0f;
        public float tileMoveSpeed = 0.5f;
        public AnimationType animationType = AnimationType.Default;
        public float slowFadeDuration = 0.5f;
        public const int MaxCells = 5;
        [SerializeField] private string fSpriteChildName = "FSprite";
        private static readonly string[] FChildFallbackNames = { "FSprite" };

        private List<GameObject> trayOrder = new List<GameObject>();
        private Dictionary<string, List<GameObject>> groupedTiles = new Dictionary<string, List<GameObject>>();
        private int currentOrderInLayer = 1;
        private bool isLocked;

        public int CurrentTileCount => trayOrder.Count;

        public int GetSpecificTileCount(Sprite sprite)
        {
            if (sprite == null) return 0;
            if (groupedTiles.TryGetValue(sprite.name, out var list))
            {
                return list.Count;
            }
            return 0;
        }

        public void AddTile(GameObject tile)
        {
            if (isLocked)
            {
                return;
            }

            SpriteRenderer fSpriteRenderer = ResolveFSpriteRenderer(tile);
            if (fSpriteRenderer == null || fSpriteRenderer.sprite == null)
            {
                Debug.LogError("tile does not have a fruit sprite child with a SpriteRenderer");
                return;
            }

            string fKey = GetSpriteKey(fSpriteRenderer);
            if (string.IsNullOrEmpty(fKey))
            {
                Debug.LogError("sprite key missing for tile");
                return;
            }

            if (!groupedTiles.ContainsKey(fKey))
            {
                groupedTiles[fKey] = new List<GameObject>();
            }

            if (trayOrder.Count >= MaxCells)
            {
                if (GameManager.Instance != null && GameManager.Instance.CurrentMode == GameManager.GameMode.TimeAttack)
                {
                    // In Time Attack, do not lose on full tray
                    return;
                }
                TriggerLoseCondition();
                return;
            }

            tile.GetComponent<TileInteraction>().IsInTray = true;


            groupedTiles[fKey].Add(tile);

            trayOrder.Add(tile);

            tile.transform.SetParent(tileCloneParent, false);

            AdjustTilePositions();

            SortingGroup sortingGroup = tile.GetComponent<SortingGroup>();
            if (sortingGroup != null)
            {
                sortingGroup.sortingLayerName = "UI";
                sortingGroup.sortingOrder = currentOrderInLayer;
            }

            tile.transform.localScale = new Vector3(1, 1, 1);

            int targetIndex = Mathf.Max(0, trayOrder.Count - 1);
            Vector3 targetPosition = GetTargetPositionForTile(targetIndex);
            StartCoroutine(MoveTileToPosition(tile, targetPosition));

            currentOrderInLayer++;

            if (!CheckForGroupsOfThree() && trayOrder.Count >= MaxCells)
            {
                if (GameManager.Instance != null && GameManager.Instance.CurrentMode == GameManager.GameMode.TimeAttack)
                {
                    // In Time Attack, do not lose on full tray
                }
                else
                {
                    TriggerLoseCondition();
                }
            }
        }

        public void RemoveTile(GameObject tile)
        {
            if (tile == null) return;

            SpriteRenderer fSpriteRenderer = ResolveFSpriteRenderer(tile);
            if (fSpriteRenderer != null)
            {
                string fKey = GetSpriteKey(fSpriteRenderer);
                if (!string.IsNullOrEmpty(fKey) && groupedTiles.ContainsKey(fKey))
                {
                    groupedTiles[fKey].Remove(tile);
                }
            }

            trayOrder.Remove(tile);
            AdjustTilePositions();
            currentOrderInLayer--; 
            
            CheckForGroupsOfThree();
        }


        private Vector3 GetTargetPositionForTile(int index)
        {
            Vector3 bottomBarPosition = bottomBar.position;
            float targetPosX = bottomBarPosition.x + horizontalOffset + index * tileSpacing;
            float targetPosY = bottomBarPosition.y + verticalOffset;

            return new Vector3(targetPosX, targetPosY, bottomBarPosition.z);
        }

        private void AdjustTilePositions()
        {
            for (int i = 0; i < trayOrder.Count; i++)
            {
                Vector3 targetPosition = GetTargetPositionForTile(i);
                StartCoroutine(MoveTileToPosition(trayOrder[i], targetPosition));
            }
        }

        private IEnumerator MoveTileToPosition(GameObject tile, Vector3 targetPosition)
        {
            if (tile == null) yield break;

            Vector3 startPosition = tile.transform.position;
            float elapsedTime = 0;

            while (elapsedTime < tileMoveSpeed)
            {
                if (tile == null) yield break;

                tile.transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / tileMoveSpeed));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (tile != null)
            {
                tile.transform.position = targetPosition;
            }
        }

        private bool CheckForGroupsOfThree()
        {
            List<List<GameObject>> matches = new List<List<GameObject>>();

            if (trayOrder.Count < 3) return false;

            for (int i = 0; i <= trayOrder.Count - 3; i++)
            {
                GameObject t1 = trayOrder[i];
                GameObject t2 = trayOrder[i + 1];
                GameObject t3 = trayOrder[i + 2];

                if (t1 == null || t2 == null || t3 == null) continue;

                string k1 = GetSpriteKey(ResolveFSpriteRenderer(t1));
                string k2 = GetSpriteKey(ResolveFSpriteRenderer(t2));
                string k3 = GetSpriteKey(ResolveFSpriteRenderer(t3));

                if (!string.IsNullOrEmpty(k1) && k1 == k2 && k2 == k3)
                {
                    matches.Add(new List<GameObject> { t1, t2, t3 });
                    i += 2;
                }
            }

            if (matches.Count > 0)
            {
                StartCoroutine(RemoveMatchingTiles(matches));
                return true;
            }

            return false;
        }

        private SpriteRenderer ResolveFSpriteRenderer(GameObject tile)
        {
            if (tile == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(fSpriteChildName))
            {
                SpriteRenderer renderer = TryGetChildRenderer(tile.transform, fSpriteChildName);
                if (renderer != null)
                {
                    return renderer;
                }
            }

            foreach (string fallback in FChildFallbackNames)
            {
                if (string.IsNullOrEmpty(fallback) || fallback == fSpriteChildName)
                {
                    continue;
                }

                SpriteRenderer renderer = TryGetChildRenderer(tile.transform, fallback);
                if (renderer != null)
                {
                    return renderer;
                }
            }

            foreach (SpriteRenderer renderer in tile.GetComponentsInChildren<SpriteRenderer>())
            {
                if (renderer.gameObject != tile)
                {
                    return renderer;
                }
            }

            return null;
        }

        private SpriteRenderer TryGetChildRenderer(Transform parent, string childName)
        {
            if (parent == null || string.IsNullOrEmpty(childName))
            {
                return null;
            }

            Transform child = parent.Find(childName);
            return child != null ? child.GetComponent<SpriteRenderer>() : null;
        }

        private IEnumerator RemoveMatchingTiles(List<List<GameObject>> matches)
        {
            foreach (var match in matches)
            {
                foreach (var tile in match)
                {
                    Animator animator = tile.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.SetTrigger("Remove");
                    }
                    StartCoroutine(ScaleDownTile(tile));
                }
            }

            if (animationType == AnimationType.SlowFade)
            {
                yield return new WaitForSeconds(slowFadeDuration);
            }
            else
            {
                yield return new WaitForSeconds(tileMoveSpeed);
            }

            yield return new WaitForSeconds(0.1f);

            foreach (var match in matches)
            {
                foreach (var tile in match)
                {
                    if (tile == null) continue;

                    SpriteRenderer fSpriteRenderer = tile.transform.Find("FSprite").GetComponent<SpriteRenderer>();
                    string fKey = GetSpriteKey(fSpriteRenderer);
                    if (!string.IsNullOrEmpty(fKey) && groupedTiles.ContainsKey(fKey))
                    {
                        groupedTiles[fKey].Remove(tile);
                        trayOrder.Remove(tile);
                        
                        if (TileManager.Instance != null)
                        {
                            TileInteraction interaction = tile.GetComponent<TileInteraction>();
                            if (interaction != null)
                            {
                                TileManager.Instance.HandleTileMatched(interaction);
                            }
                            else
                            {
                                TileManager.Instance.HandleTrayTileDestroyed();
                            }
                        }
                        
                        Destroy(tile);
                    }
                }
            }

            AdjustTilePositions();
        }

        private void TriggerLoseCondition()
        {
            isLocked = true;
            GameManager.Instance?.HandleLose();
            if (TileManager.Instance != null)
            {
                TileManager.Instance.ShowLosePanel();
            }
        }

        private string GetSpriteKey(SpriteRenderer renderer)
        {
            if (renderer == null || renderer.sprite == null)
            {
                return string.Empty;
            }

            return renderer.sprite.name;
        }

        private IEnumerator ScaleDownTile(GameObject tile)
        {
            float duration = 0.3f;
            Vector3 initialScale = tile.transform.localScale;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                if (tile == null) yield break;
                tile.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (tile != null)
            {
                tile.transform.localScale = Vector3.zero;
            }
        }
    }

}
