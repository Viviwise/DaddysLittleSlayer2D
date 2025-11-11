using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnBasedCombat : MonoBehaviour
{
 [SerializeField] GameObject player;
    [SerializeField] GameObject monster;

    [SerializeField] int playerHealth = 10;
    [SerializeField] int maxPlayerHealth = 10;
    [SerializeField] int monsterHealth = 10;
    [SerializeField] int maxMonsterHealth = 10;

    [SerializeField] TextMeshProUGUI playerHealthText;
    [SerializeField] TextMeshProUGUI monsterHealthText;

    [SerializeField] Button attackButton;
    [SerializeField] Button healButton;

    private bool playerTurn = true;

    private Vector3 playerStartPosition;
    private Vector3 monsterStartPosition;

    void Start()
    {
        playerStartPosition = player.transform.position;
        monsterStartPosition = monster.transform.position;

        UpdateUI();
        attackButton.onClick.AddListener(PlayerAttack);
        healButton.onClick.AddListener(PlayerHeal);
    }
void PlayerAttack()
    {
        if (!playerTurn) 
        {
            return;
        }

        StartCoroutine(DoAttack(player, monster, () =>
        {
            monsterHealth -= 2;

            if (monsterHealth <= 0)
            {
                monsterHealth = 0;
                UpdateUI();
                attackButton.interactable = false;
                return;
            }

            playerTurn = false;
            UpdateUI();
            Invoke(nameof(MonsterAttack), 1f);
        }));
    }


    void PlayerHeal()
    {
        if (!playerTurn) 
        {
            return;
        }

        StartCoroutine(DoHeal(player, player, () =>
        {
            monsterHealth += 1;
            
            playerTurn = false;
            UpdateUI();
            Invoke(nameof(MonsterHeal), 1f);
        }));
    }
    
    
    void MonsterAttack()
    {
        StartCoroutine(DoAttack(monster, player, () =>
        {
            playerHealth -= 1;

            if (playerHealth <= 0)
            {
                playerHealth = 0;
                UpdateUI();
                attackButton.interactable = false;
                return;
            }

            playerTurn = true;
            UpdateUI();
        }));
    }
    
    void MonsterHeal()
    {
        StartCoroutine(DoAttack(monster, monster, () =>
        {
            playerHealth += 1;
            
            playerTurn = true;
            UpdateUI();
        }));
    }
    
IEnumerator DoAttack(GameObject attacker, GameObject target, System.Action onComplete)
    {
        Vector3 attackerStart = (attacker == player) ? playerStartPosition : monsterStartPosition;
        Vector3 targetStart = (target == player) ? playerStartPosition : monsterStartPosition;

        Vector3 attackPos = attackerStart + (targetStart - attackerStart).normalized * 0.5f;
        Vector3 hitPushPos = targetStart + (targetStart - attackerStart).normalized * 0.3f;

        yield return MoveOverTime(attacker, attackerStart, attackPos, 0.1f);
        yield return MoveOverTime(attacker, attackPos, attackerStart, 0.1f);

        yield return MoveOverTime(target, targetStart, hitPushPos, 0.05f);
        yield return MoveOverTime(target, hitPushPos, targetStart, 0.1f);

        onComplete?.Invoke();
    }


    IEnumerator DoHeal(GameObject attacker, GameObject target, System.Action onComplete)
    {
        Vector3 attackerStart = (attacker == player) ? playerStartPosition : monsterStartPosition;
        Vector3 targetStart = (target == player) ? playerStartPosition : monsterStartPosition;

        Vector3 attackPos = attackerStart + (targetStart - attackerStart).normalized * 0.7f;
        Vector3 hitPushPos = targetStart + (targetStart - attackerStart).normalized * 0.1f;

        yield return MoveOverTime(attacker, attackerStart, attackPos, 0.1f);
        yield return MoveOverTime(attacker, attackPos, attackerStart, 0.1f);

        yield return MoveOverTime(target, targetStart, hitPushPos, 0.05f);
        yield return MoveOverTime(target, hitPushPos, targetStart, 0.1f);

        onComplete?.Invoke();
    }

    IEnumerator MoveOverTime(GameObject target, Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            target.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.transform.position = endPos;
    }

    void UpdateUI()
    {
        playerHealthText.text = playerHealth + "/" + maxPlayerHealth;
        monsterHealthText.text = monsterHealth + "/" + maxMonsterHealth;
    }


    private void Update()
    {
        if (playerHealth == 0)
        {
            Destroy(player);
        }
        else if (monsterHealth == 0)
        {
            Destroy(monster);
        }
    }
}
