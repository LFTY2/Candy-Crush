using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int boardWidth = 8;
    private int boardHeight = 12;
    private float tileSize = 1.0f;

    [SerializeField] private GameObject[] beanPrefaps;
    private GameObject[,] board;

    private GameObject selectedObject;
    private int selectedObjectX, selectedObjectY;

    private int score;
    private bool isRefill;

    [SerializeField] Text scoreText;
    // Start is called before the first frame update
    void Start()
    {
        board = new GameObject[boardWidth, boardHeight];
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                int index = Random.Range(0, beanPrefaps.Length);
                GameObject bean = Instantiate(beanPrefaps[index], new Vector2(x * tileSize, y * tileSize), Quaternion.identity);
                board[x, y] = bean;
            }
        }
        StartCoroutine(DestroyMatches());

    }

    // Update is called once per frame
    void Update()
    {
        if (isRefill) return;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.5f, 0.5f, 0);
            int x = Mathf.FloorToInt(mousePos.x / tileSize);
            int y = Mathf.FloorToInt(mousePos.y / tileSize);
            if (x >= 0 && x < boardWidth && y >= 0 && y < boardHeight)
            {
                SelectObject(x, y);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            DeselectObject();
        }
        if (selectedObject != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectedObject.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }
    }
    private void DeselectObject()
    {
        if (selectedObject == null)
        {
            return;
        }
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.5f, 0.5f, 0);
        int x = Mathf.FloorToInt(mousePos.x / tileSize);
        int y = Mathf.FloorToInt(mousePos.y / tileSize);
        if (x >= 0 && x < boardWidth && y >= 0 && y < boardHeight)
        {
            SwapObjects(new Vector3(selectedObjectX, selectedObjectY, 0), board[x, y].transform.position);            
        }
        else
        {
            ReturnSelectedObj();
        }
        
        selectedObject = null;
    }
    private void SwapObjects(Vector3 pos1, Vector3 pos2)
    {
        
        int x1 = Mathf.FloorToInt(pos1.x / tileSize);
        int y1 = Mathf.FloorToInt(pos1.y / tileSize);
        int x2 = Mathf.FloorToInt(pos2.x / tileSize);
        int y2 = Mathf.FloorToInt(pos2.y / tileSize);
        if (x1 == x2 && y1 == y2)
        {
            ReturnSelectedObj();
        }
        SwapObjects(x1, y1, x2, y2);
    }

    private void SelectObject(int x, int y)
    {
        
        if (selectedObject != null)
        {
            return;
        }
        selectedObject = board[x, y];
        selectedObjectX = x;
        selectedObjectY = y;
    }

    private void SwapObjects(int x1, int y1, int x2, int y2)
    {
        GameObject temp = board[x1, y1];
        board[x1, y1] = board[x2, y2];
        board[x2, y2] = temp;

        Vector3 pos1 = new Vector3(x1 * tileSize, y1 * tileSize, 0);
        Vector3 pos2 = new Vector3(x2 * tileSize, y2 * tileSize, 0);

        board[x1, y1].transform.position = pos1;
        board[x2, y2].transform.position = pos2;
        StartCoroutine(DestroyMatches());
    }

    private void ReturnSelectedObj()
    {
        selectedObject.transform.position = new Vector3(selectedObjectX * tileSize, selectedObjectY * tileSize, 0);
    }

    IEnumerator DestroyMatches()
    {
        isRefill = true;
        List<List<GameObject>> matches = CalculateMatches();
        if (matches.Count == 0)
        {
            isRefill = false;
            yield break;
        }
            
        foreach (var match in matches)
        {
            foreach(GameObject bean in match)
            {
                if(bean != null)
                {
                    Destroy(bean);
                    score += 10;
                }                
            }
            UpdateScore();
            yield return new WaitForSeconds(0.1f);            
        }
        StartCoroutine(SpawnBeans());
    }

    private List<List<GameObject>> CalculateMatches()
    {
        List<List<GameObject>> matches = new List<List<GameObject>>();

        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth - 2; x++)
            {
                GameObject obj1 = board[x, y];
                GameObject obj2 = board[x + 1, y];
                GameObject obj3 = board[x + 2, y];

                if (obj1.CompareTag(obj2.tag) && obj2.CompareTag(obj3.tag))
                {
                    List<GameObject> match = new List<GameObject>();
                    match.Add(obj1);
                    match.Add(obj2);
                    match.Add(obj3);

                    int nextX = x + 3;
                    while (nextX < boardWidth && board[nextX, y].CompareTag(obj1.tag))
                    {
                        match.Add(board[nextX, y]);
                        nextX++;
                    }

                    if (match.Count >= 3)
                    {
                        matches.Add(match);
                    }
                }
            }
        }

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight - 2; y++)
            {
                GameObject obj1 = board[x, y];
                GameObject obj2 = board[x, y + 1];
                GameObject obj3 = board[x, y + 2];

                if (obj1.CompareTag(obj2.tag) && obj2.CompareTag(obj3.tag))
                {
                    List<GameObject> match = new List<GameObject>();
                    match.Add(obj1);
                    match.Add(obj2);
                    match.Add(obj3);

                    int nextY = y + 3;
                    while (nextY < boardHeight && board[x, nextY].CompareTag(obj1.tag))
                    {
                        match.Add(board[x, nextY]);
                        nextY++;
                    }

                    if (match.Count >= 3)
                    {
                        matches.Add(match);
                    }
                }
            }
        }

        return matches;
    }

    private IEnumerator SpawnBeans()
    {
        bool isAllBeansFilled = false;
        while(!isAllBeansFilled)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 1; y < boardHeight; y++)
                {
                    if (board[x, y] != null && board[x, y - 1] == null)
                    {
                        board[x, y - 1] = board[x, y];
                        board[x, y - 1].transform.position = new(x * tileSize, y - 1 * tileSize, 0);
                        board[x, y] = null;
                    }
                }
            }
            for (int x = 0; x < boardWidth; x++)
            {
                int y = 11;
                if (board[x, y] == null)
                {
                    int index = Random.Range(0, beanPrefaps.Length);
                    GameObject bean = Instantiate(beanPrefaps[index], new Vector2(x * tileSize, y * tileSize), Quaternion.identity);
                    board[x, y] = bean;
                }
            }
            isAllBeansFilled = true;
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (board[x, y] == null)
                    {
                        isAllBeansFilled = false;
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
        }        
        StartCoroutine(DestroyMatches());
    }

    private void UpdateScore()
    {
        scoreText.text = "Score: " + score.ToString(); 
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
