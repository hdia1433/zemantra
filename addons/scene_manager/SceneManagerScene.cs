using Godot;
using System.Collections.Generic;

public partial class SceneManagerScene: Node
{
	private Node2D scene2dRoot;
    private Node2D currentScene2d;
	private Control hudRoot;
    private Control currentHud;
	private Node3D scene3dRoot;
    private Node3D currentScene3d;
	private AnimationPlayer fadeTransitionPlayer;

    private Dictionary<string, Control> loadedHuds;
    private string currentLoaded;

    public SceneManagerScene()
    {
        SceneManager.scene = this;

        loadedHuds = new();
    }
    public override void _EnterTree()
    {
		scene2dRoot = GetNodeOrNull<Node2D>("Scene 2D");
		hudRoot = GetNodeOrNull<Control>("CanvasLayer/HUD");
		scene3dRoot = GetNodeOrNull<Node3D>("Scene 3D");
		fadeTransitionPlayer = GetNode<AnimationPlayer>("CanvasLayer/Fade Transition/AnimationPlayer");

        if(scene2dRoot.GetChildCount() == 1)
        {
            currentScene2d = (Node2D)scene2dRoot.GetChild(0);
        }

        if(hudRoot.GetChildCount() == 1)
        {
            currentHud = (Control)hudRoot.GetChild(0);
        }

        if(scene3dRoot.GetChildCount() == 1)
        {
            currentScene3d = (Node3D)scene3dRoot.GetChild(0);
        }
    }

	public override void _Ready()
	{
		scene2dRoot = GetNodeOrNull<Node2D>("Scene 2D");
		hudRoot = GetNodeOrNull<Control>("CanvasLayer/HUD");
		scene3dRoot = GetNodeOrNull<Node3D>("Scene 3D");
		fadeTransitionPlayer = GetNode<AnimationPlayer>("CanvasLayer/Fade Transition/AnimationPlayer");

		      if(scene2dRoot.GetChildCount() == 1)
		      {
		          currentScene2d = (Node2D)scene2dRoot.GetChild(0);
		      }

		      if(hudRoot.GetChildCount() == 1)
		      {
		          currentHud = (Control)hudRoot.GetChild(0);
		      }

		      if(scene3dRoot.GetChildCount() == 1)
		      {
		          currentScene3d = (Node3D)scene3dRoot.GetChild(0);
		      }
	}

	public async void ChangeOutScene2d(string scenePath)
	{
		fadeTransitionPlayer.Play("fade_out");

		await ToSignal(fadeTransitionPlayer, AnimationPlayer.SignalName.AnimationFinished);
		
		Node2D newScene = GD.Load<PackedScene>(scenePath).Instantiate<Node2D>();
        if(currentScene2d != null)
        {
            currentScene2d.QueueFree();
        }
		scene2dRoot.AddChild(newScene);
        currentScene2d = newScene;
		
		fadeTransitionPlayer.Play("fade_in");

		await ToSignal(fadeTransitionPlayer, AnimationPlayer.SignalName.AnimationFinished);
	}

    public async void SwitchToNewHud(string scenePath, string sceneId, bool transition = true, bool unloadCurrent = false)
    {
        if(transition)
        {
            fadeTransitionPlayer.Play("fade_out");
            await ToSignal(fadeTransitionPlayer, AnimationPlayer.SignalName.AnimationFinished);
        }

        Control newHud = GD.Load<PackedScene>(scenePath).Instantiate<Control>();
        if(unloadCurrent)
        {
            loadedHuds.Remove(currentLoaded);
            currentHud.QueueFree();
        }
        else if(!loadedHuds.ContainsValue(currentHud))
        {
            currentHud.QueueFree();
        }
        else
        {
            currentHud.Visible = false;
            currentHud.ProcessMode = ProcessModeEnum.Disabled;
        }

        hudRoot.AddChild(newHud);
        currentHud = newHud;
        loadedHuds.Add(sceneId, newHud);
        currentLoaded = sceneId;

        if(transition)
        {
            fadeTransitionPlayer.Play("fade_in");
            await ToSignal(fadeTransitionPlayer, AnimationPlayer.SignalName.AnimationFinished);
        }
    }

    public async void SwitchToNewHud(Control newHud, string sceneId, bool transition = true, bool unloadCurrent = false)
    {
        if(transition)
        {
            fadeTransitionPlayer.Play("fade_out");
            await ToSignal(fadeTransitionPlayer, AnimationPlayer.SignalName.AnimationFinished);
        }

        if(unloadCurrent)
        {
            loadedHuds.Remove(currentLoaded);
            currentHud.QueueFree();
        }
        else if(!loadedHuds.ContainsValue(currentHud))
        {
            currentHud.QueueFree();
        }
        else
        {
            currentHud.Visible = false;
            currentHud.ProcessMode = ProcessModeEnum.Disabled;
        }

        hudRoot.AddChild(newHud);
        currentHud = newHud;
        loadedHuds.Add(sceneId, newHud);
        currentLoaded = sceneId;

        if(transition)
        {
            fadeTransitionPlayer.Play("fade_in");
            await ToSignal(fadeTransitionPlayer, AnimationPlayer.SignalName.AnimationFinished);
        }
    }

    public void HideLoadedHud()
    {
        currentHud.Visible = false;
        currentHud.ProcessMode = ProcessModeEnum.Disabled;
    }

    public void AddLoadedHud(Control newHud, string sceneId)
    {
        newHud.Visible = false;
        newHud.ProcessMode = ProcessModeEnum.Disabled;
        hudRoot.AddChild(newHud);

        loadedHuds.Add(sceneId, newHud);
    }

    public async void SwitchToLoadedHud(string sceneId, bool transition = true, bool unloadCurrent = false)
    {
        if(transition)
        {
            fadeTransitionPlayer.Play("fade_out");
            await ToSignal(fadeTransitionPlayer, AnimationPlayer.SignalName.AnimationFinished);
        }

        Control newHud = loadedHuds[sceneId];
        if(currentHud == null)
        {

        }
        else if(unloadCurrent)
        {
            loadedHuds.Remove(currentLoaded);
            currentHud.QueueFree();
        }
        else if(!loadedHuds.ContainsValue(currentHud))
        {
            currentHud.QueueFree();
        }
        else
        {
            currentHud.Visible = false;
            currentHud.ProcessMode = ProcessModeEnum.Disabled;
        }

        currentHud = newHud;
        currentLoaded = sceneId;
        currentHud.Visible = true;
        currentHud.ProcessMode = ProcessModeEnum.Inherit;
    }

    public void clearLoadedHuds()
    {
        foreach(var (_, value) in loadedHuds)
        {
            value.QueueFree();
        }

        loadedHuds.Clear();
        currentLoaded = "";

        if(hudRoot.GetChildCount() == 1)
        {
            currentHud = (Control)hudRoot.GetChild(0);
        }
    }
}
