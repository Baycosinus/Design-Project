bool interactable;
public enum InteractionType {Standard, Pickup}
public InteractionType iType;
void Start(){}
void Interact()
{
  //There will be two types of interaction.
  //1. Pickup object: Object disappears from the scene
  //  after interaction and joins to inventory.
  //2. Standard Interaction: Object plays its unique (or not so unique)
  //  animation.

  switch(iType)
  {
    case InteractionType.Standard:
      //Do Something.
      break;
    case InteractionType.Pickup:
      //Add object to inventory.
      //Destroy object instance.
      break;
  }
}
