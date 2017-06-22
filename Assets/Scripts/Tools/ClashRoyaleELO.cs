using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClashRoyaleELO {
    static public int Result (int ownELO, int ownPoints, int opponentELO, int opponentPoints) {
        int result = 0;

        // GANAR
        if (ownPoints > opponentPoints) {
            result = Ganar(ownELO, opponentELO);
        }
        // PERDER
        else if (ownPoints < opponentPoints) {
            result = Perder(ownELO, opponentELO);
        }
        // EMPATAR
        else {
            // Devolvemos el "result" por defecto (0)
        }

        Debug.Log(string.Format("ELO: {0} vs {1}: {2} = {3}", ownELO, opponentELO, 
            (ownPoints > opponentPoints) ? "<color=green>GANAR</color>" : 
            (ownPoints < opponentPoints) ? "<color=red>PERDER</color>":
            "EMPATAR",
            result));

        return result;
    }

    static private int Ganar(int ownELO, int opponentELO) {
        float diff = opponentELO - ownELO;
        float result = KGanar * (1.0f - (1.0f / (1.0f+(Mathf.Pow(10, diff/400.0f)))));
        return Mathf.RoundToInt(result);
    }

    static private int Perder(int ownELO, int opponentELO) {
        float diff = opponentELO - ownELO;
        float result = KPerder(ownELO) * (0.0f - (1.0f / (1.0f+(Mathf.Pow(10, diff/400.0f)))));
        return Mathf.RoundToInt(result);
    }

    static private float KGanar {
        get { return 59.0f; }
    }

    static private float KPerder(int ownELO) {
        return 59.0f;

        float K = 0;
        if (ownELO > 30 && ownELO <= 1000) {
            K = Mathf.Round(ownELO/20.0f);
        }
        return K;
    }

    static public void TEST() {
        Debug.Assert(25 == ClashRoyaleELO.Result(100, 1, 50, 0), "ClashRoyaleELO.Result(100, 1, 50, 0) != 25");
        Debug.Assert(-34 == ClashRoyaleELO.Result(100, 0, 50, 1), "ClashRoyaleELO.Result(100, 0, 50, 1) != -34");

        Debug.Assert(34 == ClashRoyaleELO.Result(50, 1, 100, 0), "ClashRoyaleELO.Result(50, 1, 100, 0) != 34");
        Debug.Assert(-25 == ClashRoyaleELO.Result(50, 0, 100, 1), "ClashRoyaleELO.Result(50, 0, 100, 1) != -25");
    }
}
