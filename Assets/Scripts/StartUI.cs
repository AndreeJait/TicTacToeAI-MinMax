using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    [Header("UI References :")]
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private Button firstPlay;
    [SerializeField] private Button secondPlay;

    [Header("Board Reference :")]
    [SerializeField] private Board board;

    private void Start()
    {
        firstPlay.onClick.AddListener(() =>
        {
            board.canPlay = true;
            board.isAiTurn = false;
            board.currentMark = Mark.O;
            uiCanvas.SetActive(false);
        });
        secondPlay.onClick.AddListener(() =>
        {
            board.canPlay = true;
            board.isAiTurn = true;
            board.currentMark = Mark.X;
            board.BestMove();
            uiCanvas.SetActive(false);
        });
        uiCanvas.SetActive(true);
    }
    private void OnDestroy()
    {
        firstPlay.onClick.RemoveAllListeners();
        secondPlay.onClick.RemoveAllListeners();
    }
}
