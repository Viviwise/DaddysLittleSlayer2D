using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Script
{
    public class AttackRange : MonoBehaviour
    {
        public int attackRange;
        public TilemapManager tilemapManager;
        private bool _isAttacking = false;
        public Tilemap tilemapattack;
        private bool[] _attackCells;
        
        [Header("Detection Settings")]
        public float detectionRadius = 0.5f;

        public void Start()
        {
            _isAttacking = false;
        }

        private void Update()
        {
            if (_isAttacking)
            {
                HandleAttackClick();
            }
        }

        private void HandleAttackClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Camera cameraMain = Camera.main;
                if (!cameraMain) return;
        
                Vector3 mouseWorldPos = cameraMain.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0;
        
                int clickedIndex = tilemapManager.GetIndex(mouseWorldPos);
                int[] attackRangeArray = GetAttackRange();
        
                if (clickedIndex >= 0 && clickedIndex < attackRangeArray.Length && attackRangeArray[clickedIndex] > 0)
                {
                    Vector3 targetPos = tilemapManager.GetWorldPos(clickedIndex);
            
                    Enemy enemy = DetectEnemy(targetPos);
                    if (enemy != null)
                    {
                        Debug.Log("Ennemi détecté ! Démarrage du combat.");
                
                        GameManager.Instance.StartBattle(enemy);
                
                        tilemapManager.overlayTilemap.ClearAllTiles();
                        _isAttacking = false;
                    }
                    else
                    {
                        Debug.Log("Pas d'ennemi à cette position");
                    }
                }
            }
        }

        private Enemy DetectEnemy(Vector3 position)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, detectionRadius);

            foreach (Collider2D collider in colliders)
            {
                if (collider == null) continue;

                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    return enemy; 
                }
            }

            return null;
        }



        public void ToogleAttack()
        {
            _isAttacking = !_isAttacking;
            
            tilemapManager.SetAttackMode(_isAttacking);

            if (_isAttacking)
            {
                DrawAttackRange(GetAttackRange());
            }
            else
            {
                tilemapManager.overlayTilemap.ClearAllTiles();
            }
        }

        void InitAttackCells()
        {
            tilemapManager.bounds = tilemapattack.cellBounds;
            tilemapManager.width = tilemapManager.bounds.size.x;
            var tiles = tilemapattack.GetTilesBlock(tilemapManager.bounds);
           
            _attackCells = new bool[tiles.Length];
            for (int i = 0; i < tiles.Length; i++)
            {
                _attackCells[i] = tiles[i] != null;
            }
        }

        int[] GetAttackRange()
        {
            InitAttackCells();

            var tested = new bool[_attackCells.Length];
            var range = new int[_attackCells.Length];
            
            var q = new Queue<Vector2Int>();
            q.Enqueue(new Vector2Int(0, tilemapManager.GetPlayerIndex()));

            while (q.Count > 0)
            {
                var current = q.Dequeue();
                var depth = current.x;
                var index = current.y;

                if (!tilemapManager.IsValidIndex(index)) continue;
                if (tested[index]) continue;
                tested[index] = true;

                if (!_attackCells[index]) continue;
                
                range[index] = depth;
                
                var nextDepth = depth + 1;
                
                if (nextDepth > attackRange) continue;
                
                var xy = tilemapManager.GetXY(index);
                var upIndex = tilemapManager.GetIndex(xy + Vector2Int.up);
                var downIndex = tilemapManager.GetIndex(xy + Vector2Int.down);
                var leftIndex = tilemapManager.GetIndex(xy + Vector2Int.left);
                var rightIndex = tilemapManager.GetIndex(xy + Vector2Int.right);
                
                q.Enqueue(new Vector2Int(nextDepth, upIndex));
                q.Enqueue(new Vector2Int(nextDepth, downIndex));
                q.Enqueue(new Vector2Int(nextDepth, leftIndex));
                q.Enqueue(new Vector2Int(nextDepth, rightIndex));
            }
            return range;
        }
        
        private void DrawAttackRange(int[] range)
        {
            tilemapManager.overlayTilemap.ClearAllTiles();

            for (int i = 0; i < range.Length; i++)
            {
                var possible = range[i];
                if (possible > 0)
                {
                    tilemapManager.DrawHighlight(i, possible);
                }
            }
        }
    }
}
