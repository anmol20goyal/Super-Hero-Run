using UnityEngine;

public class GamePlay : MonoBehaviour
{
    [SerializeField]
    Animator rigAnimator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStart()
    {
        rigAnimator.CrossFade("", 0.1f);
    }
}
