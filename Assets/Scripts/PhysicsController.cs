using UnityEngine;

public class PhysicsController : MonoBehaviour
{
    [SerializeField] private float ballMass;
    [SerializeField] private float dragRate;
    [SerializeField] private float angularDragRate;
    [SerializeField] private float defaultMass;
    [SerializeField] private float defaultDrag;
    [SerializeField] private float defaultAngDrag;
    private float _tempMass, _tempDrag, _tempAngDrag;
    public static PhysicsController Instance;
    public delegate void OnPhysicsChanged();
    public static OnPhysicsChanged PhysicsDelegate;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    
    void Start()
    {
        PhysicsDelegate.Invoke();
    }
    
    public void SetDefaultPhysics()
    {
        ballMass = defaultMass;
        _tempMass = defaultMass;
        dragRate = defaultDrag;
        _tempDrag = defaultDrag;
        angularDragRate = defaultAngDrag;
        _tempAngDrag = defaultAngDrag;
    }
    
    public void ApplyPhysicsChanges()
    {
        ballMass = _tempMass;
        dragRate = _tempDrag;
        angularDragRate = _tempAngDrag;
    }
    
    public float GetBallMass()
    {
        return ballMass;
    }
    
    public float GetDefaultBallMass()
    {
        return defaultMass;
    }
    
    public float GetDrag()
    {
        return dragRate;
    }
    
    public float GetDefaultDrag()
    {
        return defaultDrag;
    }
    
    public float GetAngularDrag()
    {
        return angularDragRate;
    }
    
    public float GetDefaultAngularDrag()
    {
        return defaultAngDrag;
    }
    
    public void SetBallMass(float ballMass)
    {
        this.ballMass = ballMass;
    }
    
    public void SetTempMass(float ballMass)
    {
        _tempMass = ballMass;
    }
    
    public void SetDrag(float dragRate)
    {
        this.dragRate = dragRate;
    }
    
    public void SetTempDrag(float dragRate)
    {
        _tempDrag = dragRate;
    }
    
    public void SetAngularDrag(float angularDragRate)
    {
        this.angularDragRate = angularDragRate;
    }
    
    public void SetTempAngularDrag(float angularDragRate)
    {
        _tempAngDrag = angularDragRate;
    }
}
