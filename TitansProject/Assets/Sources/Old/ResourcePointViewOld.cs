using UnityEngine;

public class ResourcePointViewOld : MonoBehaviour {
    public int Count = 10;

    public void Collect(int count = 1) {
        Count -= 1;
        if (Count <= 0)
            Remove();
    }

    public void Remove() {
        Destroy(gameObject, 0.2f);
    }
}
