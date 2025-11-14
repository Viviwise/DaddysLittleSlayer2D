using System;
using System.Collections.Generic;
using DG.Tweening;
using Script;
using UnityEngine;
using UnityEngine.Tilemaps;
using ColorUtility = Unity.VisualScripting.ColorUtility;


public class TilemapManager: MonoBehaviour
{
    public Tilemap walkableTilemap;
    public Tilemap overlayTilemap;
    public Tile highlightTile;
    public Tile[] obstacleTiles;
    
    public GameObject player;
    public int playerRange = 3;
    private PlayerAnimator _playerAnimController;

    
    public bool isInit;
    private bool _isMoving = false ;
    private bool _isAttacking = false;
    

    private Tile[] _highlightTiles;
    public  BoundsInt bounds;
    private bool[] _walkableCells;
    public  int width;
    private int[] _possibleCells;
    public static TilemapManager instance { get; set; }
    

    private void Awake()
    {

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
        Init();
        _isMoving = false;

        
    }
    

    
    private void OnDrawGizmos()
    {
        try
        {
            Init();
        }
        catch (Exception error)
        {
            Debug.LogWarning($"TilemapManager ${error.Message} : ${error.StackTrace}");
        }
    }
    
    void Init(bool forceInit = false)
    {
        if (isInit && !forceInit) return;
        isInit = true;
        
        if (!walkableTilemap) throw new ArgumentNullException(nameof(walkableTilemap));
        if (!overlayTilemap) throw new ArgumentNullException(nameof(overlayTilemap));
        if (!highlightTile) throw new ArgumentNullException(nameof(highlightTile));
        if (!player) throw new ArgumentNullException(nameof(player));
        
        _playerAnimController = player.GetComponent<PlayerAnimator>();
        if (!_playerAnimController)
        {
            _playerAnimController = player.AddComponent<PlayerAnimator>();
        }
        InitWalkableCells();
    }

    void InitWalkableCells()
    {
        bounds = walkableTilemap.cellBounds;
        width = bounds.size.x;
        var tiles = walkableTilemap.GetTilesBlock(bounds);
    
        _walkableCells = new bool[tiles.Length];
        for (int i = 0; i < tiles.Length ; i++)
            _walkableCells[i] = tiles[i] != null && tiles[i] != IsObstacleTile(tiles[i]);    }

    bool IsObstacleTile(TileBase tile)
    {
        if (obstacleTiles == null || obstacleTiles.Length == 0) 
            return false ;
        foreach (Tile obstacleTile in obstacleTiles)
        {
            if (tile == obstacleTile)
            {
                return true;
            }
            ;
        }
        return false;
    }

    Tile[] GetHighlightTiles()
    {
        if (_highlightTiles != null) return _highlightTiles;
        
        Color[] colors = {
            Color.red,
            Color.orange,
            Color.yellow,
            Color.green,
            Color.blue,
            Color.blueViolet,
        };

        _highlightTiles = Array.ConvertAll(colors, color =>
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = highlightTile.sprite;
            tile.color = ColorUtility.WithAlpha(color, 0.2f);
        
            return tile;
        });
        
        return _highlightTiles;
    }

    public bool IsValidIndex(int index) => index >= 0 && index < _walkableCells.Length;
    
    public Vector2Int GetXY(int index) => new Vector2Int(index % width, index / width);
    
    Vector3Int GetTilePos(Vector2Int xy) => new Vector3Int(xy.x, xy.y, 0) + bounds.position;
    Vector3Int GetTilePos(int index) => GetTilePos(GetXY(index));
    Vector3Int GetTilePos(Vector3 worldPos) => walkableTilemap.WorldToCell(worldPos);
    
    public Vector3 GetWorldPos(int index) => walkableTilemap.GetCellCenterWorld(GetTilePos(index));
    
    public  int GetIndex(Vector2Int xy) => xy.x + xy.y * width;
    public int GetIndex(Vector3Int tilePos) => (tilePos.y - bounds.yMin) * width + (tilePos.x - bounds.xMin);
    public int GetIndex(Vector3 worldPos) => GetIndex(GetTilePos(worldPos));
    public int GetPlayerIndex() => GetIndex(player.transform.position);
    
    public bool IsInMovementMode => _isMoving;
    public bool IsInAttackMode => _isAttacking;
    

    // ReSharper disable Unity.PerformanceAnalysis
    int[] GetWalkableRange()
    {
        InitWalkableCells();
            
        var tested = new bool[_walkableCells.Length];
        var range = new int[_walkableCells.Length];
        
        var q = new Queue<Vector2Int>();
        q.Enqueue(new Vector2Int(0, GetPlayerIndex()));
        
        while (q.Count > 0)
        {
            var current = q.Dequeue();
            var depth = current.x;
            var index = current.y;
            
            if (!IsValidIndex(index)) continue;
            
            if (tested[index]) continue;
            tested[index] = true;

            if (!_walkableCells[index]) continue;
            
            range[index] = depth;
            
            var nextDepth = depth + 1;
            if (nextDepth > playerRange) continue;
            
            var xy = GetXY(index);
            var upIndex = GetIndex(xy + Vector2Int.up);
            var downIndex = GetIndex(xy + Vector2Int.down);
            var leftIndex = GetIndex(xy + Vector2Int.left);
            var rightIndex = GetIndex(xy + Vector2Int.right);
            
            q.Enqueue(new Vector2Int(nextDepth, upIndex));
            q.Enqueue(new Vector2Int(nextDepth, downIndex));
            q.Enqueue(new Vector2Int(nextDepth, leftIndex));
            q.Enqueue(new Vector2Int(nextDepth, rightIndex));
        }

        return range;
    }

    private void DrawWalkableRange(int[] range)
    {
        overlayTilemap.ClearAllTiles();

        for (int i = 0; i < range.Length; i++)
        {
            var possible = range[i];
            if (possible > 0)
            {
                DrawHighlight(i, possible);
            }
        }
    }

    private void Start()
    {
        _isMoving = false;
        Init();
        overlayTilemap.ClearAllTiles();
    }

    private void Update()
    {
       
        if (_isMoving && !_isAttacking)
        {
            PlayerMovement();
        }
    }
    
    private void PlayerMovement()
    {
        
        if (!_isMoving || _isAttacking)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log($"Update mousePosition:{Input.mousePosition}");

            Camera cameraMain = Camera.main;
            if (!cameraMain) return;

            Debug.Log($"Update cameraMain:{cameraMain}");
            Vector3 mouseWorldPos = cameraMain.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            InitWalkableCells();
            int clickedIndex = GetIndex(mouseWorldPos);

            MovePlayerTo(clickedIndex);
        }
    }
    
    
    public void SetEnabledMouvement()
    {
        Init(); 
        if (!_isMoving && _isAttacking)
        {
            _isAttacking = false;
            Debug.Log("Mode attaque désactivé automatiquement");
        }

        _isMoving = !_isMoving;
        Debug.Log($"Mouvement {(_isMoving ? "activé" : "désactivé")}");

        if (_isMoving)
        {
            DrawWalkableRange(GetWalkableRange());
        }
        else
        {
            overlayTilemap.ClearAllTiles();
        }
    }
    
    public void SetAttackMode(bool enabled)
    {
        _isAttacking = enabled;
        Debug.Log($"Mode attaque {(enabled ? "activé" : "désactivé")}");

        
        if (enabled && _isMoving)
        {
            _isMoving = false;
            overlayTilemap.ClearAllTiles(); 
            Debug.Log("Mode mouvement désactivé automatiquement");
        }
    }
    
    private int FindNextStepInPath(int currentIndex, int[] values)
    {
        var xy = GetXY(currentIndex);

        
        int GetVal(int i) => IsValidIndex(i) && values[i] > 0 ? values[i] : int.MaxValue;

        int upIndex = GetIndex(xy + Vector2Int.up);
        int downIndex = GetIndex(xy + Vector2Int.down);
        int leftIndex = GetIndex(xy + Vector2Int.left);
        int rightIndex = GetIndex(xy + Vector2Int.right);

        int upVal = GetVal(upIndex);
        int downVal = GetVal(downIndex);
        int leftVal = GetVal(leftIndex);
        int rightVal = GetVal(rightIndex);

        int min = Mathf.Min(upVal, downVal, leftVal, rightVal);

        if (min == int.MaxValue)
            throw new Exception($"Aucun chemin trouvé depuis l'index {currentIndex}");

        if (min == upVal) return upIndex;
        if (min == downVal) return downIndex;
        if (min == leftVal) return leftIndex;
        return rightIndex;
    }

private void MovePlayerTo(int targetIndex)
{
    if (!_isMoving || _isAttacking)
    {
        Debug.Log("Mouvement annulé - mode non actif");
        return;
    }

    var values = GetWalkableRange();
    if (values[targetIndex] == 0)
    {
        Debug.Log("Case non accessible");
        return;
    }

    _isMoving = false;
    List<int> path = new List<int>();
    int currentIndex = targetIndex;
    path.Add(currentIndex);

    while (values[currentIndex] > 1)
    {
        currentIndex = FindNextStepInPath(currentIndex, values);
        path.Add(currentIndex);
    }

    path.Reverse();
    
    overlayTilemap.ClearAllTiles();
    
    Sequence moveSequence = DOTween.Sequence();

    for (int i = 0; i < path.Count - 1; i++) // ⚠️ 
    {
        int currentStepIndex = path[i];
        int nextStepIndex = path[i + 1]; 
        
        Vector3 targetPosition = walkableTilemap.GetCellCenterWorld(
            new Vector3Int(
                nextStepIndex % width + bounds.xMin,
                nextStepIndex / width + bounds.yMin,
                0
            )
        );

        int currentX = currentStepIndex % width;
        int currentY = currentStepIndex / width;
        int nextX = nextStepIndex % width;
        int nextY = nextStepIndex / width;
        
        int deltaX = nextX - currentX;
        int deltaY = nextY - currentY;
        
        moveSequence.Append(
            player.transform.DOMove(targetPosition, 0.3f)
                .SetEase(Ease.Linear)
                .OnStart(() =>
                {
                    Vector2 gridDirection = new Vector2(deltaX, deltaY);

                    if (gridDirection.sqrMagnitude > 0.0001f)
                    {
                        gridDirection.Normalize();
                        
                        PlayerAnimator.instance?.SetMovementDirection(gridDirection);
                    }
                })
        );
    }

    moveSequence.OnComplete(() =>
    {
        PlayerAnimator.instance?.StopAnimation();
    });

    moveSequence.Play();
}

    public void DrawHighlight(int index, int value)
    {
        
        if (!IsValidIndex(index)) return;
        
        var highlightTiles = GetHighlightTiles();
        var tile = highlightTiles[value % highlightTiles.Length];
        
        overlayTilemap.SetTile(GetTilePos(index), tile);
        
    }
}