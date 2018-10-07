public UIManager uiManager;
public EventManager nearObjectEventManager;
GameObject nearObject;

void Start()
{
  //Initialize uiManager
}
public void ManageInteraction()
{
  if(checkProximity() != null)
  {
    //If there is an interactable object
    //nearby...
    uiManager.HighLightObject(nearobject);

    if(/*Keypress*/)
    {
      nearObjectEventManager.Interact();
    }
  }
}

GameObject CheckProximity()
{
  //Raycast any proximity object
  GameObject thatObject;
  //Initialize nearObjectEventManager for that object
  if(eventManagerOfThatObject.interactable)
  {
    nearObject = thatObject;
  }
  return nearObject;
}
