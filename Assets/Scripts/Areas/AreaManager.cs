using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class AreaManager {

  private static Dictionary<int, List<DifficultyArea>> areas = new Dictionary<int, List<DifficultyArea>>();

  public static void AddArea(DifficultyArea area) {
    int difficulty = (int)area.difficulty;

    if (!areas.ContainsKey( difficulty )) {
      areas.Add( difficulty, new List<DifficultyArea>() );
    }

    areas[difficulty].Add( area );
  }

  public static void RemArea(DifficultyArea area) {
    int difficulty = (int)area.difficulty;

    areas[difficulty].Remove( area );
  }

  public static Vector3 GetRandomPoint(Difficulty difficulty, GameMode gameMode, float _xparam = 1f) {
    int diff = (int)difficulty;

    if (!areas.ContainsKey( diff )) {
      throw new ArgumentOutOfRangeException( "difficulty", "No area registered for difficulty " + difficulty );
    }

    var modeAreas = from area in areas[diff]
      where ((area.goalKeeper && gameMode == GameMode.GoalKeeper)
              || (!area.goalKeeper && gameMode == GameMode.Shooter))
      select area;

    var availableAreas = modeAreas.ToList();



    // Choose an area based on the ratio of total
    // space used by all this difficulty's areas.
    float totalArea = 0;
    foreach (var area in availableAreas) {
      totalArea += area.Area.width * area.Area.height;
    }

    float rand = UnityEngine.Random.Range( 0f, 1.0f );
    float accRatio = 0;
    foreach (var area in availableAreas) {
      accRatio += (area.Area.width * area.Area.height) / totalArea;
      if (accRatio > rand) {
        if(_xparam == 1f)
          return area.GetRandomPoint();
        else
          return area.GetRandomPointParamX(_xparam); //NOTA: solo sirve para el modo portero del kicks
      }
    }

    throw new ArithmeticException( "Could not calculate an area to return a value from..." );
  }

}
