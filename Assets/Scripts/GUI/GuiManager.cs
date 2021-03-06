using UnityEngine;
using System.Collections;

public class GuiManager : MonoBehaviour
{
    private bool escIsEnabled;
    private enum GameModes
    {
        gameplay,
        paused,
        toolSelect,
        blockSelect
    };
    private GameModes gameMode;
    private GameObject player;
    private MenuActions menuAction;
    public GameHud gameHUD;
    private GameManager gameManager;
    private PauseMenu pauseMenu;
    private ToolSelectMenu toolSelectMenu;
    private BlockSelectMenu blockSelectMenu;

    public int lastMouseButton;

    void Start()
    {
        gameManager = (GameManager)FindObjectOfType(typeof(GameManager));
        player = gameManager.getPlayer();
        lastMouseButton = 0;

        escIsEnabled = true;
        setResume();

        getPauseMenu();
        getToolSelectMenu();
        getBlockSelectMenu();
        getGameHUD();
    }

    private BlockSelectMenu getBlockSelectMenu()
    {
        if (blockSelectMenu == null)
            blockSelectMenu = new BlockSelectMenu(gameManager.shipInfo);
        return blockSelectMenu;
    }

    private ToolSelectMenu getToolSelectMenu()
    {
        if (toolSelectMenu == null)
            toolSelectMenu = new ToolSelectMenu(this);
        return toolSelectMenu;
    }

    public GameHud getGameHUD()
    {
        if (gameHUD == null)
            gameHUD = new GameHud(getBlockSelectMenu());
        return gameHUD;
    }

    private PauseMenu getPauseMenu()
    {
        if (pauseMenu != null)
            return pauseMenu;

        pauseMenu = new PauseMenu();
        return pauseMenu;
    }

    private void setResume()
    {
        gameMode = GameModes.gameplay;
        setGameState(false);
    }

    private void setPause()
    {
        gameMode = GameModes.paused;
        setGameState(true);
    }

    private void setToolSelect()
    {
        gameMode = GameModes.toolSelect;
        mouseInputGoesToPlayer(false);
    }

    private void setBlockSelect()
    {
        gameMode = GameModes.blockSelect;
        mouseInputGoesToPlayer(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            lastMouseButton = 0;
        if (Input.GetMouseButtonDown(1))
            lastMouseButton = 1;

        if (!escIsEnabled && !Input.GetButton("Escape"))
            escIsEnabled = true;

        if (Input.GetButton("Escape") && escIsEnabled)
        {
            if (gameMode == GameModes.paused)
            {
                menuAction = MenuActions.resumeGame;
            }
            else if (gameMode == GameModes.gameplay)
            {
                setPause();
            }
        }

        if (Input.GetButton("Tools"))
        {
            if (gameMode == GameModes.gameplay)
                setToolSelect();
        }
        else if (gameMode == GameModes.toolSelect)
        {
            setResume();
        }

        if (Input.GetButton("Block Select"))
        {
            if (gameMode == GameModes.gameplay)
                setBlockSelect();
        }
        else if (gameMode == GameModes.blockSelect)
        {
            setResume();
        }

        if (!Screen.showCursor)
            Screen.lockCursor = true;
    }

    void OnGUI()
    {
        if (gameMode == GameModes.gameplay)
        {
            menuAction = MenuActions.none;
            gameHUD.draw();
        }
        else if (gameMode == GameModes.paused)
        {
            menuAction = pauseMenu.draw();
        }
        else if (gameMode == GameModes.toolSelect)
        {
            menuAction = toolSelectMenu.draw();
        }
        else if (gameMode == GameModes.blockSelect)
        {
            menuAction = blockSelectMenu.draw();
        }

        if (menuAction != null)
            updateMenu();
    }

    private void mouseInputGoesToPlayer(bool flag)
    {
        //this toggles whether the mouse is hidden and locked onto the player controller
        Screen.showCursor = !flag;
        Screen.lockCursor = flag;
        player.GetComponent<PlayerController>().setPlayerInput(flag);
    }

    private void setGameState(bool isPaused)
    {
        Time.timeScale = isPaused ? 0 : 1f;
        player.GetComponent<PlayerController>().setWorldCanMovePlayer(!isPaused);
        mouseInputGoesToPlayer(!isPaused);
    }

    public void updateMenu()
    {
        if (menuAction == MenuActions.resumeGame)
            setResume();

        if (menuAction == MenuActions.gotoMainMenu)
        {
            gameManager.saveGame();
            Application.LoadLevel("Main Menu");
        }
    }
}
