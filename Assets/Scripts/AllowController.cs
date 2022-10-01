using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllowController : MonoBehaviour
{
    private MyonController myonController;

    void Update()
    {
        myonController = GameObject.Find("Myon").GetComponent<MyonController>();

        bool isCharging = myonController.IsCharging;
        float chargeDir = myonController.ChargeDir;
        float chargePower = myonController.ChargePower;
        float maxChargePower = myonController.MaxChargePower;

        transform.position = GameObject.Find("Myon").transform.position;
        transform.rotation = Quaternion.Euler(0, 0, -chargeDir);

        if(isCharging){
            if(chargePower > maxChargePower / 2){
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, (2 * chargePower - maxChargePower) / maxChargePower);
                transform.localScale = new Vector3(0.3f, 0.2f + 0.15f * (2 * chargePower - maxChargePower) / maxChargePower, 1);
            }
            
        }else{
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, -1);
            transform.localScale = new Vector3(0.3f, 0.2f, 1);
        }
    }
}
