using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;

    // Method to set the max health and the current health to the slider
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    // Method to update the health value on the slider
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}