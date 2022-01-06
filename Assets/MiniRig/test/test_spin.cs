using UnityEngine;
public class test_spin : MonoBehaviour {
    void Update() => transform.Rotate( new Vector3( 45, 15, 95 ) * Time.deltaTime, Space.Self );
}