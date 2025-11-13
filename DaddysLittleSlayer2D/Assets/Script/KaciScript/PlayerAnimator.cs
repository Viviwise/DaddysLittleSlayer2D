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
                animator = GetComponent<Animator>();
                spriteRenderer = GetComponent<SpriteRenderer>();

                if (animator == null) animator = GetComponentInChildren<Animator>();
                if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
            else
            {
                animator = spriteTransform.GetComponent<Animator>();
                spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
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

            if (spriteRenderer == null)
            {
                Debug.LogError("❌ SpriteRenderer manquant !");
                return;
            }

            if (direction.sqrMagnitude < 0.01f)
            {
                SetIdle();
                return;
            }

            direction.Normalize();

            string directionName = GetMainDirection(direction);

            animator.SetBool("IsMoving", true);

            animator.SetBool("IsUp", directionName == "Up");
            animator.SetBool("IsDown", directionName == "Down");
            animator.SetBool("IsLeft", directionName == "Left");
            animator.SetBool("IsRight", directionName == "Right");

            ApplyFlip(directionName, direction);
            
        }

        private string GetMainDirection(Vector2 direction)
        {
            float absX = Mathf.Abs(direction.x);
            float absY = Mathf.Abs(direction.y);

            if (absY > absX)
            {
                
                return direction.y > 0 ? "Up" : "Down";
            }
            else
            {
                return direction.x > 0 ? "Right" : "Left";
            }
        }

        private void ApplyFlip(string directionName, Vector2 direction)
        {
            switch (directionName)
            {
                case "Up":
                    spriteRenderer.flipY = false;
                    spriteRenderer.flipX = true;
                    break;

                case "Down":
                    spriteRenderer.flipY = false;
                    spriteRenderer.flipX = true;
                    break;

                case "Left":
                    spriteRenderer.flipY = false;
                    spriteRenderer.flipX = false;
                    break;

                case "Right":
                    spriteRenderer.flipY = false;
                    spriteRenderer.flipX = false;
                    break;
            }
        }

       

        public void SetIdle()
        {
            if (animator == null) return;

            animator.SetBool("IsMoving", false);
            animator.SetBool("IsUp", false);
            animator.SetBool("IsDown", false);
            animator.SetBool("IsLeft", false);
            animator.SetBool("IsRight", false);

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = false;
                spriteRenderer.flipY = false;
            }

            if (showDebugLogs)
            {
                Debug.Log("🟠 IDLE");
            }
        }

        public void StopAnimation() => SetIdle();
    }
}
