using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchView : MonoBehaviour, IMatchView
{
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private Image[] player1Balls, player2Balls;
    [SerializeField] private TextMeshProUGUI turnText, endMessageText;

    [SerializeField] private Sprite blackBallSprite;
    [SerializeField] private Sprite whiteBallSprite;


    public void UpdateUI(int pocketedBallsP1, int pocketedBallsP2, GameState currentState)
    {
        turnText.text = currentState == GameState.Player1Turn ? "P1 TURN" : "P2 TURN";

        for (int i = 0; i < pocketedBallsP1; i++)
        {
            Image ballImage = player1Balls[i].GetComponent<Image>();
            ballImage.sprite = blackBallSprite;
            ballImage.enabled = true;
        }
        for (int i = player2Balls.Length - 1; i >= player2Balls.Length - pocketedBallsP2; i--)
        {
            Image ballImage = player2Balls[i].GetComponent<Image>();
            ballImage.sprite = whiteBallSprite;
            ballImage.enabled = true;
        }

        CheckGameEnd(pocketedBallsP1, pocketedBallsP2);
    }

    private void CheckGameEnd(int pocketedBallsP1, int pocketedBallsP2)
    {
        if (pocketedBallsP1 == 8 || pocketedBallsP2 == 8)
        {
            endMessageText.text = pocketedBallsP1 == 8 ? "PLAYER 1 WON" : "PLAYER 2 WON";
            endGamePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
