/*using System.Collections;
using System.Collections.Generic;   
using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour
{
    [SerializeField] private float maxMana = 100;
    [SerializeField] private float currentMana;
    [SerializeField] public Image manaBar;

    void Start()
    {
        if (maxMana == 0)
        {
            maxMana = 100;
        }
        currentMana = maxMana;
    }
    
    void Update()
    {
        if (currentMana >= maxMana)
        {
            currentMana = maxMana;
        }

        if (maxMana > 0)
        {
            manaBar.fillAmount = currentMana / maxMana;
        }
    }
}
*/
