using UnityEngine;
using System.Collections;

namespace TestHM
{

    public class TileInteraction : MonoBehaviour
    {
        private Animator animator;
        private BottomBarManager bottomBarManager;
        private TileManager tileManager;
        private Sprite assignedSprite;
        private bool isInteractable = true;
        private bool isLocked;
        private static bool globalInputLocked;
        private Coroutine clickRoutine;

        public bool IsInteractable => isInteractable;
        public bool IsLocked => isLocked;
        public Sprite AssignedSprite => assignedSprite;
        public bool IsInTray { get; set; }
        public LayerState OriginalLayer { get; set; }
        public Vector3 OriginalPosition { get; private set; }
        public Transform OriginalParent { get; private set; }
        public Vector3 OriginalScale { get; private set; }
        public Quaternion OriginalRotation { get; private set; }



        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Initialize(TileManager manager, Sprite sprite, BottomBarManager bar, bool locked)
        {
            tileManager = manager;
            assignedSprite = sprite;
            bottomBarManager = bar;
            isLocked = locked;
            OriginalPosition = transform.position;
            OriginalParent = transform.parent;
            OriginalScale = transform.localScale;
            OriginalRotation = transform.rotation;
        }

        public void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
        }


        public void SetLocked(bool locked)
        {
            isLocked = locked;
        }

        private void OnMouseEnter()
        {
            if (!CanInteract())
            {
                return;
            }

            animator?.SetBool("isHovering", true);
        }

        private void OnMouseExit()
        {
            if (!CanInteract())
            {
                return;
            }

            animator?.SetBool("isHovering", false);
        }

        private void OnMouseDown()
        {
            TriggerInteraction();
        }

        public void TriggerInteraction()
        {
            if (!CanInteract())
            {
                return;
            }

            if (clickRoutine == null)
            {
                clickRoutine = StartCoroutine(ClickAnimation());
            }
        }

        public static void SetGlobalInputLock(bool locked)
        {
            globalInputLocked = locked;
        }

        private bool CanInteract()
        {
            if (IsInTray)
            {
                if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
                {
                    return false;
                }
                return true;
            }

            if (!isInteractable || isLocked || globalInputLocked)
            {
                return false;
            }

            if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            {
                return false;
            }

            return true;
        }

        private IEnumerator ClickAnimation()
        {
            if (animator != null)
            {
                animator.SetBool("isClicked", true);
                animator.SetBool("isHovering", false);
            }

            yield return new WaitForSeconds(0.2f);

            if (IsInTray)
            {
                if (GameManager.Instance != null && GameManager.Instance.CurrentMode == GameManager.GameMode.TimeAttack)
                {
                    if (tileManager != null)
                    {
                        tileManager.ReturnTileToBoard(this);
                    }
                    else
                    {
                        Debug.LogError("tileManager reference missing on TileInteraction");
                    }
                }
            }
            else
            {
                if (bottomBarManager != null)
                {
                    bottomBarManager.AddTile(gameObject);
                }
                else
                {
                    Debug.LogError("bottomBarManager reference missing on TileInteraction");
                }
            }

            clickRoutine = null;
        }
    }

}