using Lean.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalEndGame : MonoBehaviour
{
    public Text P1Score;
    public Text P2Score;
    public Text Title;
    public Text Description;

    private ImageManager imageManager;
    private Image back;

    private void Awake()
    {
        imageManager = FindObjectOfType<ImageManager>();
        back = gameObject.transform.Find("Panel").Find("Backing").GetComponent<Image>();
    }

    public void EndGame(int p1Score, int p2Score)
    {
        P1Score.text = p1Score.ToString();
        P2Score.text = p2Score.ToString();

        if (p1Score > p2Score)
        {
            Win();
        }
        else if (p2Score > p1Score)
        {
            Lose();
        }
        else
        {
            Draw();
        }
        GetComponent<LeanWindow>().TurnOn();
    }

    private void Win()
    {
        back.sprite = imageManager.Get("Modal.Back.Win");
        Title.text = "Victory!";
        Description.text = "Congratulations,\nYou won!";
    }

    private void Lose()
    {
        back.sprite = imageManager.Get("Modal.Back.Lose");
        Title.text = "Defeat!";
        Description.text = "Sorry,\nYou lose!";
    }

    private void Draw()
    {
        Title.text = "Draw!";
        Description.text = "This is a tie!";
    }

}
