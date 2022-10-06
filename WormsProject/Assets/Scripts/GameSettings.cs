using UnityEngine;

public class GameSettings : MonoBehaviour {
    public int AmountOfPlayers;
    public int AmountOfWorms;
    // private const int DefaultPlayers = 2;
    // private const int DefaultWorms = 1;
    private void Awake() => DontDestroyOnLoad(this);

    public void SetAmountOfWorms(int players, int worms) {
        AmountOfPlayers = players;
        AmountOfWorms = worms;
    }
    

}
