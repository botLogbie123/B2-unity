using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private int currenthealth;
    public int maxhealth = 125;
    public Image healthBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currenthealth = maxhealth;
    }

    public void TakenDamage(int damage) 
    {
        currenthealth = currenthealth - damage;
        float healthFill = currenthealth / maxhealth;
        Debug.Log("Health: " + healthFill);
        healthBar.fillAmount = healthFill;
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
