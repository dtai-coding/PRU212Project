using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;
    void Start()
    {
        player = FindObjectOfType<Player>();
        totalHealthBar.fillAmount = player.deathCount / 10;
    }

    void Update()
    {
		try
		{
			currentHealthBar.fillAmount = player.deathCount / 10f; // Suppress error on this line
		}
		catch (System.NullReferenceException)
		{
			// Suppress the NullReferenceException
		}
	}
}
