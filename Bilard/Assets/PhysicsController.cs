using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsController : MonoBehaviour
{
    [SerializeField]private float ballMass;
    [SerializeField]private float dragRate;
    [SerializeField]private float angularDragRate;
    public static PhysicsController instance;
    public delegate void OnPhysicsChanged();
    public static OnPhysicsChanged physicsDelegate;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        physicsDelegate.Invoke();
    }

    void Update()
    {

    }
    
    public float getBallMass()
    {
        return ballMass;
    }
    public float getDrag()
    {
        return dragRate;
    }
    public float getAngularDrag()
    {
        return angularDragRate;
    }
    public void setBallMass(float ballMass)
    {
        this.ballMass = ballMass;
    }
    public void setDrag(float dragRate)
    {
        this.dragRate = dragRate;
    }
    public void setAngularDrag(float angularDragRate)
    {
        this.angularDragRate = angularDragRate;
    }
}
