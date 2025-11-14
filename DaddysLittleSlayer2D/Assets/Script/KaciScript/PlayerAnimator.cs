using UnityEngine;

namespace Script
{
    [DefaultExecutionOrder(-150)]
    public class PlayerAnimator : MonoBehaviour
    {
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        public static PlayerAnimator instance { get; private set; }

        public Transform spriteTransform;
        
        public bool showDebugLogs = true;

        private static readonly int DirectionXParam = Animator.StringToHash("DirectionX");
        private static readonly int DirectionYParam = Animator.StringToHash("DirectionY");

        private void Awake()
        {
            /*
            if (instance != null && instance != this)
            {
                Debug.Log(instance);
                Destroy(gameObject);
                return;
            }
            */
            instance = this;
            if (spriteTransform == null)
                spriteTransform = transform;

            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            if (_animator == null)
                Debug.LogError(" Animator manquant sur " + spriteTransform.name);

            if (_spriteRenderer == null)
                Debug.LogError("SpriteRenderer manquant sur " + spriteTransform.name);
        }

        public void SetMovementDirection(Vector2 gridDirection)
        {
            if (_animator == null) return;

            if (gridDirection.sqrMagnitude < 0.01f)
            {
                if (showDebugLogs)
                    Debug.Log(" Pas de mouvement détecté");
                return;
            }

            gridDirection.Normalize();

            string mainDirection = GetMainDirection(gridDirection);

            switch (mainDirection)
            {
                case "Up":
                    _animator.SetFloat(DirectionXParam, 0);
                    _animator.SetFloat(DirectionYParam, 1);
                    break;

                case "Down":
                    _animator.SetFloat(DirectionXParam, 0);
                    _animator.SetFloat(DirectionYParam, -1);
                    break;

                case "Left":
                    _animator.SetFloat(DirectionXParam, -1);
                    _animator.SetFloat(DirectionYParam, 0);
                    break;

                case "Right":
                    _animator.SetFloat(DirectionXParam, 1);
                    _animator.SetFloat(DirectionYParam, 0);
                    break;
            }

           
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
        
        public void StopAnimation()
        {
            if (showDebugLogs);
        }
        
    }
}

