using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { START, PLAYERTURN, ENNEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject ennemyPrefab;

    [Header("Positions")]
    public Transform playerBattleStation;
    public Transform ennemyBattleStation;

    [Header("UI")]
    public Text dialogueText;
    public BattleHUD playerHUD;
    public BattleHUD ennemyHUD;

    [Header("Loadout")]
    public PlayerLoadout playerLoadout;

    public BattleState state;

    Unit playerUnit;
    Unit ennemyUnit;

    [Header("Buttons")]
    public Button healButton;
    public Button attack1Button;
    public Button attack2Button;
    
    // bool pour l'animator
    private bool monsterIsAttacking;
    private bool monsterIsHurted;
    
    private bool playerIsAttacking;
    private bool playerIsHealing;
    private bool playerIsHurted;
    
    void Start()
    {
        healButton.gameObject.SetActive(false);
        attack1Button.gameObject.SetActive(false);
        attack2Button.gameObject.SetActive(false);

        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }
    

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject ennemyGO = Instantiate(ennemyPrefab, ennemyBattleStation);
        ennemyUnit = ennemyGO.GetComponent<Unit>(); 

        EquipWeaponFromLoadout();

        dialogueText.text = "Un terrible " + ennemyUnit.unitName + " veut vous croquez !";

        playerHUD.SetHUD(playerUnit);
        ennemyHUD.SetHUD(ennemyUnit);

        SetupAttackButtons();

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
        
    }
    

    void EquipWeaponFromLoadout()
    {
        foreach (var item in playerLoadout.selectedItems)
        {
            if (item != null && item.itemType == ItemType.Weapon)
            {
                playerUnit.equippedItem = item;
                return;
            }
        }

        Debug.LogError("Aucune arme trouvée dans les 3 objets !");
    }



    void SetupAttackButtons()
    {
        if (playerUnit.equippedItem == null)
        {
            Debug.LogError("Je n'ai aucune arme");
            return;
        }

        ItemData weapon = playerUnit.equippedItem;

        TextMeshProUGUI attack1Text = attack1Button.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI attack2Text = attack2Button.GetComponentInChildren<TextMeshProUGUI>();
        
        
        if (weapon.attack1 != null)
            attack1Text.text = weapon.attack1.attackName + " " + weapon.attack1.damage;

        if (weapon.attack2 != null)
            attack2Text.text = weapon.attack2.attackName + " " + weapon.attack2.damage;
    }
    

    void PlayerTurn()
    {
        dialogueText.text = "Choisis une action";

        attack1Button.gameObject.SetActive(true);
        attack2Button.gameObject.SetActive(true);
        healButton.gameObject.SetActive(true);
    }
    

    IEnumerator PlayerAttack(int damage, int damageMyself)
    {
        playerIsAttacking =  true;
        
        attack1Button.gameObject.SetActive(false);
        attack2Button.gameObject.SetActive(false);
        healButton.gameObject.SetActive(false);

        bool isDead = ennemyUnit.TakeDamage(damage);
        playerUnit.TakeDamage(damageMyself);

        ennemyHUD.SetPV(ennemyUnit.currentPV);
        playerHUD.SetPV(playerUnit.currentPV);
        dialogueText.text = "Attaque réussie !";

        yield return new WaitForSeconds(1.5f);

        monsterIsHurted = true;

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENNEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
    public void OnAttack1Button()
    {
        if (state != BattleState.PLAYERTURN) return;

        if (playerUnit.equippedItem != null && playerUnit.equippedItem.attack1 != null)
            StartCoroutine(PlayerAttack(playerUnit.equippedItem.attack1.damage, playerUnit.equippedItem.attack1.damageMyself));
    }

    public void OnAttack2Button()
    {
        if (state != BattleState.PLAYERTURN) return;

        if (playerUnit.equippedItem != null && playerUnit.equippedItem.attack2 != null)
            StartCoroutine(PlayerAttack(playerUnit.equippedItem.attack2.damage, playerUnit.equippedItem.attack2.damageMyself));
    }
    

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN) return;
        

        ItemData healItem = null;
        int healIndex = -1; // pour retirer l'objet après utiliser
        for (int i = 0; i < playerLoadout.selectedItems.Length; i++)
        {
            var item = playerLoadout.selectedItems[i];
            if (item != null && item.itemType == ItemType.Consumable)
            {
                healItem = item;
                healIndex = i;
                break;
            }
        }

        if (healItem != null)
        {
            healButton.gameObject.SetActive(false);

            playerLoadout.selectedItems[healIndex] = null;

            StartCoroutine(PlayerHeal(healItem));
        }
        else
        {
            dialogueText.text = "Aucun objet de soin disponible !";
        }
    }
    
    IEnumerator PlayerHeal(ItemData healItem)
    {      
        playerIsHealing = true;
        
        playerUnit.Heal(healItem.healAmount);

        playerHUD.SetPV(playerUnit.currentPV);
        dialogueText.text = "Tu utilises " + healItem.itemName + " et récupères " + healItem.healAmount + " PV !";

        attack1Button.gameObject.SetActive(false);
        attack2Button.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        state = BattleState.ENNEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    
    
    IEnumerator EnemyTurn()
    {
        dialogueText.text = ennemyUnit.unitName + " attaque !";
        
        monsterIsAttacking = true;
        
        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(ennemyUnit.damage);

        playerHUD.SetPV(playerUnit.currentPV);

        yield return new WaitForSeconds(1f);
        
        playerIsHurted = true;

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }

        if (ennemyUnit.currentPV <= ennemyUnit.maxPV / 2)
        {
            ennemyUnit.damage += 3;
        }
    }
    

    void EndBattle()
    {
        if (state == BattleState.WON)
            dialogueText.text = "Tu as vaincu le " + ennemyUnit.unitName;

        else if (state == BattleState.LOST)
            dialogueText.text = "Tu as été dévoré par " + ennemyUnit.unitName;
    }
}
