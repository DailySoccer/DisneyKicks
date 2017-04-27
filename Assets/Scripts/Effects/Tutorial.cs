using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

    LineRenderer line;
    bool isActive;
    int step = 0;
    public bool hide = false;

    public static int[] tutorialIndexes = {218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237};

    public void ActivateTutorial()
    {
        isActive = true;
        PopUpVisible(true);
    }

    public static Tutorial instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        //ServiceLocator.Request<IShotService>().RegisterListener(EraseTutorial);
        ServiceLocator.Request<IShotService>().RegisterListener(RecordTarget);
        ServiceLocator.Request<IDefenseService>().RegisterListener(StopGKTuto);
    }

    void RecordTarget(ShotInfo _info)
    {
        if(isActive)
        {
            hide = false;
        }
    }

    void StopGKTuto(DefenseInfo _info)
    {
        if(!GameplayService.IsGoalkeeper()) return;
        hide = true;
        step++;
        if(step > 1)
        {
            isActive = false;
            PopUpVisible(false);
            line.SetVertexCount(0);
        }
    }

    public void EraseTutorial(ShotInfo _info)
    {
        line.SetVertexCount(0);
        PopUpVisible(false);
        hide = true;
        step++;
        if(step > 3)
        {
            PopUpVisible(false);
            line.SetVertexCount(0);
            isActive = false;
        }
    }

    public void DeactivateTutorial()
    {
        PopUpVisible(false);
        hide = true;
        isActive = false;
    }

    string m_textoActual;

    public void ThrowTuto(string _texto)
    {
        m_textoActual = _texto;
        ActivateTutorial();
        EnableTutorial();
    }

    public void EnableTutorial()
    {
        if(isActive)
        {
            PopUpVisible(true);
            hide = false;
        }
    }

    float m_offset = 0f;
    Vector3 prevCameraPosition = Vector3.zero;
    void Update()
    {
        m_offset += Time.deltaTime * 1.5f;
        line.material.SetTextureOffset("_MainTex", new Vector2(m_offset, 0));

        if(isActive && !hide)
        {
            GameObject.Find("pastilla_tuto/texto").GetComponent<txtText>().SetText(m_textoActual);
            GameObject.Find("pastilla_tuto/texto/sombra").GetComponent<txtText>().SetText(m_textoActual);
            if(!GameplayService.IsGoalkeeper() && BallPhysics.instance.state == BallPhysics.BallState.Waiting)
            {
                float mov = Vector3.Distance(Camera.main.transform.position, prevCameraPosition);
                prevCameraPosition = Camera.main.transform.position;
                if(MissionManager.instance.GetMission().indexMision == 0 && mov < 0.01f)
                {
                    if(ServiceLocator.Request<IDifficultyService>().GetRounds() > 2)
                    {
                        LoadThrowerTutorial_2();
                    }
                    else
                    {
                        LoadThrowerTutorial_1();
                    }
                }
            }
            if(GameplayService.IsGoalkeeper() && BallPhysics.instance.state == BallPhysics.BallState.Flying)
            {
                if(MissionManager.instance.GetMission().indexMision == 1)
                {
                    LoadGoalkeeperTutorial();
                }
            }
            else if(GameplayService.IsGoalkeeper())
            {
                line.SetVertexCount(0);
            }
        }
        /*if(isActive && !hide)
        {
            if(GameplayService.IsGoalkeeper() && BallPhysics.instance.state == BallPhysics.BallState.Flying)
            {
                GameObject.Find("pastilla_tuto/texto").GetComponent<GUIText>().text = "Para realizar una parada, sigue el\ntrazo desde el portero al punto rojo.";
                LoadGoalkeeperTutorial();
            }
            else
            {
                line.SetVertexCount(0);
            }
            
            if(!GameplayService.IsGoalkeeper() && BallPhysics.instance.state == BallPhysics.BallState.Waiting)
            {
                switch(step)
                {
                case 0:
                case 1:
                    GameObject.Find("pastilla_tuto/texto").GetComponent<GUIText>().text = "Sigue el trazo desde el balón a la diana\npara efectuar tu lanzamiento";
                    LoadThrowerTutorial_1();
                    break;
                case 2:
                case 3:
                    GameObject.Find("pastilla_tuto/texto").GetComponent<GUIText>().text = "Puedes añadir efecto a tus lanzamientos.\nSigue el trazo y verás el resultado.";
                    LoadThrowerTutorial_2();
                    break;
                }
            }
        }*/
    }

    void PopUpVisible(bool _visible)
    {
        GameObject.Find("pastilla_tuto/texto").GetComponent<GUIText>().enabled = _visible;
        GameObject.Find("pastilla_tuto/texto/sombra").GetComponent<GUIText>().enabled = _visible;
        GameObject.Find("pastilla_tuto").GetComponent<GUITexture>().enabled = _visible;
    }

    public void LoadThrowerTutorial_1()
    {
        if(BallPhysics.instance == null || ServiceLocator.Request<IDifficultyService>().GetBullseye() == null) return;
        int length = 20;
        Vector3 point1;
        Vector3 point2;


        point1 = Camera.main.WorldToScreenPoint(BallPhysics.instance.transform.position);
        point2 = Camera.main.WorldToScreenPoint(ServiceLocator.Request<IDifficultyService>().GetBullseye().transform.position);
        float interval = 1f/(float)length;

        Vector3[] points = new Vector3[length];
        points[0] = point2;
        points[length - 1] = point1;
        for(int i = 1; i < length; i++)
        {
            float t = i * interval;
            points[i] = t * (point1 - point2) + point2;
        }
        SetLine(points);
    }

    public void LoadGoalkeeperTutorial()
    {
        if(GameObject.Find ("crosshair 1(Clone)") == null) Debug.LogWarning("No hay referencia para el tutorial de portero");
        if(Goalkeeper.instance == null || GameObject.Find ("crosshair 1(Clone)") == null) return; //TODO arreglar esa aberracion de GameObject.Find alguna forma
        int length = 20;
        Vector3 point1;
        Vector3 point2;


        point1 = Camera.main.WorldToScreenPoint(new Vector3(0,0.75f,-49f));
        point2 = Camera.main.WorldToScreenPoint(GameObject.Find ("crosshair 1(Clone)").transform.position);//(ballTarget);
        float interval = 1f/(float)length;

        Vector3[] points = new Vector3[length];
        points[0] = point2;
        points[length - 1] = point1;
        for(int i = 1; i < length; i++)
        {
            float t = i * interval;
            points[i] = t * (point1 - point2) + point2;
        }
        SetLine(points);
    }

    public void LoadThrowerTutorial_2()
    {
        int length = 20;

        Vector3 point0;
        Vector3 point1;
        Vector3 point2;

        float interval = 1f/(float)length;

        point0 = Camera.main.WorldToScreenPoint(ServiceLocator.Request<IDifficultyService>().GetBullseye().transform.position);
        point1 = Camera.main.WorldToScreenPoint(BallPhysics.instance.transform.position);
        Vector3 normal = (point1 - point0);
        normal = new Vector3(normal.y, normal.x * -1f, normal.z).normalized;
        point2 = new Vector3((point0.x + point1.x)/2f,(point0.y + point1.y)/2f,(point0.z + point1.z)/2f )*1.4f + (normal * 500f);

        Vector3[] points = new Vector3[length];
        for(int i = 1; i < length; i++)
        {
            float t = i * interval;
            points[i] = (1f-t)*((1f-t)*point0 + t*point2) + t * ((1f-t)*point2 + t*point1);
        }

        points[0] = point0;
        points[length - 1] = point1;
        SetLine(points);
    }

    void SetLine(Vector3[] _points)
    {
        Transform cam = Camera.main.transform;
        Vector3 planePosition = cam.position + cam.forward * (Camera.main.nearClipPlane + 0.1f);
        Plane plane = new Plane(cam.forward, planePosition);

        line.SetVertexCount(_points.Length);
        
        for(int i = 0; i < _points.Length ; i++)
        {
            Vector3 point = _points[i];
            
            Ray ray = Camera.main.ScreenPointToRay(point);
            float distance;
            plane.Raycast(ray, out distance);
            
            line.SetPosition(i, ray.GetPoint(distance));
        }
    }
}
