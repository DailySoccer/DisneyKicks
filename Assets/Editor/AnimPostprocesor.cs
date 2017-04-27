using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class AnimPostprocesor : AssetPostprocessor
{
  List<AnimationEvent> m_events = new List<AnimationEvent>();
  static AnimationDescriptorsResource m_adr;

  float m_grabEventTime;
  Vector3 m_grabEventDiff;

  static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
  {
  }
  void OnPostprocessModel(GameObject go)
  {
    if (assetPath.Contains("Animaciones") && go.GetComponent<Animation>() != null)
    {
      if (m_adr == null) m_adr = AssetDatabase.LoadAssetAtPath("Assets/Resources/AnimationDescriptorsResource.asset", typeof(AnimationDescriptorsResource)) as AnimationDescriptorsResource;
      if (m_adr == null)
      {
        m_adr = ScriptableObject.CreateInstance<AnimationDescriptorsResource>();
        AssetDatabase.CreateAsset(m_adr, "Assets/Resources/AnimationDescriptorsResource.asset");
      }

      EditorUtility.SetDirty(m_adr);

      AnimationClip[] clips = AnimationUtility.GetAnimationClips(go);
      if (clips.Length != 0)
      {
        m_grabEventTime /= clips[0].frameRate;
        m_grabEventDiff = Vector3.zero;
        foreach (AnimationClip clip in clips)
        {
          AnimationState anmsts = go.GetComponent<Animation>()[clip.name];
          anmsts.enabled = true;
          anmsts.time = m_grabEventTime;
          go.GetComponent<Animation>().Sample();

          Transform balon = go.transform.Find("Bip01/Balon");
          if (balon != null)
            m_grabEventDiff = balon.position;
          else
            m_grabEventDiff = Vector3.zero;


          AnimationClipCurveData[] curves = AnimationUtility.GetAllCurves(clip, true);
          clip.ClearCurves();
          clip.name = go.name.Substring(4);
          Vector3 vel = Vector3.zero;
          foreach (AnimationClipCurveData data in curves)
          {
            Keyframe[] keys = data.curve.keys;
            if (data.path == "Bip01")
            {
              if (data.propertyName.Contains("m_LocalPosition.x")) { vel.x = keys[keys.Length - 1].value - keys[0].value; }
              if (data.propertyName.Contains("m_LocalPosition.z")) { vel.z = keys[keys.Length - 1].value - keys[0].value; }


              if (vel.magnitude > 0.1f)
              {
                // Recorre las keys y las pone a cero.
                for (int i = 0; i < keys.Length; ++i)
                {
                  //                if (data.propertyName.Contains("m_LocalPosition.x")) keys[i].value = 0.0f;
                  //                if (data.propertyName.Contains("m_LocalPosition.z")) keys[i].value = 0.0f;
                }
              }
            }
            AnimationCurve curve = new AnimationCurve(keys);
            clip.SetCurve(data.path, data.type, data.propertyName, curve);
          }
          m_adr.Add(clip.name, vel / clip.length, m_grabEventTime, m_grabEventDiff);

          if (m_events.Count != 0)
          {
            for (int i = 0; i < m_events.Count; ++i)
            {
              m_events[i].time /= clips[0].frameRate;
            }
            AnimationUtility.SetAnimationEvents(clips[0], m_events.ToArray());
            AnimationUtility.SetAnimationClips(go.GetComponent<Animation>(), clips);
            m_events.Clear();
          }
        }
      }
    }
  }

  void OnPostprocessGameObjectWithUserProperties(GameObject _go, string[] _properties, System.Object[] _values)
  {
    if (assetPath.Contains("Animaciones"))
    {
      for (int i = 0; i < _properties.Length; i++)
      {
        if (_properties[i] == "UDP3DSMAX")
        {
          m_grabEventTime = -1;

          string val = ((string)_values[i]).Trim((char)13, (char)10);
          string[] str = val.ToLower().Split(' ');
          for (int j = 0; j < str.Length; ++j)
          {
            AnimationEvent ev = new AnimationEvent();
            string tmp = str[j];
            string param = _go.name;
            if (tmp != null && tmp != "")
            {
              int len = tmp.Length - 1;
              ev.time = (float)(System.Convert.ToInt32(tmp.Substring(1, len)));
              switch (tmp[0])
              {
                //              case 'h': Debug.Log(">>> h en "+(float)(System.Convert.ToInt32(tmp.Substring(1, len)) - 1) ); break;
                case 'h': m_grabEventTime = ev.time; break;
                case 'a': ev.functionName = "EventShow"; m_grabEventTime = ev.time; break;
                case 'd': ev.functionName = "EventHide"; m_grabEventTime = ev.time; ev.time -= 2; break;
                case 's':
                  {
                    ev.functionName = "EventSound";
                    len = tmp.IndexOf('(') - 1;
                    if (len > 0)
                    {
                      param = tmp.Substring(len + 2, tmp.Length - (len + 3));
                    } else
                    {
                      param = "Sound";
                      len = tmp.Length - 1;
                    }
                  }
                  break;
              }
              if (ev.time < 0) ev.time = 0;
              ev.stringParameter = param;
              if (tmp[0] != 'h') m_events.Add(ev);
            }
          }
        }
      }
    }
  }
}