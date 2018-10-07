public Camera camera;
public InteractionManager iManager;


void Start()
{
  //Initialize Camera
  //Initialize Interaction Manager
}
public void ManagePlayer()
{
    CheckMovement();
    CheckRotation();
    iManager.ManageInteraction();
}

void CheckMovement(){}
void CheckRotation(){}
