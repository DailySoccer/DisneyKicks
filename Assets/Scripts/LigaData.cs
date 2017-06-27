using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LigaData {

    public static List<Liga> SkillLevels = new List<Liga> {
        new Liga {
            Name            = "Liga de Barrio",
            SkillLevelUp    = 0,
            SkillLevelDown  = -1
        },
        new Liga {
            Name            = "Liga Regional",
            SkillLevelUp    = 70,
            SkillLevelDown  = 50
        },
        new Liga {
            Name            = "Liga Nacional",
            SkillLevelUp    = 140,
            SkillLevelDown  = 130
        },
        new Liga {
            Name            = "Liga Mundial",
            SkillLevelUp    = 200,
            SkillLevelDown  = 190
        },
    };
}

