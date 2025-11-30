using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

namespace TestHM
{
    public class TileManager : MonoBehaviour
    {


        public static TileManager Instance { get; private set; }

        [Header("Prefabs, Sprite")]
        public GameObject horizontalBaseTilePrefab;
        public GameObject verticalBaseTilePrefab;
        [SerializeField]
        public Sprite[] fSprites;

        [Header("Horizontal Layout")]
        public Vector3 horizontalTileScale = new Vector3(0.5f, 0.5f, 0.5f);
        public float horizontalTileOffset = 0.0f;
        public float rowSpacing = 0.3f;
        public int rows = 4;
        public int columns = 6;

        [Header("vertical Layout")]
        public Vector3 verticalTileScale = new Vector3(0.5f, 0.5f, 0.5f);
        public float verticalTileOffset = 0.0f;
        public float columnSpacing = 0.7f;
        public int verticalRows = 7;
        public int verticalColumns = 3;

        [Header("Layer Setting")]
        [Min(1)] public int totalLayers = 2;
        [SerializeField] private Color lockedTint = new Color(0.5f, 0.5f, 0.5f, 1f);

        public List<LayerDimensions> layerDimensions = new List<LayerDimensions>();
        [SerializeField] private string fSpriteChildName = "FSprite";
        [SerializeField] private GameObject panelWin;
        [SerializeField] private GameObject panelLose;
        private static readonly string[] FChildFallbackNames = { "FSprite" };

        [Header("gameplay")]
        [SerializeField] private BottomBarManager bottomBarManager;
        [SerializeField] private float autoPlayDelay = 0.5f;
        [SerializeField] private TextMeshProUGUI timeText;

        private float timeRemaining = 60f;
        private bool isTimerRunning = false;

        public List<TileInteraction> SelectableTiles => selectableTiles;
        public Dictionary<Sprite, List<TileInteraction>> AvailableTilesBySprite => availableTilesBySprite;
        public BottomBarManager BottomBarManager => bottomBarManager;
        public float AutoPlayDelay => autoPlayDelay;

        private AutoplayHandler autoplayHandler;


        // config 
        public GameObject HorizontalBaseTilePrefab => horizontalBaseTilePrefab;
        public GameObject VerticalBaseTilePrefab => verticalBaseTilePrefab;
        public Sprite[] FSprites => fSprites;
        public Vector3 HorizontalTileScale => horizontalTileScale;
        public float HorizontalTileOffset => horizontalTileOffset;
        public float RowSpacing => rowSpacing;
        public int Rows => rows;
        public int Columns => columns;
        public Vector3 VerticalTileScale => verticalTileScale;
        public float VerticalTileOffset => verticalTileOffset;
        public float ColumnSpacing => columnSpacing;
        public int VerticalRows => verticalRows;
        public int VerticalColumns => verticalColumns;
        public int TotalLayers => totalLayers;
        public List<LayerDimensions> LayerDimensions => layerDimensions;
        public string FSpriteChildName => fSpriteChildName;
        public List<LayerState> Layers => layers;
        public Dictionary<TileInteraction, LayerState> TileToLayer => tileToLayer;

        public void SetCurrentLayerIndex(int index) => currentLayerIndex = index;
        public void SetRemainingBoardTiles(int count) => remainingBoardTiles = count;
        public void SetBoardReady(bool ready) => boardReady = ready;

        private BoardGenerator boardGenerator;

        private readonly Dictionary<Sprite, List<TileInteraction>> availableTilesBySprite = new Dictionary<Sprite, List<TileInteraction>>();
        private readonly List<TileInteraction> selectableTiles = new List<TileInteraction>();
        private readonly List<LayerState> layers = new List<LayerState>();
        private readonly Dictionary<TileInteraction, LayerState> tileToLayer = new Dictionary<TileInteraction, LayerState>();

        private int horizontalTileCount;
        private int verticalTileCount;
        private int remainingBoardTiles;
        private bool boardReady;
        private int currentLayerIndex;

        //private GameManager.AutoplayMode requestedAutoplayMode = GameManager.AutoplayMode.None;

        private void Awake()
        {
            Instance = this;
            autoplayHandler = gameObject.AddComponent<AutoplayHandler>();
            autoplayHandler.Initialize(this);
            boardGenerator = gameObject.AddComponent<BoardGenerator>();
            boardGenerator.Initialize(this);
        }

        private void Start()
        {
            if (bottomBarManager == null)
            {
                bottomBarManager = FindObjectOfType<BottomBarManager>();
            }

            horizontalTileCount = rows * columns;
            verticalTileCount = verticalRows * verticalColumns;

            if (!boardGenerator.BuildLayers())
            {
                return;
            }

            StartCoroutine(boardGenerator.GenerateBoard());

            if (GameManager.Instance != null && GameManager.Instance.CurrentMode == GameManager.GameMode.TimeAttack)
            {
                if (timeText == null)
                {
                    GameObject timeObj = GameObject.Find("TimeRemainText");
                    if (timeObj != null)
                    {
                        timeText = timeObj.GetComponent<TextMeshProUGUI>();
                    }
                }

                if (timeText != null)
                {
                    timeText.gameObject.SetActive(true);
                    timeRemaining = 60f;
                    isTimerRunning = true;
                }
            }
            else
            {
                if (timeText == null)
                {
                    GameObject timeObj = GameObject.Find("TimeRemainText");
                    if (timeObj != null)
                    {
                        timeText = timeObj.GetComponent<TextMeshProUGUI>();
                    }
                }
                
                if (timeText != null)
                {
                    timeText.gameObject.SetActive(false);
                }
            }
        }

        private bool isPaused = false;

        public void SetTimerPaused(bool paused)
        {
            isPaused = paused;
        }

        private void Update()
        {
            if (isTimerRunning && !isPaused && !GameManager.Instance.IsGameOver)
            {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0)
                {
                    timeRemaining = 0;
                    isTimerRunning = false;
                    GameManager.Instance.HandleLose();
                    ShowLosePanel();
                }

                if (timeText != null)
                {
                    timeText.text = Mathf.CeilToInt(timeRemaining).ToString();
                }
            }
        }

        public TileInteraction RegisterPlayableTile(GameObject tile, Sprite sprite, LayerState layer, bool locked)
        {
            TileInteraction interaction = tile.GetComponent<TileInteraction>();
            if (interaction == null)
            {
                return null;
            }

            selectableTiles.Add(interaction);
            interaction.Initialize(this, sprite, bottomBarManager, locked);
            interaction.SetLocked(locked);
            ApplyLockState(interaction, locked);
            layer.Tiles.Add(interaction);
            tileToLayer[interaction] = layer;
            interaction.OriginalLayer = layer;


            if (!availableTilesBySprite.ContainsKey(sprite))
            {
                availableTilesBySprite[sprite] = new List<TileInteraction>();
            }

            availableTilesBySprite[sprite].Add(interaction);
            return interaction;
        }

        public void ReturnTileToBoard(TileInteraction interaction)
        {
            if (interaction == null || bottomBarManager == null)
            {
                return;
            }

            bottomBarManager.RemoveTile(interaction.gameObject);

            interaction.IsInTray = false;

            selectableTiles.Add(interaction);
            if (!availableTilesBySprite.ContainsKey(interaction.AssignedSprite))
            {
                availableTilesBySprite[interaction.AssignedSprite] = new List<TileInteraction>();
            }
            availableTilesBySprite[interaction.AssignedSprite].Add(interaction);

            if (interaction.OriginalLayer != null)
            {
                interaction.OriginalLayer.TilesRemaining++;
                remainingBoardTiles = interaction.OriginalLayer.TilesRemaining;
                tileToLayer[interaction] = interaction.OriginalLayer;
            }

            interaction.transform.SetParent(interaction.OriginalParent);
            interaction.transform.position = interaction.OriginalPosition;
            interaction.transform.localScale = interaction.OriginalScale;
            interaction.transform.rotation = interaction.OriginalRotation;

            interaction.SetInteractable(true);
        }


        public void OnTileRemovedFromBoard(TileInteraction interaction)
        {
            if (interaction == null)
            {
                return;
            }

            selectableTiles.Remove(interaction);
            if (availableTilesBySprite.TryGetValue(interaction.AssignedSprite, out var list))
            {
                list.Remove(interaction);
            }

            if (tileToLayer.TryGetValue(interaction, out var layer))
            {
                layer.TilesRemaining = Mathf.Max(0, layer.TilesRemaining - 1);
                remainingBoardTiles = layer.TilesRemaining;
                tileToLayer.Remove(interaction);
            }

            CheckForCompletion();
        }

        public void HandleTrayTileDestroyed()
        {
            CheckForCompletion();
        }

        public void HandleTileMatched(TileInteraction tile)
        {
            if (tile == null) return;
            
            if (tile.OriginalLayer != null)
            {
                tile.OriginalLayer.TilesMatched++;
            }
            
            CheckForCompletion();
        }

        private void CheckForCompletion()
        {
            if (!boardReady || bottomBarManager == null || layers.Count == 0)
            {
                return;
            }

            LayerState currentLayer = layers[currentLayerIndex];
            
            if (currentLayer.TilesMatched < currentLayer.TotalTiles)
            {
                return;
            }

            if (!currentLayer.IsCompleted)
            {
                currentLayer.IsCompleted = true;
            }

            if (currentLayerIndex < layers.Count - 1)
            {
                UnlockNextLayer();
                return;
            }

            if (bottomBarManager.CurrentTileCount == 0)
            {
                GameManager.Instance?.HandleWin();
                if (panelWin != null)
                {
                    panelWin.SetActive(true);
                    Debug.Log("enter");
                }
            }
        }

        public void ShowLosePanel()
        {
            if (panelLose != null)
            {
                panelLose.SetActive(true);
            }
        }

        private void UnlockNextLayer()
        {
            int nextLayerIndex = currentLayerIndex + 1;
            if (nextLayerIndex >= layers.Count)
            {
                return;
            }

            LayerState nextLayer = layers[nextLayerIndex];
            if (nextLayer.IsUnlocked)
            {
                return;
            }

            nextLayer.IsUnlocked = true;
            currentLayerIndex = nextLayerIndex;
            remainingBoardTiles = nextLayer.TilesRemaining;

            foreach (var tile in nextLayer.Tiles)
            {
                if (tile == null)
                {
                    continue;
                }

                ApplyLockState(tile, false);
            }
        }

        public void MaybeStartAutoplay()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            var mode = GameManager.Instance.PendingAutoplayMode;
            if (mode == GameManager.AutoplayMode.None)
            {
                return;
            }

            GameManager.Instance.ClearAutoplayMode();
            if (autoplayHandler != null)
            {
                autoplayHandler.SetAutoplayMode(mode);
            }
        }

        public void ApplyLockState(TileInteraction interaction, bool locked)
        {
            if (interaction == null)
            {
                return;
            }

            interaction.SetLocked(locked);

            GameObject tile = interaction.gameObject;
            Collider2D col = tile.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = !locked;
            }

            SpriteRenderer baseRenderer = tile.GetComponent<SpriteRenderer>();
            if (baseRenderer != null)
            {
                Color baseColor = locked ? new Color(128f / 255f, 128f / 255f, 128f / 255f, 125f / 255f) : Color.white;
                baseRenderer.color = baseColor;
            }

            Transform fSprite = ResolveFSpriteTransform(tile);
            if (fSprite != null)
            {
                SpriteRenderer fRenderer = fSprite.GetComponent<SpriteRenderer>();
                if (fRenderer != null)
                {
                    Color fColor = locked ? new Color(128f / 255f, 128f / 255f, 128f / 255f, 125f / 255f) : Color.white;
                    fRenderer.color = fColor;
                }
            }
        }

        public Transform ResolveFSpriteTransform(GameObject tile)
        {
            if (tile == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(fSpriteChildName))
            {
                Transform child = tile.transform.Find(fSpriteChildName);
                if (child != null)
                {
                    return child;
                }
            }

            foreach (string fallback in FChildFallbackNames)
            {
                if (string.IsNullOrEmpty(fallback) || fallback == fSpriteChildName)
                {
                    continue;
                }

                Transform child = tile.transform.Find(fallback);
                if (child != null)
                {
                    return child;
                }
            }

            foreach (Transform child in tile.transform)
            {
                if (child != tile.transform && child.GetComponent<SpriteRenderer>() != null)
                {
                    return child;
                }
            }

            return null;
        }
    }
}


