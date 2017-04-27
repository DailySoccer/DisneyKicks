using UnityEngine;
using System.Collections.Generic;

public class ShotFeedbackManager : MonoBehaviour {    

    public GameObject shotFeedbackPrefab = null;

    public GameObject extraLifePrefab = null;

    public enum ShotFeedbackTypes {
        Score,
        EffectBonus,
        YellowZone,
        ShotReview,
        ExtraLife,
    }

    public GameObject SpawnShotFeedback (string feedbackText, 
                                         Vector3 feedbackPosition, 
                                         ShotFeedbackTypes feedbackType) {
        GameObject go = Instantiate( shotFeedbackPrefab ) as GameObject;
        ShotFeedbackUnit sfu = go.GetComponent<ShotFeedbackUnit>();
        if ( sfu != null ) {
            sfu.SetText( feedbackText );
            Vector3 vector = Camera.main.WorldToViewportPoint( feedbackPosition );
            vector.x = Mathf.Clamp(vector.x, 0.1f, 0.9f);
            sfu.SetPosition( vector );
            ConfigureShotFeedback( feedbackType, sfu );
            sfu.AdjustGUISize();
        }

        return go;
    }

    public GameObject SpawnShotFeedback (MultiColoredString feedbackText, 
                                         Vector3 feedbackPosition, 
                                         ShotFeedbackTypes feedbackType) {
        GameObject go = Instantiate( shotFeedbackPrefab ) as GameObject;
        ShotFeedbackUnit sfu = go.GetComponent<ShotFeedbackUnit>();
        if ( sfu != null ) {
            sfu.SetMulticoloredText( feedbackText );
            sfu.SetPosition( Camera.main.WorldToViewportPoint( feedbackPosition ) );
            ConfigureShotFeedback( feedbackType, sfu );
            sfu.AdjustGUISize();
        }

        return go;
    }

    private static Vector3 _shotReviewFeedback = new Vector3( 0.5f, 0.75f, 10.0f );

    public GameObject SpawnShotReviewFeedback (string feedbackText) {
        GameObject go = Instantiate( shotFeedbackPrefab ) as GameObject;
        ShotFeedbackUnit sfu = go.GetComponent<ShotFeedbackUnit>();
        if ( sfu != null ) {
            sfu.SetText( feedbackText );
            sfu.SetPosition( _shotReviewFeedback );
            ConfigureShotFeedback( ShotFeedbackTypes.ShotReview, sfu );
            sfu.AdjustGUISize();
        }

        return go;
    }

    public GameObject SpawnExtraLifeFeedback (int extraLives, Vector3 position) {
        GameObject go = Instantiate( extraLifePrefab ) as GameObject;
        ShotFeedbackUnit sfu = go.GetComponent<ShotFeedbackUnit>();
        if ( sfu != null ) {
            sfu.SetText( "+" + extraLives.ToString() );
            sfu.SetPosition( Camera.main.WorldToViewportPoint( position ) );
            ConfigureShotFeedback( ShotFeedbackTypes.ExtraLife, sfu );
            sfu.AdjustGUISize();
        }

        return go;
    }

    private void ConfigureShotFeedback (ShotFeedbackTypes feedbackType, ShotFeedbackUnit feedbackUnit) {
        switch ( feedbackType ) {
            case ShotFeedbackTypes.Score:
                feedbackUnit.SetColor( new Color( 252.0f / 255.0f, 218.0f / 255.0f, 19.0f / 255.0f, 1.0f ) );
                feedbackUnit.SetFontSize( 58 );
                feedbackUnit.SetShadowThickness( 3 );
                feedbackUnit.SetLifetime( 1.0f );
                feedbackUnit.SetMovementAnimation( Vector3.up, 0.1f );
                feedbackUnit.SetAlphaAnimation( 0.0f, 1.0f );
                break;
            case ShotFeedbackTypes.EffectBonus:
                feedbackUnit.SetColor( Color.white );
                feedbackUnit.SetFontSize( 42 );
                feedbackUnit.SetShadowThickness( 3 );
                feedbackUnit.SetLifetime( 1.0f );
                feedbackUnit.SetMovementAnimation( Vector3.up, 0.1f );
                feedbackUnit.SetAlphaAnimation( 0.0f, 1.0f );
                break;
            case ShotFeedbackTypes.YellowZone:
                feedbackUnit.SetColor( Color.white );
                feedbackUnit.SetFontSize( 42 );
                feedbackUnit.SetShadowThickness( 2 );
                feedbackUnit.SetLifetime( 1.0f );
                feedbackUnit.SetAlphaAnimation( 0.0f, 1.0f );
                break;
            case ShotFeedbackTypes.ShotReview:
                feedbackUnit.SetColor( Color.white );
                feedbackUnit.SetFontSize( 90 );
                feedbackUnit.SetShadowThickness( 5 );
                feedbackUnit.SetLifetime( 1.5f );
                break;
            case ShotFeedbackTypes.ExtraLife:
                feedbackUnit.SetColor( Color.white );
                feedbackUnit.SetFontSize( 42 );
                feedbackUnit.SetShadowThickness( 2 );
                feedbackUnit.SetLifetime( 1.0f );
                feedbackUnit.SetMovementAnimation( Vector3.up, 0.1f );
                feedbackUnit.SetAlphaAnimation( 0.0f, 1.0f );
                break;
        }
    }
    
    #region MonoBehaviour

    void Awake () {
        _instance = this;
	}	

    #endregion

    #region Singleton

    private static ShotFeedbackManager _instance = null;
    public static ShotFeedbackManager Instance {
        get { return _instance; }
    }

    #endregion
}
