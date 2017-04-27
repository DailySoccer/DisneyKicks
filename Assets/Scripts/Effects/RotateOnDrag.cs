using UnityEngine;
using System.Collections;

public class RotateOnDrag : MonoBehaviour {

  public bool goalkeeper;
  Quaternion originalRotation;
  public Vector3 originalPosition;
  Vector3 lastMouse;
  bool isOnDrag = false;
  bool dragInitialised = false;
  bool initialised = false;


  void OnMouseDrag()
  {
    if(!dragInitialised)
    {
      lastMouse = Input.mousePosition;
      dragInitialised = true;
      ifcBase.blocked = true;
    }
    isOnDrag = true;
    Vector3 delta = lastMouse - Input.mousePosition;
    Transform target = goalkeeper ? Interfaz.instance.goalkeeperModel.transform : Interfaz.instance.throwerModel.transform;
    //target.Rotate(new Vector3(0,delta.x / 2.5f,0));
    target.RotateAround(transform.position, Vector3.up, (delta.x * (940f /*ancho original*/ / Screen.width)) / 1.5f);
    lastMouse = Input.mousePosition;
  }

  public void Update()
  {
      if (!initialised) {
          if (Interfaz.instance.goalkeeperModel != null && Interfaz.instance.throwerModel != null) {
              initialised = true;
              originalRotation = goalkeeper ? Interfaz.instance.goalkeeperModel.transform.rotation : Interfaz.instance.throwerModel.transform.rotation;
              originalPosition = goalkeeper ? Interfaz.instance.goalkeeperModel.transform.position : Interfaz.instance.throwerModel.transform.position;
          }
      }

    if(isOnDrag)
    {
      isOnDrag = false;
    }
    else
    {
      if(dragInitialised)
      {
        ifcBase.blocked = false;
        dragInitialised = false;
      }
      if((goalkeeper ? Interfaz.instance.goalkeeperModel : Interfaz.instance.throwerModel) != null)
      {
        Transform target = goalkeeper ? Interfaz.instance.goalkeeperModel.transform : Interfaz.instance.throwerModel.transform;
        target.rotation = Quaternion.Lerp(target.rotation, originalRotation, 0.25f);
        target.position = Vector3.Lerp(target.position, transform.parent.transform.position, 0.25f);
      }
    }
  }
}
