using UnityEngine;

public sealed class LifecycleProbe: MonoBehaviour {
  private void Awake() {
    Debug.Log("LifecycleProbe : Awake",this);
  }

  private void OnEnable() {
    Debug.Log("LifecycleProbe : OnEnable",this);
  }

  private void Start() {
    Debug.Log("LifecycleProbe : Start",this);
  }

  private void OnDisable() {
    Debug.Log("LifecycleProbe : OnDisable",this);
  }

  private void OnDestroy() {
    Debug.Log("LifecycleProbe : OnDestroy",this);
  }
}