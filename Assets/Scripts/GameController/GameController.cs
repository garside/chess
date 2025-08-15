using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(AudioManager))]
public class GameController : MonoBehaviour {
  public static GameController Instance => GameObject.FindWithTag("GameController").GetComponent<GameController>();

  public UnityEvent OnReady;

  public bool IsReady { get; private set; }

  private void ReadyUp() {
    if (IsReady) return;
    IsReady = true;
    OnReady.Invoke();
  }

  private void Start() {

  }

  private void Awake() {
  }
}
