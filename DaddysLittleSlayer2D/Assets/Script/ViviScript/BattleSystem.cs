using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENNEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject ennemyPrefab;

    public Transform playerBattleStation;
    public Transform ennemyBattleStation;

    Unit playerUnit;
    Unit ennemyUnit;

    public Text dialogueText;
    public BattleHUD playerHUD;
    public BattleHUD ennemyHUD;

    public BattleState state;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

   IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject ennemyGO = Instantiate(ennemyPrefab, ennemyBattleStation);
        ennemyUnit = ennemyGO.GetComponent<Unit>();

        dialogueText.text = "Un terrible " + ennemyUnit.unitName + " approche !";
        
        playerHUD.SetHUD(playerUnit);
        ennemyHUD.SetHUD(ennemyUnit);

        yield return new WaitForSeconds(2f);
        
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = ennemyUnit.TakeDamage(playerUnit.damage);
        
        ennemyHUD.SetPV(ennemyUnit.currentPV);
        dialogueText.text = "Attaque réussi !!";
        
        yield return new WaitForSeconds(2f);

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

    IEnumerator EnemyTurn()
    {
        dialogueText.text = ennemyUnit.unitName + " attaque";
        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(ennemyUnit.damage);
        
        playerHUD.SetPV(playerUnit.currentPV);
        yield return new WaitForSeconds(1f);

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
        {
            dialogueText.text = "Tu as vaincu le " + ennemyUnit.unitName;
            
        }else if (state == BattleState.LOST)
        {
            dialogueText.text = "Tu as été dévoré par un " + ennemyUnit.unitName;
        }
    }
    
    void PlayerTurn()
    {
        dialogueText.text = "Choisis une action";
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(5);
        
        playerHUD.SetPV(playerUnit.currentPV);
        dialogueText.text = "Tu te sens mieux d'un coup !";
        
        yield return new WaitForSeconds(2f);
        
        state = BattleState.ENNEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public void OnAttackButton()
    {
     if (state != BattleState.PLAYERTURN)
         return;

     StartCoroutine(PlayerAttack());
    }
    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }
}

