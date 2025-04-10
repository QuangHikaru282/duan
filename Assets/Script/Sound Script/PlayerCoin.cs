using UnityEngine;

public class PlayerCoin : MonoBehaviour
{
    public int currentCoins = 0;

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        Debug.Log("Đã thu thập tiền! Tổng: " + currentCoins);
    }
}