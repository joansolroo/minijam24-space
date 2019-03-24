using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
[RequireComponent(typeof(ShuttleProfile))]
public class MisionControl : MonoBehaviour
{
    ShuttleProfile shuttleProfile;
    public Shuttle template;
    public Shuttle currentShuttle;
    List<Shuttle> allShuttles;


    [Header("Buttons")]
    [SerializeField] Button buttonLaunch;
    [SerializeField] Button buttonLeft;
    [SerializeField] Button buttonRight;
    [SerializeField] Button buttonForward;
    [SerializeField] Button buttonBackwards;
    [SerializeField] Button buttonStabilize;

    [SerializeField] ProgressBar fuelBar;
    [SerializeField] ProgressBar hpBar;
    [SerializeField] ProgressBar oxigenBar;
    [SerializeField] ProgressBar foodBar;

    [SerializeField] GameObject shipFail;
    [SerializeField] GameObject shipOutOfRange;
    [SerializeField] GameObject shipDead;

    [SerializeField] GameObject buttonDepartureLeft;
    [SerializeField] GameObject buttonDepartureRight;

    [SerializeField] UIFollow labelFollowingShuttle;
    [SerializeField] [Range(0, 1)] float departure;
    // Start is called before the first frame update
    void Start()
    {
        shuttleProfile = GetComponent<ShuttleProfile>();
        PrepareShip();
    }

    Vector3 targetLaunch;
    private void Update()
    {
        UpdateButtonsState();

        int layer = 2 << (5 - 1);//ui
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000, layer))
        {
            
            Vector3 p = hit.point;
            p.y = 0;
            if (Vector3.Distance(p, transform.position) < 50)
            {
                targetLaunch = p;
                transform.LookAt(hit.point);
                if (Input.GetMouseButtonUp(0))
                {
                    DoLaunch();
                }
            }
        }
    }

    public void PrepareShip()
    {
        if (currentShuttle != null)
        {
            currentShuttle.RequestAction(Shuttle.ShuttleAction.deactivate);
        }
        currentShuttle = GameObject.Instantiate<Shuttle>(template);
        currentShuttle.gameObject.SetActive(true);
        currentShuttle.transform.parent = this.transform.parent;
        currentShuttle.transform.localPosition = template.transform.localPosition;
        currentShuttle.transform.localScale = template.transform.localScale;
        currentShuttle.transform.parent = this.transform;

        labelFollowingShuttle.target = currentShuttle.gameObject;
        
        labelFollowingShuttle.gameObject.SetActive(true);

        currentShuttle.profile = shuttleProfile;
        currentShuttle.food = shuttleProfile.maxFood;
        currentShuttle.hp = shuttleProfile.maxHp;
        currentShuttle.oxigen = shuttleProfile.maxOxigen;
        currentShuttle.fuel = shuttleProfile.maxFuel;
        currentShuttle.fuelStage1 = shuttleProfile.maxFuelStage1;
    }
    public void DoLaunch()
    {
        currentShuttle.RequestAction(Shuttle.ShuttleAction.launch);
    }
    public void DoForward()
    {
        currentShuttle.RequestAction(Shuttle.ShuttleAction.forward);
    }
    public void DoBackwards()
    {
        currentShuttle.RequestAction(Shuttle.ShuttleAction.backwards);
    }
    public void DoLeft()
    {
        currentShuttle.RequestAction(Shuttle.ShuttleAction.left);
    }
    public void DoRight()
    {
        currentShuttle.RequestAction(Shuttle.ShuttleAction.right);
    }
    public void DoStabilize()
    {
        currentShuttle.RequestAction(Shuttle.ShuttleAction.stabilize);
    }
    public void DoDepartureEarlier()
    {
        this.transform.Rotate(0, -10, 0);
    }
    public void DoDepartureLater()
    {
        this.transform.Rotate(0, 10, 0);
    }
    void UpdateButtonsState()
    {
        bool launched = currentShuttle.launched;
        bool active = launched && currentShuttle.hp > 0 && currentShuttle.fuel > 0 && (!currentShuttle.tripulated || currentShuttle.food>0 && currentShuttle.oxigen>0);
        bool unreachable = currentShuttle.hp == 0 || currentShuttle.distanceToBase > shuttleProfile.maxRange;
        buttonLaunch.interactable = !launched;

        buttonForward.interactable = active;
        buttonBackwards.interactable = active;
        buttonLeft.interactable = active;
        buttonRight.interactable = active;
        buttonStabilize.interactable = active;

        buttonDepartureLeft.SetActive(!launched);
        buttonDepartureRight.SetActive(!launched);
        hpBar.SetValue(shuttleProfile.maxFuelStage1 > 0 ? (currentShuttle.fuelStage1 * 100 / shuttleProfile.maxFuelStage1) : 0);
        fuelBar.SetValue(shuttleProfile.maxFuel>0? (currentShuttle.fuel*100 / shuttleProfile.maxFuel):0);
        oxigenBar.SetValue(shuttleProfile.maxOxigen > 0 ? (currentShuttle.oxigen * 100 / shuttleProfile.maxOxigen) : 0);
        foodBar.SetValue(shuttleProfile.maxFood > 0 ? (currentShuttle.food * 100 / shuttleProfile.maxFood) : 0);
       // hpBar.SetValue(shuttleProfile.maxHp > 0 ? (currentShuttle.hp * 100 / shuttleProfile.maxHp) : 0);

        
        shipFail.SetActive(currentShuttle.fuel == 0 || currentShuttle.hp==0);
        shipDead.SetActive(currentShuttle.tripulated && (currentShuttle.food == 0 || currentShuttle.oxigen == 0));
        shipOutOfRange.SetActive(unreachable);

        labelFollowingShuttle.gameObject.SetActive(currentShuttle.hp > 0);
    }
    
    private void OnDrawGizmos()
    {
        int layer = 2 << (5-1);//ui
        RaycastHit hit;
        //if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit, 1000,layer))
        {
            Gizmos.DrawSphere(targetLaunch, 1);
            Gizmos.DrawLine(transform.position, targetLaunch);
        }
    }
}

