using UnityEngine;

// Controls the main game where players will fight.
public class GameController : MonoBehaviour{
    [HideInInspector]
    // The map state controller which is in charge
    // of any map-relation function such as saving, loading,
    // and interacting with map entities.
    public MapStateController msc;

    // Map drift related fields.
    [SerializeField]
    private float driftFactor = 0.1f;
    [SerializeField]
    private float driftFadeFactor = 0.5f;
    private Vector2 mapDrift = Vector2.zero;

    // Fields related to moving the map.
    private Vector2 mapOffset = Vector2Int.zero; 
    private Vector2 initMapOffset;
    private Vector2 initMousePos;
    private Vector2 prevMousePos;
    private bool isDragging = false;
    private float driftInterpolate = 0f;
    private bool camSnapOn = false;
    private bool camDriftOn = true;

    // Information about the tile sprite.
    SpriteInfo spriteInfo;

    PlayerController playerOne;
    PlayerController playerTwo;
    
    // Creates a new map state controller and loads the tile sprite info.
    private void Start(){
        msc = gameObject.AddComponent<MapStateController>();
        msc.LoadFile();

        playerOne = gameObject.AddComponent<PlayerController>();
        playerTwo = gameObject.AddComponent<PlayerController>();
        playerOne.SetPlayerNum(1);
        playerTwo.SetPlayerNum(2);

        spriteInfo = Resources.Load("SpriteInfo/TileSpriteInfo") as SpriteInfo;

        msc.SetUnit(new Vector2Int(5, 5), msc.CreateUnit(new Vector2Int(5, 5), UnitID.TST_UNIT, playerOne).GetComponent<Unit>());
        msc.SetUnit(new Vector2Int(7, 5), msc.CreateUnit(new Vector2Int(7, 5), UnitID.TST_UNIT, playerTwo).GetComponent<Unit>());
    }

    // Allows map movement, and toggling camera options.
    private void Update(){
        // Moves the map and adds map drift when drag is over.
        if(Input.GetMouseButton(2)){
            if(!isDragging){
                isDragging = true;
                initMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                initMapOffset = mapOffset;
                mapDrift = Vector2.zero;
            }   
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseDiff = new Vector2(mousePos.x - initMousePos.x, mousePos.y - initMousePos.y);
            Vector2 posChange = camSnapOn ? Vector2Int.RoundToInt(Vector2.Scale(mouseDiff, new Vector2(1/spriteInfo.width, 1/spriteInfo.height))) : mouseDiff;
            prevMousePos = mousePos;
            mapOffset = initMapOffset + posChange;
            msc.SetMapPosition(mapOffset);
        }else if(isDragging){
            isDragging = false;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mapDrift = Vector2.Scale(new Vector2(mousePos.x - prevMousePos.x, mousePos.y - prevMousePos.y), new Vector2(driftFactor, driftFactor));
            driftInterpolate = 0f;
        }

        // Drifts the map.
        if(!mapDrift.Equals(Vector2.zero) && camDriftOn && !camSnapOn){
            mapOffset += mapDrift;
            msc.SetMapPosition(mapOffset);
            mapDrift = Vector2.Lerp(mapDrift, Vector2.zero, driftInterpolate);
            driftInterpolate += driftFadeFactor * Time.deltaTime;
        }

        // TODO: Remove these by adding user friendly functionality or putting them in a proper dev tool.
        // Toggles camera snap and camera drift.
        if(Input.GetKeyDown("c")){
            Debug.Log("Turning camera snap " + (camSnapOn ? "on" : "off"));
            camSnapOn = !camSnapOn;
        }else if(Input.GetKeyDown("d")){
            Debug.Log("Turning camera drift " + (camDriftOn ? "on" : "off"));
            camDriftOn = !camDriftOn;
        }
    }
}
