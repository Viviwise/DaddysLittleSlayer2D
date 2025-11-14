using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    public Button item1Button;
    public Button item2Button;
    public Button attack1Button;
    public Button attack2Button;

    

    // variable pour se souvenir de l'item sélectionné (index dans selectedItems)
    private int selectedItemIndex = -1;

    // bool pour l'animator
    private bool monsterIsAttacking;
    private bool monsterIsHurted;

    private bool playerIsAttacking;
    private bool playerIsHealing;
    private bool playerIsHurted;

    void Start()
    {
        // caches initiaux
        healButton.gameObject.SetActive(false);
        attack1Button.gameObject.SetActive(false);
        attack2Button.gameObject.SetActive(false);
        item1Button.gameObject.SetActive(false);
        item2Button.gameObject.SetActive(false);

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

        dialogueText.text = "Un terrible " + ennemyUnit.unitName + " veut vous croquer !";

        playerHUD.SetHUD(playerUnit);
        ennemyHUD.SetHUD(ennemyUnit);

        SetupAttackButtons(); // met à jour les textes d'attaques

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void EquipWeaponFromLoadout()
    {
        /*foreach (var item in playerLoadout.selectedItems)
        {
            if (item != null && item.itemType == ItemType.Weapon)
            {
                playerUnit.equippedItem = item;
                return;
            }
        }*/

        playerUnit.equippedItem = Inventory.Instance.WeaponSlot.Data;

        Debug.LogError("Aucune arme trouvée dans les objets !");
    }

    void SetupAttackButtons()
    {
        InventoryItemData weapon = playerUnit.equippedItem;

        TextMeshProUGUI attack1Text = attack1Button.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI attack2Text = attack2Button.GetComponentInChildren<TextMeshProUGUI>();

        attack1Text.text = "—";
        attack2Text.text = "—";

        if (weapon != null)
        {
            if (weapon.attack1 != null)
                attack1Text.text = weapon.attack1.attackName + " " + weapon.attack1.damage;

            if (weapon.attack2 != null)
                attack2Text.text = weapon.attack2.attackName + " " + weapon.attack2.damage;
        }
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choisis un objet pour attaquer.";

        attack1Button.gameObject.SetActive(false);
        attack2Button.gameObject.SetActive(false);

        item1Button.gameObject.SetActive(true);
        item2Button.gameObject.SetActive(true);

        bool hasHeal = false;
        foreach (var it in playerLoadout.selectedItems)
        {
            if (it != null && it.itemType == ItemType.Consumable)
            {
                hasHeal = true;
                break;
            }
        }
        healButton.gameObject.SetActive(hasHeal);

        TextMeshProUGUI item1Text = item1Button.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI item2Text = item2Button.GetComponentInChildren<TextMeshProUGUI>();

        item1Text.text = (playerLoadout.selectedItems.Length > 0 && playerLoadout.selectedItems[0] != null) ? playerLoadout.selectedItems[0].itemName : "Vide";
        item2Text.text = (playerLoadout.selectedItems.Length > 1 && playerLoadout.selectedItems[1] != null) ? playerLoadout.selectedItems[1].itemName : "Vide";

        selectedItemIndex = -1;
    }

    public void OnItem1Button() { OnItemButtonPressed(0); }
    public void OnItem2Button() { OnItemButtonPressed(1); }

    void OnItemButtonPressed(int index)
    {
        if (state != BattleState.PLAYERTURN) return;

        if (index < 0 || index >= playerLoadout.selectedItems.Length)
        {
            dialogueText.text = "Item invalide.";
            return;
        }

        InventoryItemData item = playerLoadout.selectedItems[index];
        if (item == null)
        {
            dialogueText.text = "Aucun objet à cet emplacement.";
            return;
        }

        if (item.itemType != ItemType.Weapon)
        {
            dialogueText.text = "Ce n'est pas une arme.";
            return;
        }

        selectedItemIndex = index;
        playerUnit.equippedItem = item;
        
        ShowAttacksForItem(item);
    }

    void ShowAttacksForItem(InventoryItemData item)
    {
        dialogueText.text = "Choisis une attaque pour " + item.itemName;

        item1Button.gameObject.SetActive(false);
        item2Button.gameObject.SetActive(false);

        attack1Button.gameObject.SetActive(true);
        attack2Button.gameObject.SetActive(true);
        healButton.gameObject.SetActive(true);

        TextMeshProUGUI attack1Text = attack1Button.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI attack2Text = attack2Button.GetComponentInChildren<TextMeshProUGUI>();

        if (item.attack1 != null)
            attack1Text.text = item.attack1.attackName + " (" + item.attack1.damage + ")";
        else
            attack1Text.text = "Aucune";

        if (item.attack2 != null)
            attack2Text.text = item.attack2.attackName + " (" + item.attack2.damage + ")";
        else
            attack2Text.text = "Aucune";
    }

    IEnumerator PlayerAttack(int damage, int damageMyself)
    {
        playerIsAttacking = true;

        attack1Button.gameObject.SetActive(false);
        attack2Button.gameObject.SetActive(false);
        healButton.gameObject.SetActive(false);

        bool isDead = ennemyUnit.TakeDamage(damage);
        playerUnit.TakeDamage(damageMyself);

        ennemyHUD.SetPV(ennemyUnit.currentPV);
        playerHUD.SetPV(playerUnit.currentPV);
        dialogueText.text = "Attaque réussie !";

        yield return new WaitForSeconds(1.5f);

        //_monsterIsHurted = true;

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

        if (selectedItemIndex < 0)
        {
            dialogueText.text = "Choisis d'abord un objet.";
            return;
        }

        InventoryItemData item = playerLoadout.selectedItems[selectedItemIndex];
        if (item == null || item.attack1 == null)
        {
            dialogueText.text = "Attaque invalide.";
            return;
        }

        StartCoroutine(PlayerAttack(item.attack1.damage, item.attack1.damageMyself));
    }

    public void OnAttack2Button()
    {
        if (state != BattleState.PLAYERTURN) return;

        if (selectedItemIndex < 0)
        {
            dialogueText.text = "Choisis d'abord un objet.";
            return;
        }

        InventoryItemData item = playerLoadout.selectedItems[selectedItemIndex];
        if (item == null || item.attack2 == null)
        {
            dialogueText.text = "Attaque invalide.";
            return;
        }

        StartCoroutine(PlayerAttack(item.attack2.damage, item.attack2.damageMyself));
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN) return;

        InventoryItemData healItem = Inventory.Instance.ConsumableSlot.Data;
        /*int healIndex = -1; // pour retirer l'objet après utiliser*/
        /*for (int i = 0; i < playerLoadout.selectedItems.Length; i++)
        {
            var item = playerLoadout.selectedItems[i];
            if (item != null && item.itemType == ItemType.Consumable)
            {
                healItem = item;
                healIndex = i;
                break;
            }
        }*/

        if (healItem != null)
        {
            healButton.gameObject.SetActive(false);

            //playerLoadout.selectedItems[healIndex] = null;

            StartCoroutine(PlayerHeal(healItem));
        }
        else
        {
            dialogueText.text = "Aucun objet de soin disponible !";
        }
    }

    IEnumerator PlayerHeal(InventoryItemData healItem)
    {
        playerIsHealing = true;

        playerUnit.Heal(healItem.healAmount);

        playerHUD.SetPV(playerUnit.currentPV);
        dialogueText.text = "Tu utilises " + healItem.itemName + " et récupères " + healItem.healAmount + " PV !";

        attack1Button.gameObject.SetActive(false);
        attack2Button.gameObject.SetActive(false);
        item1Button.gameObject.SetActive(false);
        item2Button.gameObject.SetActive(false);

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
    IEnumerator DeathSequence()
    {
        yield return StartCoroutine(playerHUD.FadeToBlack(2f));

        SceneManager.LoadScene("DeathScene");
    }

    

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "Tu as vaincu le " + ennemyUnit.unitName;
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "Tu as été dévoré par " + ennemyUnit.unitName;
            StartCoroutine(DeathSequence());
        }
    }

}
