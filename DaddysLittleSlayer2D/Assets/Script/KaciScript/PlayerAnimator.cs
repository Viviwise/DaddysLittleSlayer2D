using UnityEngine;

namespace Script
{
    public class PlayerAnimatior : MonoBehaviour
    {
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        public static PlayerAnimatior instance { get; set; }

        [Header("Sprite Settings")]
        public Transform spriteTransform;

        [Header("Debug")]
        public bool showDebugLogs = true;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (spriteTransform == null)
            {
                spriteTransform = transform;
                animator = GetComponentInChildren<Animator>();
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
            else
            {
                animator = spriteTransform.GetComponent<Animator>();
                spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
            }

            if (showDebugLogs)
            {
                Debug.Log($"✅ PlayerAnimator sur {spriteTransform.name}");
                Debug.Log($"   Animator: {(animator != null ? "✅" : "❌")}");
                Debug.Log($"   SpriteRenderer: {(spriteRenderer != null ? "✅" : "❌")}");
            }

            SetIdle();
        }

        public void SetMovementDirection(Vector2 direction)
        {
            if (animator == null)
            {
                Debug.LogError("❌ Animator manquant !");
                return;
            }

            if (direction.sqrMagnitude < 0.01f)
            {
                SetIdle();
                return;
            }

            // Déterminer la direction principale
            string directionName = GetDirectionName(direction);

            // Activer le mouvement
            animator.SetBool("IsMoving", true);
            
            // Définir la direction (4 bools)
            animator.SetBool("IsUp", directionName == "Up");
            animator.SetBool("IsDown", directionName == "Down");
            animator.SetBool("IsLeft", directionName == "Left");
            animator.SetBool("IsRight", directionName == "Right");

            if (showDebugLogs)
            {
                Debug.Log($"🎮 Direction: {GetDirectionEmoji(directionName)} {directionName}");
            }
        }

        private string GetDirectionName(Vector2 direction)
        {
            // Normaliser
            direction.Normalize();

            // Déterminer la direction dominante
            float absX = Mathf.Abs(direction.x);
            float absY = Mathf.Abs(direction.y);

            if (absY > absX)
            {
                // Vertical
                return direction.y > 0 ? "Up" : "Down";
            }
            else
            {
                // Horizontal
                return direction.x > 0 ? "Right" : "Left";
            }
        }

        private string GetDirectionEmoji(string direction)
        {
            return direction switch
            {
                "Up" => "⬆️",
                "Down" => "⬇️",
                "Left" => "⬅️",
                "Right" => "➡️",
                _ => "❓"
            };
        }

        public void SetIdle()
        {
            if (animator == null) return;

            animator.SetBool("IsMoving", false);
            animator.SetBool("IsUp", false);
            animator.SetBool("IsDown", false);
            animator.SetBool("IsLeft", false);
            animator.SetBool("IsRight", false);

            if (showDebugLogs)
            {
                Debug.Log("🟠 IDLE");
            }
        }

        public void StopAnimation() => SetIdle();
    }
}
