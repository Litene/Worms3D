using UnityEngine;
using UnityEngine.Serialization;

public class Nest : MonoBehaviour {
    private PlayerColor colorOfNest;
    public PlayerColor GetNestColor() {
        return colorOfNest;
    }
    public void SetNestColor(PlayerColor color) {
        this.colorOfNest = color;
    }
}
