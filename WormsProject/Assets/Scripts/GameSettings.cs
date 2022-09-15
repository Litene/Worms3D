using UnityEngine;

public class GameSettings : MonoBehaviour {
    public int amountOfPlayers;
    public int amountOfWorms;
    private const int DefaultPlayers = 2;
    private const int DefaultWorms = 1;
    private void Awake() => DontDestroyOnLoad(this);

    private void Start() {
        amountOfPlayers = DefaultPlayers;
        amountOfWorms = DefaultWorms;
    }
    public void SetAmountOfWorms(int worms) => amountOfWorms = worms + 1;
    public void SetAmountOfPlayers(int players) => amountOfPlayers = players + 2;

}
