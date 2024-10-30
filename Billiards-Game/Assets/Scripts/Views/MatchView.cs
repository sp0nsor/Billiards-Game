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

        UpdatePocketedBallsUI(player1Balls, pocketedBallsP1, blackBallSprite);
        UpdatePocketedBallsUI(player2Balls, pocketedBallsP2, whiteBallSprite, reverseOrder: true);

        CheckGameEnd(pocketedBallsP1, pocketedBallsP2);
    }

    private void UpdatePocketedBallsUI(Image[] ballObjects, int pocketedBalls, Sprite ballSprite, bool reverseOrder = false)
    {
        int count = ballObjects.Length;
        for (int i = 0; i < pocketedBalls; i++)
        {
            int index = reverseOrder ? count - 1 - i : i;

            if (index < 0 || index >= count) continue;

            Image ballImage = ballObjects[index].GetComponent<Image>();
            if (ballImage != null)
            {
                ballImage.sprite = ballSprite;
                ballImage.enabled = true;
                ballImage.color = new Color(ballImage.color.r, ballImage.color.g, ballImage.color.b, 1f);
            }
        }
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
