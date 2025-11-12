using UnityEngine;

public class MonsterAnimation : MonoBehaviour
{
    public Animator animator;
    public bool isIdle = true;
  

    // Update is called once per frame
    void Update()
    {
        if (isIdle)
        {
            animator.SetBool("isIdle", true);
        }
    }
}
