using System;
using UnityEngine;

public struct PlayerInfo {

  public int Attempts { get;set;}

  public int Points { get; set; }

}

public interface IPlayerService {
  PlayerInfo GetPlayerInfo();
  void RegisterListener(Action<PlayerInfo> listener);
  void UnregisterListener(Action<PlayerInfo> listener);
  void AddPoints(int points);
  void SetLives(int lives);
  int[] GetRecompensas();
  bool IsGameOver();
  void SetGameOver();
  void TryShotResult(ShotResult _shotResult);
}
