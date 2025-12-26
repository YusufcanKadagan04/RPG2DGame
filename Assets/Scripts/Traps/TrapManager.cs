using UnityEngine;

public class TrapManager : MonoBehaviour
{
    public void ActivateTraps()
    {
        Animator[] childAnimators = GetComponentsInChildren<Animator>();
        foreach (Animator anim in childAnimators)
        {
            anim.SetTrigger("PopUp");
        }
    }

    void Update()
    {
        
    }
}