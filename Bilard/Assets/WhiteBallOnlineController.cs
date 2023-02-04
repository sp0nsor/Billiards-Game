using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class WhiteBallOnlineController : WhiteBallController
{
    
    private PhotonView _photonView;
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        _photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    override protected void Update()
    {
        //TODO: if it is player turn then let him control stick
        base.Update();
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();

    }
    protected override void Shoot(Vector3 force)
    {
        // base.Shoot(force);
        _photonView.RPC("RPC_Shoot", RpcTarget.All, force);
    }
    [PunRPC]
    public void RPC_Shoot(Vector3 shotForce)
    {
        rb.AddForce(shotForce, ForceMode.Impulse);
    }

}
