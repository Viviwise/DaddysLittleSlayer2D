using System.Collections;
using Script;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private static readonly int Attack = Animator.StringToHash("Attack");

    [Header("Systems")]
    public TilemapManager tilemapManager;
    public AttackRange attackRange;

    [Header("Battle UI")]
    
    public TextMeshProUGUI dialogueText;
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;
    public Button healButton;
    public Button attack1Button;
    public Button attack2Button;

    [Header("panel UI")]
    public GameObject mapUIPanel;
    public GameObject battleUIPanel;

    
    [Header("Loadout")] 
    public PlayerLoadout playerLoadout;

    private bool _inBattle;
    private Unit _playerUnit;
    private Unit _currentEnemyUnit;
    private BattleState _battleState;
    
    

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        GameObject player = tilemapManager.player;
        _playerUnit = player.GetComponent<Unit>();

        if (_playerUnit == null)
        {
            Debug.LogError("Le joueur n'a pas de composant Unit !");
        }

        battleUIPanel.SetActive(false);
        mapUIPanel.SetActive(true);
    }

    public void StartBattle(Enemy enemy)
    {
        if (_inBattle) return;

        _inBattle = true;
        _currentEnemyUnit = enemy.GetComponent<Unit>();

        if (_currentEnemyUnit == null)
        {
            Debug.LogError("L'ennemi n'a pas de composant Unit !");
            return;
        }

        tilemapManager.enabled = false;
        attackRange.enabled = false;
        tilemapManager.overlayTilemap.ClearAllTiles();

        mapUIPanel.SetActive(false);
        battleUIPanel.SetActive(true);

        StartCoroutine(SetupBattle());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator SetupBattle()
    {
        EquipWeaponFromLoadout();

        playerHUD.SetHUD(_playerUnit);
        enemyHUD.SetHUD(_currentEnemyUnit);

        SetupAttackButtons();

        dialogueText.text = "Un " + _currentEnemyUnit.unitName + " apparaît !";

        yield return new WaitForSeconds(2f);

        _battleState = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void EquipWeaponFromLoadout()
    {
        _playerUnit.equippedItem = playerLoadout.selectedWeapon;

        //Debug.LogError("Aucune arme dans le loadout !");
    }

    void SetupAttackButtons()
    {
        if (_playerUnit.equippedItem == null) return;

        InventoryItemData weapon = _playerUnit.equippedItem;

        TextMeshProUGUI attack1Text = attack1Button.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI attack2Text = attack2Button.GetComponentInChildren<TextMeshProUGUI>();

        if (weapon.attack1 != null)
        {
            attack1Text.text = weapon.attack1.attackName + " (" + weapon.attack1.damage + ")";
            attack1Button.gameObject.SetActive(true);
        }

        if (weapon.attack2 != null)
        {
            attack2Text.text = weapon.attack2.attackName + " (" + weapon.attack2.damage + ")";
            attack2Button.gameObject.SetActive(true);
        }

        bool hasHealItem = false;
        foreach (var item in playerLoadout.selectedConsumables)
        {
            if (item != null && item.itemType == ItemType.Consumable)
            {
                hasHealItem = true;
                break;
            }
        }
        healButton.gameObject.SetActive(hasHealItem);
    }

    void PlayerTurn()
    {
        dialogueText.text = "À ton tour !";
        attack1Button.interactable = true;
        attack2Button.interactable = true;
        healButton.interactable = true;
    }

    public void OnAttack1Button()
    {
        if (_battleState != BattleState.PLAYERTURN) return;

        if (_playerUnit.equippedItem?.attack1 != null)
        {
            StartCoroutine(PlayerAttack(
                _playerUnit.equippedItem.attack1.damage,
                _playerUnit.equippedItem.attack1.damageMyself
            ));
        }
    }

    public void OnAttack2Button()
    {
        if (_battleState != BattleState.PLAYERTURN) return;

        if (_playerUnit.equippedItem?.attack2 != null)
        {
            StartCoroutine(PlayerAttack(
                _playerUnit.equippedItem.attack2.damage,
                _playerUnit.equippedItem.attack2.damageMyself
            ));
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator PlayerAttack(int damage, int selfDamage)
    {
        attack1Button.interactable = false;
        attack2Button.interactable = false;
        healButton.interactable = false;

        dialogueText.text = "Tu attaques !";

        yield return new WaitForSeconds(0.5f);

        bool enemyDead = _currentEnemyUnit.TakeDamage(damage);
        enemyHUD.SetPV(_currentEnemyUnit.currentPV);

        if (selfDamage > 0)
        {
            _playerUnit.TakeDamage(selfDamage);
            playerHUD.SetPV(_playerUnit.currentPV);
        }

        yield return new WaitForSeconds(1f);

        if (enemyDead)
        {
            _battleState = BattleState.WON;
            StartCoroutine(EndBattle(true));
        }
        else
        {
            _battleState = BattleState.ENNEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    public void OnHealButton()
    {
        if (_battleState != BattleState.PLAYERTURN) return;

        InventoryItemData healItem = null;
        int healIndex = -1;

        for (int i = 0; i < playerLoadout.selectedConsumables.Count; i++)
        {
            var item = playerLoadout.selectedConsumables[i];
            if (item != null && item.itemType == ItemType.Consumable)
            {
                healItem = item;
                healIndex = i;
                break;
            }
        }

        if (healItem != null)
        {
            StartCoroutine(PlayerHeal(healItem, healIndex));
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator PlayerHeal(InventoryItemData healItem, int index)
    {
        attack1Button.interactable = false;
        attack2Button.interactable = false;
        healButton.interactable = false;

        _playerUnit.Heal(healItem.healAmount);
        playerHUD.SetPV(_playerUnit.currentPV);

        dialogueText.text = "Tu utilises " + healItem.itemName + " (+"+healItem.healAmount+" PV)";

        playerLoadout.selectedConsumables[index] = null;
        healButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        _battleState = BattleState.ENNEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator EnemyTurn()
    {
        dialogueText.text = _currentEnemyUnit.unitName + " attaque !";
        
        Animator enemyAnimator = _currentEnemyUnit.GetComponent<Animator>();
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger(Attack);
        }

        yield return new WaitForSeconds(1f);

        bool playerDead = _playerUnit.TakeDamage(_currentEnemyUnit.damage);
        playerHUD.SetPV(_playerUnit.currentPV);

        yield return new WaitForSeconds(1f);

        if (playerDead)
        {
            _battleState = BattleState.LOST;
            StartCoroutine(EndBattle(false));
        }
        else
        {
            _battleState = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }
    IEnumerator DeathSequence()
    {
        yield return StartCoroutine(playerHUD.FadeToBlack(2f));

        SceneManager.LoadScene("DeathScene");
    }

    IEnumerator EndBattle(bool won)
    {
        if (won)
        {
            dialogueText.text = "Victoire ! " + _currentEnemyUnit.unitName + " vaincu !";
            
            Destroy(_currentEnemyUnit.gameObject);
        }
        else
        {
            dialogueText.text = "Défaite... Tu as été vaincu.";
            StartCoroutine(DeathSequence());

        }

        yield return new WaitForSeconds(3f);

        battleUIPanel.SetActive(false);
        mapUIPanel.SetActive(true);

        tilemapManager.enabled = true;
        attackRange.enabled = true;

        _inBattle = false;
        _currentEnemyUnit = null;
    }
    
}
