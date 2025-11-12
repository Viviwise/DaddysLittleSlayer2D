using System;
using UnityEngine;

namespace Script
{
    public class PlayerAnimatior : MonoBehaviour
    {
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private Vector3 lastPosition;
        public static PlayerAnimatior instance { get; set; }

        [Header("Sprite Settings")]
        [Tooltip("Si le sprite est sur un enfant, assignez-le ici")]
        public Transform spriteTransform; // Référence au transform du sprite

        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int IsVertical = Animator.StringToHash("IsVertical");

        private void Awake()
        {
           
            if (spriteTransform == null)
                spriteTransform = transform;

            animator = spriteTransform.GetComponent<Animator>();
            spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
            
            lastPosition = transform.position;

            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public void UpdateAnimationFromMovement()
        {
            Vector3 direction = (transform.position - lastPosition).normalized;

            if (direction.magnitude > 0.01f)
            {
                animator.SetBool(IsWalking, true);

                bool isVertical = Mathf.Abs(direction.y) > Mathf.Abs(direction.x);
                animator.SetBool(IsVertical, isVertical);

                if (!isVertical)
                {
                   
                    spriteRenderer.flipX = direction.x < 0;
                }
            }

            lastPosition = transform.position;
        }

        public void UpdateAnimation(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;

            if (direction.magnitude > 0.01f)
            {
                animator.SetBool(IsWalking, true);

                bool isVertical = Mathf.Abs(direction.y) > Mathf.Abs(direction.x);
                animator.SetBool(IsVertical, isVertical);

                if (!isVertical)
                {
                    spriteRenderer.flipX = direction.x < 0;
                }
            }
            else
            {
                animator.SetBool(IsWalking, false);
            }
        }

        public void StopAnimation()
        {
            animator.SetBool(IsWalking, false);
        }
    }
}
