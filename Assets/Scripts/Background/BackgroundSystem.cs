using UnityEngine;

public class BackgroundSystem : MonoBehaviour
{
    public Transform generalBg;

    public PlayerController playerController;

    public GameObject bgSky;
    public GameObject bgCity;
    public GameObject bgTrain;
    public GameObject bgFactory;

    private Material bgCityMat;
    private Material bgTrainMat;
    private Material bgFactoryMat;

    public float citySpeed = 0.3f;
    public float trainSpeed = 0.5f;
    public float factorySpeed = 0.7f;

    private void Start()
    {
        bgCityMat = bgCity.GetComponent<Renderer>().material;
        bgTrainMat = bgTrain.GetComponent<Renderer>().material;
        bgFactoryMat = bgFactory.GetComponent<Renderer>().material;
    }

    private void Update()
    {
        if (playerController.isMoving)
        {
            MaterialsRepeat(!playerController.isFaceRight);
            BackgroundsFollow();
        }
    }

    private void MaterialsRepeat(bool isLeft)
    {
        if (isLeft)
        {
            citySpeed = -Mathf.Abs(citySpeed);
            trainSpeed = -Mathf.Abs(trainSpeed);
            factorySpeed = -Mathf.Abs(factorySpeed);
        }
        else
        {
            citySpeed = Mathf.Abs(citySpeed);
            trainSpeed = Mathf.Abs(trainSpeed);
            factorySpeed = Mathf.Abs(factorySpeed);
        }

        float offsetCity = Time.time * citySpeed;
        float offsetTrain = Time.time * trainSpeed;
        float offsetFactory = Time.time * factorySpeed;

        bgCityMat.mainTextureOffset = new Vector2(offsetCity, 0);
        bgTrainMat.mainTextureOffset = new Vector2(offsetTrain, 0);
        bgFactoryMat.mainTextureOffset = new Vector2(offsetFactory, 0);
    }

    private void BackgroundsFollow()
    {
        Vector3 playerPos = Camera.main.transform.position;
        generalBg.position = new Vector3(playerPos.x, 1.3f, 5);
    }
}
