using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour
{
    public GameObject pool;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //Vector3 pos = new Vector3 (transform.position.x*transform.localScale.x, transform.position.y*transform.localScale.y, transform.position.z*transform.localScale.z) - Vector3.back;
        pool.transform.position = transform.position - new Vector3(0,0,0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            rb.AddForce(vertical, 0, -horizontal, ForceMode.Impulse);
            
            Debug.Log(rb.velocity);
            //Debug.Log(horizontal);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.forward, ForceMode.Impulse);
        }
    }
}
