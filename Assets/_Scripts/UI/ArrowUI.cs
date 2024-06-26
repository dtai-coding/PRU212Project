using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Image currentChargeBar;
    [SerializeField] private Image Bounce1;
    [SerializeField] private Image Bounce2;
    [SerializeField] private Image Bounce3;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        int bounceCount = player.bounceCount;


        Bounce1.gameObject.SetActive(bounceCount == 1);
        Bounce2.gameObject.SetActive(bounceCount == 2);
        Bounce3.gameObject.SetActive(bounceCount == 3);

        currentChargeBar.fillAmount = player.chargeTime / player.chargeTimeSet;
    }
}
