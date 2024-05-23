using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class randomDice : MonoBehaviour
{

    public Animator ani;
    public SpriteRenderer spriteRenderer;
    public GameObject Player;
    public GameObject Board;
    public GameObject[] Cell;
    
    
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        // Cell = Board.GetComponents<GameObject>(CompareTag("Cell"));
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RollDice();
        }
    }

    public void RollDice()
    {
        //play animation
        ani.Play("Roll");
        //GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Assets/QuocElements/Asset/DiceSprSheetX128.png");
        //Debug.Log(GetComponent<SpriteRenderer>().sprite.name);

        //gán số đã quay lên
    }

    public void DoneDice()
    {
        int num = Random.Range(1, 7); // Sử dụng Random.Range thay vì Random.RandomRange
        string nameDice = "Sprites/" + num;
        //Sprite sprite = Resources.Load<Sprite>(nameDice);
        Debug.Log(num + " Name: " + nameDice);
        ani.Play("" + num);
        Player.transform.position = GameObject.Find("C"+num).transform.position;

        



        //if (sprite != null)
        //{
        //    // Gán sprite mới cho SpriteRenderer
        //    spriteRenderer.sprite = sprite;
        //}
        //else
        //{
        //    Debug.LogError($"Sprite '{nameDice}' không tồn tại trong thư mục 'Resources/Sprites'");
        //}

        //Debug.Log("Now: " + spriteRenderer.sprite.name);
    }




}
