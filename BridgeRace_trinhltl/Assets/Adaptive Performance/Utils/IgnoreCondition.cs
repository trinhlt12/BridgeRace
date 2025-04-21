using System;
using System.Collections;
using System.Collections.Generic;
using _GAME.Scripts.Character;
using _GAME.Scripts.Floor;
using UnityEngine;
using UnityEngine.UI;

public class IgnoreCondition : MonoBehaviour
{
    [SerializeField] private Button              ignoreButton;
    private static           List<BotController> bots => FloorManager.Instance.allBots;

    private void Start()
    {
        ignoreButton.onClick.AddListener(OnIgnoreButtonClicked);
    }

    private static void OnIgnoreButtonClicked()
    {
        foreach (var bot in bots)
        {
            bot.IgnoreCondition = true;
        }
    }
}