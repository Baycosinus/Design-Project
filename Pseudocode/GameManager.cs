public FPSController fpsController;
public InteractionManager iManager;
public UIManager uiManager;

void Start()
{
  // Initialize fpsController
  // Initialize iManager
  // Initialize uiManager
  // Initialize eManager
}
void Update()
{
  fpsController.ManagePlayer();
  eManager.ManageEvents();
}
