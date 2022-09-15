using UnityEngine;
using UnityEngine.Serialization;

public class Nest : MonoBehaviour {
    [FormerlySerializedAs("colorOfNest")] [SerializeField]private PlayerColor colorOfNest;

    public PlayerColor GetNestColor() {
        return colorOfNest;
    }

    public void SetNestColor(PlayerColor color) {
        this.colorOfNest = color;
    }
}
