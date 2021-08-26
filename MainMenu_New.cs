using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu_New : MonoBehaviour
{    
    private bool _isControlsUp = false;
    [SerializeField] private RectTransform _ControlsUI;
    [SerializeField] private float _Speed;
    private Vector3 _startPos;
    private bool _moveUI = false;
    private Vector3 _target;


    private bool _isBluePlayer = true;
    private int _redPlayerHeadNumb;
    private int _redPlayerBodyNumb;
    private int _bluePlayerHeadNumb;
    private int _bluePlayerBodyNumb;

    private int _headCurrentNumb = 5;
    private int _bodyCurrentNumb = 5;
    [SerializeField] private GameObject[] _heads;
    [SerializeField] private GameObject[] _bodys;

    [SerializeField] private Button _redPlayerButton;
    [SerializeField] private Button _bluePlayerButton;

    [SerializeField] private GameObject[] _scooterParts;
    [SerializeField] private Material _redScooter;
    [SerializeField] private Material _blueScooter;

    [SerializeField] private GameObject _pizzaBall;
    [SerializeField] private RectTransform _front;
    [SerializeField] private RectTransform _back;

    [SerializeField] private float _fallingPizzaRange;


    private void Start()
    {
        _redPlayerHeadNumb = PlayerPrefs.GetInt("RedHead", 1);
        _redPlayerBodyNumb = PlayerPrefs.GetInt("RedBody", 1);
        _bluePlayerHeadNumb = PlayerPrefs.GetInt("BlueHead", 0);
        _bluePlayerBodyNumb = PlayerPrefs.GetInt("BlueBody", 0);
        Time.timeScale = 1;
        _startPos = _ControlsUI.localPosition;
        SelectPlayer(0);
        StartCoroutine(SpawnPizza());
    }

    private void Update()
    {
        if (_moveUI)
        {
            _ControlsUI.gameObject.SetActive(true);
            _ControlsUI.localPosition = Vector3.Lerp(
                _ControlsUI.localPosition, 
                _target,
                Time.deltaTime * _Speed);

            if (_target == Vector3.zero)
            {
                if (_ControlsUI.localPosition.y >= _target.y - 0.05)
                {
                    _moveUI = false;                    
                }
            }
            else if (Mathf.Abs(_ControlsUI.localPosition.y) >= Mathf.Abs(_target.y) - 0.05)
            {                               
                 _moveUI = false;                                    
            }            
        }        
    }
    

    public void PlayGame()
    {
        PlayerPrefs.SetInt("RedHead", _redPlayerHeadNumb);
        PlayerPrefs.SetInt("RedBody", _redPlayerBodyNumb);
        PlayerPrefs.SetInt("BlueHead", _bluePlayerHeadNumb);
        PlayerPrefs.SetInt("BlueBody", _bluePlayerBodyNumb);
        SceneManager.LoadScene(1);
    }

    public void ControlsKeys(int Up)
    {
        _moveUI = true;
        if (Up == 1)
        {            
            _target = Vector3.zero;
        }        
        else
        {                        
            _target = _startPos;
        }

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CosmeticButton(int situation)
    {
        switch (situation)
        {
            case 0: //Head >
                CosmeticScroller(_headCurrentNumb, 1, _heads);
                _headCurrentNumb += 1;
                if (_headCurrentNumb > _heads.Length - 1)
                    _headCurrentNumb = 0;

                if (_isBluePlayer)
                    _bluePlayerHeadNumb = _headCurrentNumb;
                else
                    _redPlayerHeadNumb = _headCurrentNumb;
                //Debug.Log(_headCurrentNumb);
                break;

            case 1: //Head <
                CosmeticScroller(_headCurrentNumb, -1, _heads);
                _headCurrentNumb -= 1;
                if (_headCurrentNumb < 0)
                    _headCurrentNumb = _heads.Length-1;

                if (_isBluePlayer)
                    _bluePlayerHeadNumb = _headCurrentNumb;
                else
                    _redPlayerHeadNumb = _headCurrentNumb;
                //Debug.Log(_headCurrentNumb);
                break;

            case 2: // Body >
                CosmeticScroller(_bodyCurrentNumb, 1, _bodys);
                _bodyCurrentNumb += 1;
                if (_bodyCurrentNumb > _bodys.Length - 1)
                    _bodyCurrentNumb = 0;

                if (_isBluePlayer)
                    _bluePlayerBodyNumb = _bodyCurrentNumb;
                else
                    _redPlayerBodyNumb = _bodyCurrentNumb;
                break;

            case 3: //Body <
                CosmeticScroller(_bodyCurrentNumb, -1, _bodys);
                _bodyCurrentNumb -= 1;
                if (_bodyCurrentNumb < 0)
                    _bodyCurrentNumb = _bodys.Length - 1;

                if (_isBluePlayer)
                    _bluePlayerBodyNumb = _bodyCurrentNumb;
                else
                    _redPlayerBodyNumb = _bodyCurrentNumb;
                //Debug.Log(_headCurrentNumb);
                break;

            default:
                break;
        }
    }

    private void CosmeticScroller(int currentNumb, int scrollerAmount, GameObject[] objects)
    {
        int target = currentNumb + scrollerAmount;
        if (target > objects.Length -1)
            target = 0;
        if (target < 0)
            target = objects.Length - 1;

        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(i == target);              
        }
    }

    public void ChangeButton(Button up, Button down)
    {
        up.GetComponent<Image>().color = Color.white;
        down.GetComponent<Image>().color = new Color(0.50f, 0.50f, 0.50f, 20);
   
    }

    public void SelectPlayer(int player)
    {
        switch (player)
        {
            case 0: // click blue
                _isBluePlayer = true;
                CosmeticScroller(_bluePlayerHeadNumb, 0, _heads);
                CosmeticScroller(_bluePlayerBodyNumb, 0, _bodys);
                ChangeButton(_bluePlayerButton, _redPlayerButton);
                ChangePartsColor(_blueScooter);
                break;

            case 1: // click red
                _isBluePlayer = false;
                CosmeticScroller(_redPlayerHeadNumb, 0, _heads);
                CosmeticScroller(_redPlayerBodyNumb, 0, _bodys);
                ChangeButton(_redPlayerButton, _bluePlayerButton);
                ChangePartsColor(_redScooter);               
                break;
        }
    }

    private void ChangePartsColor(Material material)
    {
        foreach (GameObject part in _scooterParts)
        {
            part.GetComponent<MeshRenderer>().material = material;
        }
    }

    private void FallingPizza()
    {
        int pizzaLayer = Random.Range(0, 2);
        float randomPos = Random.Range(-_fallingPizzaRange, _fallingPizzaRange);
        Vector3 t = new Vector3(_back.transform.position.x + randomPos,
            _back.transform.position.y,
            _back.transform.position.z);
        switch (pizzaLayer)
        {
            case 0: // behind    
                GameObject a =
                Instantiate(_pizzaBall,t,
                    Quaternion.Euler(0,0,Random.Range(0,360)),
                    _back.transform);
                a.GetComponent<RectTransform>().localScale = PizzaScale(pizzaLayer);
                break;

            case 1: // front
                GameObject b =
                Instantiate(_pizzaBall,t,
                    Quaternion.Euler(0, 0, Random.Range(0, 360)),
                    _front.transform);
                b.GetComponent<RectTransform>().localScale = PizzaScale(pizzaLayer);
                break;
        }        
    }

    IEnumerator SpawnPizza()
    {
        FallingPizza();
        yield return new WaitForSeconds(Random.Range(1,5));
        StartCoroutine(SpawnPizza());
    }

    private Vector3 PizzaScale(int front)
    {
        float f;
        if (front != 0)
        {
            f = Random.Range(0.5f, 1f);
        }
        else
        {
            f = Random.Range(0.3f, 0.5f);
        }
        return new Vector3(f, f, f);
        
    }
}
