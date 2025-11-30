using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TestHM
{
    public class AutoplayHandler : MonoBehaviour
    {
        private TileManager tileManager;
        private Coroutine autoplayRoutine;
        private GameManager.AutoplayMode currentMode = GameManager.AutoplayMode.None;

        public void Initialize(TileManager manager)
        {
            tileManager = manager;
        }

        public void SetAutoplayMode(GameManager.AutoplayMode mode)
        {
            currentMode = mode;
            RestartAutoplayIfNeeded();
        }

        private void RestartAutoplayIfNeeded()
        {
            if (currentMode == GameManager.AutoplayMode.None)
            {
                if (autoplayRoutine != null)
                {
                    StopCoroutine(autoplayRoutine);
                    autoplayRoutine = null;
                }
                return;
            }

            if (autoplayRoutine != null)
            {
                StopCoroutine(autoplayRoutine);
            }

            autoplayRoutine = StartCoroutine(currentMode == GameManager.AutoplayMode.Win
                ? RunAutoPlayWin()
                : RunAutoPlayLose());
        }

        private IEnumerator RunAutoPlayWin()
        {
            Debug.Log("Auto Play : bắt đầu chạy");

            while (currentMode == GameManager.AutoplayMode.Win)
            {
                List<TileInteraction> tilesToClick = null;

                try
                {
                    CleanUpLists();

                    if (TryGetNextMatchingSet(out List<TileInteraction> set))
                    {
                        tilesToClick = set;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"autoPlay Logic Error: {ex.Message}");
                }

                if (tilesToClick != null && tilesToClick.Count == 3)
                {
                    if (tilesToClick[0] != null)
                    {
                        Debug.Log($"auto Play: tìm thấy bộ 3 {tilesToClick[0].AssignedSprite.name}");
                    }

                    foreach (var tile in tilesToClick)
                    {
                        if (tile != null && tile.gameObject != null)
                        {
                            tile.TriggerInteraction();
                            tile.SetLocked(true);
                        }
                        yield return new WaitForSeconds(tileManager.AutoPlayDelay);
                    }

                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            autoplayRoutine = null;
        }

        private IEnumerator RunAutoPlayLose()
        {
            Debug.Log("autoPlay lose: Start game");

            while (currentMode == GameManager.AutoplayMode.Lose)
            {
                if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
                {
                    break;
                }

                CleanUpLists();

                TileInteraction bestTile = null;

                foreach (var tile in tileManager.SelectableTiles)
                {
                    if (tile == null || !tile.IsInteractable || tile.IsLocked) continue;

                    int currentCount = tileManager.BottomBarManager.GetSpecificTileCount(tile.AssignedSprite);
                    if (currentCount < 2)
                    {
                        bestTile = tile;
                        break;
                    }
                }

                if (bestTile == null)
                {
                    bestTile = tileManager.SelectableTiles.FirstOrDefault(t => t != null && t.IsInteractable && !t.IsLocked);
                }

                if (bestTile != null)
                {
                    bestTile.TriggerInteraction();
                    bestTile.SetLocked(true);
                    yield return new WaitForSeconds(tileManager.AutoPlayDelay);
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            autoplayRoutine = null;
        }

        private void CleanUpLists()
        {
            tileManager.SelectableTiles.RemoveAll(t => t == null || t.gameObject == null);

            foreach (var key in tileManager.AvailableTilesBySprite.Keys.ToList())
            {
                var list = tileManager.AvailableTilesBySprite[key];
                if (list != null)
                {
                    list.RemoveAll(t => t == null || t.gameObject == null);
                }
            }
        }

        private bool TryGetNextMatchingSet(out List<TileInteraction> set)
        {
            foreach (var sprite in tileManager.AvailableTilesBySprite.Keys.ToList())
            {
                if (!tileManager.AvailableTilesBySprite.TryGetValue(sprite, out var bucket))
                {
                    continue;
                }

                List<TileInteraction> candidates = bucket.Where(t => t != null && t.IsInteractable && !t.IsLocked).ToList();
                if (candidates.Count >= 3)
                {
                    set = candidates.Take(3).ToList();
                    return true;
                }
            }

            set = null;
            return false;
        }
    }

}
