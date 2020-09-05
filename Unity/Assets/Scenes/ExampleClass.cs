using System.Collections;
using Scenes;
using UnityEngine;
using UnityEngine.Networking;

public class ExampleClass : MonoBehaviour
{
    private WebCamTexture _texture;
    private bool _processing;
    private FaceData _faceData;

    [SerializeField] private Camera _camera;
    private MeshCollider _planeColider;

    void Start()
    {
        _texture = new WebCamTexture();
        var renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = _texture;
        _planeColider = GetComponent<MeshCollider>();
        _texture.Play();
    }

    private void Update()
    {
        Texture2D snap = new Texture2D(_texture.width, _texture.height);
        snap.SetPixels(_texture.GetPixels());
        snap.Apply();
        if (!_processing)
        {
            _processing = true;
            StartCoroutine(Upload(snap.EncodeToJPG()));
        }
    }

    IEnumerator Upload(byte[] data)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", data, "test.jpg");

        UnityWebRequest www = UnityWebRequest.Post("http://localhost:8080", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            _faceData = JsonUtility.FromJson<FaceData>(www.downloadHandler.text);
        }

        _processing = false;
    }

    private void OnGUI()
    {
        Vector3 min = _camera.WorldToScreenPoint(_planeColider.bounds.min);
        Vector3 max = _camera.WorldToScreenPoint(_planeColider.bounds.max);

        var distanceX = max.x - min.x;
        var distanceY = max.y - min.y;

        //Flip result position
        _faceData?.locations?.ForEach(location =>
        {
            GUI.Box(Rect.MinMaxRect(min.x + GetScreenPositionX(_texture.width - location.right, distanceX),
                    min.y + GetScreenPositionY(_texture.height - location.top, distanceY)
                    , min.x + GetScreenPositionX(_texture.width - location.left, distanceX),
                    min.y + GetScreenPositionY(_texture.height - location.bottom, distanceY)),
                "Face");
        });
    }

    private float GetScreenPositionX(int x, float distanceX)
    {
        var i = distanceX / _texture.width;
        return x * i;
    }

    private float GetScreenPositionY(int y, float distanceY)
    {
        var i = distanceY / _texture.height;
        return y * i;
    }
}