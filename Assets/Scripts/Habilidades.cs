using UnityEngine;
using System.Collections.Generic;

public static class Habilidades
{

    public enum Skills
    {
        Vista_halcon = 0,
        Mago_balon = 1,
        VIP = 2,
        Prima = 3,
        Goleador = 4,
        Premonicion = 5,
        Practico = 6,
        Heroico = 7,
        Barrera = 8,
        BarreraPro = 9
    }

    public const int NUM_HABILIDADES_LANZADOR = 5;
    public const int NUM_HABILIDADES_PORTERO = 5;

    private static bool delayAlpha = false;
    private static bool delayAlphaRival = false;

    public static void EndRound(bool _fail)
    {
        if(GameplayService.networked && GameplayService.IsGoalkeeper())
        {
            delayAlpha = _fail;
        }
        else if(GameplayService.networked)
        {
            delayAlphaRival = _fail;
        }

        if(!GameplayService.networked)
        {
            delayAlpha = _fail;
        }
    }

    public static void ResetPremonicion()
    {
        delayAlpha = false;
        delayAlphaRival = false;
    }

    public static string GetAllHabilidadesTexto()
    {
        string result = "";
        Skills[] skills = GetAllHabilidades();

        for(int i = 0 ; i < skills.Length ; i++)
        {
            result += SkillToString(skills[i]);
            if(i < (skills.Length - 1))
            {
                result += ", ";
            }
        }
        return result;
    }

    public static string SkillToString(Skills _skill)
    {
        string result = "";
        switch(_skill)
        {
            case Skills.Barrera: result = LocalizacionManager.instance.GetTexto(146); break;
            case Skills.BarreraPro: result = LocalizacionManager.instance.GetTexto(147); break;
            case Skills.Goleador: result = LocalizacionManager.instance.GetTexto(148); break;
            case Skills.Heroico: result = LocalizacionManager.instance.GetTexto(149); break;
            case Skills.Mago_balon: result = LocalizacionManager.instance.GetTexto(150); break;
            case Skills.Practico: result = LocalizacionManager.instance.GetTexto(151); break;
            case Skills.Premonicion: result = LocalizacionManager.instance.GetTexto(152); break;
            case Skills.Prima: result = LocalizacionManager.instance.GetTexto(153); break;
            case Skills.VIP: result = LocalizacionManager.instance.GetTexto(154); break;
            case Skills.Vista_halcon: result = LocalizacionManager.instance.GetTexto(155); break;
        }
        return result;
    }

    public static string SkillDescription(Skills _skill)
    {
        string result = "";
        switch(_skill)
        {
            case Skills.Barrera: result = LocalizacionManager.instance.GetTexto(156); break;
            case Skills.BarreraPro: result = LocalizacionManager.instance.GetTexto(157); break;
            case Skills.Goleador: result = LocalizacionManager.instance.GetTexto(158); break;
            case Skills.Heroico: result = LocalizacionManager.instance.GetTexto(159); break;
            case Skills.Mago_balon: result = LocalizacionManager.instance.GetTexto(160); break;
            case Skills.Practico: result = LocalizacionManager.instance.GetTexto(161); break;
            case Skills.Premonicion: result = LocalizacionManager.instance.GetTexto(162); break;
            case Skills.Prima: result = LocalizacionManager.instance.GetTexto(163); break;
            case Skills.VIP: result = LocalizacionManager.instance.GetTexto(164); break;
            case Skills.Vista_halcon: result = LocalizacionManager.instance.GetTexto(165); break;
        }
        return result;
    }

    public static Skills[] GetAllHabilidades()
    {
        List<Skills> skills = new List<Skills>();
        
        int i = GameplayService.IsGoalkeeper() ? NUM_HABILIDADES_LANZADOR : 0;
        for( ; i < (GameplayService.IsGoalkeeper() ? (NUM_HABILIDADES_LANZADOR + NUM_HABILIDADES_PORTERO) : NUM_HABILIDADES_LANZADOR) ; i++)
        {
            Skills skill = (Skills)i;
            if(IsActiveSkill(skill))
            {
                skills.Add(skill);
            }
        }
        return skills.ToArray();
    }

    public static bool IsActiveSkill(Skills _skill)
    {
        bool result = false;
        if((int)_skill > NUM_HABILIDADES_LANZADOR - 1)
        {
            if(Goalkeeper.instance) result = FieldControl.instance.GoalkeeperObject.TieneHabilidad(_skill);
        }
        else
        {
            result = FieldControl.instance.ThrowerObject.TieneHabilidad(_skill);
        }

        if(_skill == Skills.Premonicion)
        {
            bool active = false;
            if(GameplayService.networked && GameplayService.IsGoalkeeper())
            {
                active = delayAlpha;
            }
            else if(GameplayService.networked)
            {
                active = delayAlphaRival;
            }

            if(!GameplayService.networked)
            {
                active = delayAlpha;
            }

            if(!active)
            {
                result = false;
            }
        }
        return result;
    }
}
