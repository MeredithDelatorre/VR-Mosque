using UnityEngine;

public class RandomAnimatorStart : MonoBehaviour
{
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.Normal;

            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            float randomTime = Random.Range(0f, 1f);
            animator.Play(state.fullPathHash, 0, randomTime);
        }
    }
}
