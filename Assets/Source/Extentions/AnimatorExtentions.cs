using UnityEngine;

public static class AnimatorExtentions
{
    public static void ForcePlay(this Animator animator, string stateName)
    {
        animator.Play(stateName);
        animator.Update(0);
    }
}
